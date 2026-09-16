using EzySlice;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SliceObject : MonoBehaviour
{
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public VelocityEstimator velocityEstimator;
    public LayerMask sliceAbleLayer;
    public Material crossSectionMaterial;
    public float cutForce;

    [Header("Cutting Motion Filter")]
    [Tooltip("Arah gerak pisau yang dianggap sebagai ayunan memotong (world space). Untuk memotong bahan di atas talenan, pakai arah ke bawah.")]
    public Vector3 expectedCutDirection = Vector3.down;
    [Tooltip("Kecepatan minimum pisau (m/s) supaya dianggap sebagai ayunan memotong, bukan gerak pelan/diam saat diangkat.")]
    public float minCutSpeed = 0.5f;
    [Tooltip("Ambang dot product antara arah velocity dan expectedCutDirection (0..1). Semakin besar nilainya, semakin ketat arah gerak harus lurus ke bawah.")]
    [Range(0f, 1f)]
    public float directionDotThreshold = 0.3f;

    // Status linecast frame sebelumnya, dipakai untuk edge-trigger (hindari slice berulang selama masih overlap)
    private bool wasHitLastFrame = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceAbleLayer);

        // Edge-trigger: proses hanya saat transisi dari TIDAK menyentuh -> menyentuh.
        // Ini mencegah slice terpanggil berulang kali selama pisau masih overlap dengan collider yang sama,
        // misalnya saat pisau diangkat kembali setelah memotong.
        bool isRisingEdge = hasHit && !wasHitLastFrame;

        if (isRisingEdge)
        {
            Vector3 velocity = velocityEstimator.GetVelocityEstimate();

            if (IsValidCuttingMotion(velocity))
            {
                GameObject target = hit.collider.gameObject;
                Slice(target, velocity);
            }
            // Jika motion tidak valid (misalnya pisau sedang diangkat naik), linecast tetap
            // dianggap "hit" untuk state berikutnya, tapi tidak memicu slice.
        }

        wasHitLastFrame = hasHit;
    }

    // Menentukan apakah gerakan pisau saat ini layak disebut ayunan memotong
    // berdasarkan kecepatan minimum dan kesesuaian arah terhadap expectedCutDirection.
    private bool IsValidCuttingMotion(Vector3 velocity)
    {
        float speed = velocity.magnitude;

        // Terlalu pelan / nyaris diam -> bukan ayunan memotong (misalnya pisau digeser perlahan atau diangkat pelan)
        if (speed < minCutSpeed)
        {
            return false;
        }

        // Arah gerak harus searah dengan arah potong yang diharapkan (mis. ke bawah).
        // Kalau pisau sedang diangkat (arah berlawanan), dot product akan rendah/negatif -> ditolak.
        float dot = Vector3.Dot(velocity.normalized, expectedCutDirection.normalized);
        if (dot < directionDotThreshold)
        {
            return false;
        }

        return true;
    }

    public void Slice(GameObject target, Vector3 velocity)
    {
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();
        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if (hull != null)
        {
            // 1. Ambil layer dan pengaturan Grab dari objek asli sebelum dihancurkan
            int layerAsli = target.layer;
            XRGrabInteractable grabAsli = target.GetComponent<XRGrabInteractable>();

            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            SetupSlicedComponent(upperHull, layerAsli, grabAsli);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            SetupSlicedComponent(lowerHull, layerAsli, grabAsli);

            Destroy(target);
        }
    }
    public void SetupSlicedComponent(GameObject slicedObject, int layerTarget, XRGrabInteractable grabAsli)
    {
        slicedObject.layer = layerTarget;

        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;

        // --- TAMBAHAN WAJIB: Masukkan bentuk 3D ke dalam Collider ---
        MeshFilter meshFilter = slicedObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            collider.sharedMesh = meshFilter.sharedMesh;
        }

        // Tambahkan komponen Grab
        XRGrabInteractable grabBaru = slicedObject.AddComponent<XRGrabInteractable>();

        if (grabAsli != null)
        {
            grabBaru.interactionLayers = grabAsli.interactionLayers;
            grabBaru.movementType = grabAsli.movementType;
        }

        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }
}
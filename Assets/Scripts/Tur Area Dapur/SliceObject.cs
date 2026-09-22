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

    [Header("Identitas Pisau (HACCP)")]
    public TipeTalenan jenisPisauIni;
    public EvaluasiPersiapan managerPersiapan;
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

                // --- EVALUASI KESESUAIAN PISAU ---
                BahanPersiapan bahan = target.GetComponent<BahanPersiapan>();
                if (bahan == null) bahan = target.GetComponentInParent<BahanPersiapan>();
                if (bahan != null && !bahan.sudahDievaluasiPisau)
                {
                    bahan.sudahDievaluasiPisau = true; // Kunci agar tidak dievaluasi berkali-kali

                    if (bahan.talenanYangBenar != jenisPisauIni)
                    {
                        if (managerPersiapan != null)
                        {
                            managerPersiapan.CatatPelanggaranPisau(bahan.namaBahan, jenisPisauIni, bahan.talenanYangBenar);
                        }
                    }
                }

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
            int layerAsli = target.layer;
            XRGrabInteractable grabAsli = target.GetComponent<XRGrabInteractable>();

            string namaBahan = "Tidak Diketahui";
            Material materialDalam = crossSectionMaterial;

            BahanPersiapan bahanUtuh = target.GetComponent<BahanPersiapan>();
            if (bahanUtuh == null) bahanUtuh = target.GetComponentInParent<BahanPersiapan>();

            if (bahanUtuh != null)
            {
                namaBahan = bahanUtuh.namaBahan;
                if (bahanUtuh.materialBagianDalam != null) materialDalam = bahanUtuh.materialBagianDalam;
            }
            else
            {
                PotonganBahan potonganUtuh = target.GetComponent<PotonganBahan>();
                if (potonganUtuh == null) potonganUtuh = target.GetComponentInParent<PotonganBahan>();
                if (potonganUtuh != null)
                {
                    namaBahan = potonganUtuh.namaBahanAsal;
                    if (potonganUtuh.materialBagianDalamAsal != null) materialDalam = potonganUtuh.materialBagianDalamAsal;
                }
            }

            // --- HITUNG MASSA SEBELUM MEMBELAH ---
            Rigidbody targetRb = target.GetComponent<Rigidbody>();
            float massaAwal = (targetRb != null) ? targetRb.mass : 1.0f; // Ambil massa bahan saat ini (misal 1 kg)

            GameObject upperHull = hull.CreateUpperHull(target, materialDalam); // Ganti crossSectionMaterial jadi materialDalam
            GameObject lowerHull = hull.CreateLowerHull(target, materialDalam); // Ganti crossSectionMaterial jadi materialDalam
            //GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            //GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);

            // --- HITUNG VOLUME MASING-MASING POTONGAN ---
            float volUpper = 0.5f;
            float volLower = 0.5f;

            if (upperHull != null)
            {
                MeshFilter mfUpper = upperHull.GetComponent<MeshFilter>();
                if (mfUpper != null && mfUpper.sharedMesh != null)
                    volUpper = HitungVolumeMesh(mfUpper.sharedMesh);
            }

            if (lowerHull != null)
            {
                MeshFilter mfLower = lowerHull.GetComponent<MeshFilter>();
                if (mfLower != null && mfLower.sharedMesh != null)
                    volLower = HitungVolumeMesh(mfLower.sharedMesh);
            }

            // Hitung rasio pembagian massa
            float totalVolume = volUpper + volLower;
            float rasioUpper = (totalVolume > 0.0001f) ? (volUpper / totalVolume) : 0.5f;

            // Pastikan massa tidak bernilai 0 (minimal 0.02 kg / 20 gram agar fisika tetap stabil)
            float massaUpper = Mathf.Max(0.02f, massaAwal * rasioUpper);
            float massaLower = Mathf.Max(0.02f, massaAwal * (1f - rasioUpper));

            // --- SETUP MASING-MASING OBJEK ---
            if (upperHull != null)
            {
                upperHull.transform.position += planeNormal * 0.005f;
                SetupSlicedComponent(upperHull, layerAsli, grabAsli, namaBahan, massaUpper, materialDalam);
            }

            if (lowerHull != null)
            {
                lowerHull.transform.position -= planeNormal * 0.005f;
                SetupSlicedComponent(lowerHull, layerAsli, grabAsli, namaBahan, massaLower, materialDalam);
            }

            
            Destroy(target);
        }
    }
    public void SetupSlicedComponent(GameObject slicedObject, int layerTarget, XRGrabInteractable grabAsli, string namaBahan, float massaPotongan, Material materialDalam)
    {
        slicedObject.layer = layerTarget;

        MeshFilter meshFilter = slicedObject.GetComponent<MeshFilter>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();

        if (meshFilter != null)
        {
            collider.sharedMesh = meshFilter.sharedMesh;
        }
        collider.convex = true;

        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        rb.mass = massaPotongan; // <-- MASSA DITERAPKAN SESUAI BESAR POTONGAN
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        XRGrabInteractable grabBaru = slicedObject.AddComponent<XRGrabInteractable>();
        if (grabAsli != null)
        {
            grabBaru.interactionLayers = grabAsli.interactionLayers;
            grabBaru.movementType = grabAsli.movementType;
        }

        PotonganBahan ktp = slicedObject.AddComponent<PotonganBahan>();
        ktp.namaBahanAsal = namaBahan;
        ktp.materialBagianDalamAsal = materialDalam;

        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }
    private float HitungVolumeMesh(Mesh mesh)
    {
        if (mesh == null) return 0f;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        float volume = 0f;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];

            volume += Vector3.Dot(p1, Vector3.Cross(p2, p3)) / 6.0f;
        }

        return Mathf.Abs(volume);
    }
}
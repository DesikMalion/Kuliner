
using TMPro;
using UnityEngine;

public class FoodIngredientLabelManager : MonoBehaviour
{
    public static FoodIngredientLabelManager Instance;

    [Header("UI Label")]
    public GameObject labelPanel;
    public TMP_Text labelText;

    [Header("Tampilan")]
    public bool faceCamera = true;

    [Tooltip("Balik arah jika sisi depan Canvas membelakangi kamera.")]
    public bool reverseFacing = false;

    [Header("Informasi Label")]
    public bool showStorageInfo = true;

    private Transform target;
    private Renderer[] targetRenderers;
    private Vector3 currentOffset;

    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;

        // Cari kamera utama, termasuk kamera XR saat runtime.
        FindMainCamera();

        if (labelPanel != null)
            labelPanel.SetActive(false);
    }

    private void LateUpdate()
    {
        if (target == null || labelPanel == null)
            return;

        if (!labelPanel.activeSelf)
            return;

        // Posisi dan rotasi diperbarui setiap frame.
        UpdateLabelPosition();
        UpdateLabelRotation();
    }

    private void FindMainCamera()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            // Fallback jika kamera belum memiliki tag MainCamera.
            mainCamera = FindFirstObjectByType<Camera>();
        }
    }

    public void Show(
        FoodIngredient ingredient,
        Transform targetTransform,
        string description,
        Vector3 offset)
    {
        if (ingredient == null || targetTransform == null)
            return;

        target = targetTransform;
        currentOffset = offset;

        // Ambil Renderer model dan semua child-nya.
        targetRenderers =
            target.GetComponentsInChildren<Renderer>(true);

        // Perbarui isi label.
        if (labelText != null)
        {
            string info =
                "<b>" + ingredient.ingredientName + "</b>\n" +
                "Kategori: " + ingredient.category + "\n" +
                "Kondisi: " + ingredient.condition;

            if (showStorageInfo)
            {
                info += "\nPenyimpanan: " +
                        ingredient.requiredStorage;
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                info += "\n" + description;
            }

            labelText.text = info;
        }

        if (labelPanel != null)
            labelPanel.SetActive(true);

        // Langsung atur posisi dan arah ketika label muncul.
        UpdateLabelPosition();
        UpdateLabelRotation();
    }

    private void UpdateLabelPosition()
    {
        if (target == null)
            return;

        // Default: gunakan pivot bahan.
        Vector3 topPosition = target.position;

        if (targetRenderers != null && targetRenderers.Length > 0)
        {
            bool foundRenderer = false;
            Bounds combinedBounds = new Bounds();

            foreach (Renderer rend in targetRenderers)
            {
                if (rend == null || !rend.enabled ||
                    !rend.gameObject.activeInHierarchy)
                    continue;

                if (!foundRenderer)
                {
                    combinedBounds = rend.bounds;
                    foundRenderer = true;
                }
                else
                {
                    combinedBounds.Encapsulate(rend.bounds);
                }
            }

            // Ambil titik tertinggi seluruh model.
            if (foundRenderer)
            {
                topPosition = new Vector3(
                    combinedBounds.center.x,
                    combinedBounds.max.y,
                    combinedBounds.center.z
                );
            }
        }

        // Offset diukur dari bagian atas model.
        transform.position = topPosition + currentOffset;
    }

    private void UpdateLabelRotation()
    {
        if (!faceCamera)
            return;

        if (mainCamera == null)
            FindMainCamera();

        if (mainCamera == null)
            return;

        // Arahkan sisi depan Canvas ke kamera.
        Vector3 direction =
            mainCamera.transform.position - transform.position;

        if (reverseFacing)
            direction = -direction;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(
                direction.normalized,
                Vector3.up
            );
        }
    }

    public void Hide(Transform requestingTarget)
    {
        // Jangan sembunyikan label milik bahan lain.
        if (target != requestingTarget)
            return;

        target = null;
        targetRenderers = null;

        if (labelPanel != null)
            labelPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}

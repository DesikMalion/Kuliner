
using UnityEngine;

public enum IngredientType
{
    Meat,
    Fish,
    Vegetable,
    Egg
}

public class Ingredient : MonoBehaviour
{
    [Header("Ingredient Info")]
    public string ingredientName;
    public IngredientType ingredientType;

    // =========================================================
    // COOKING STATE
    // =========================================================

    [Header("Cooking State")]

    [Range(0f, 1f)]
    public float cookingProgress = 0f;

    public bool isCooking { get; private set; }
    public bool isCooked { get; private set; }

    public bool isFlipped { get; private set; }
    public bool isStirred { get; private set; }
    public bool isRotated { get; private set; }

    // =========================================================
    // COOKING VISUAL MODE
    // =========================================================

    [Header("Cooking Visual Mode")]
    public CookingVisualMode cookingVisualMode =
        CookingVisualMode.Material;

    public enum CookingVisualMode
    {
        Material,
        GameObject,
        RendererArray
    }

    // =========================================================
    // MATERIAL
    // =========================================================

    [Header("Cooking Material")]

    public Material materialMentah;
    public Material materialMatang;

    [Tooltip("Renderer untuk mode Material.")]
    public Renderer ingredientRenderer;

    // =========================================================
    // RENDERER ARRAY
    // =========================================================

    [Header("Cooking Renderer Array")]

    [Tooltip("Semua renderer yang materialnya berubah saat memasak.")]
    public Renderer[] ingredientRenderers;

    // Menyimpan material instance untuk setiap slot renderer.
    private Material[][] runtimeMaterialsArray;

    // =========================================================
    // GAMEOBJECT
    // =========================================================

    [Header("Cooking GameObject")]

    public GameObject objectMentah;
    public GameObject objectMatang;

    // =========================================================
    // COOKING SETTINGS
    // =========================================================

    [Header("Cooking Settings")]

    public bool requiresFlip = false;
    public bool requiresStir = false;
    public bool requiresRotate = false;

    [Min(1)]
    public int requiredRotations = 4;

    // =========================================================
    // ROTATE STATE
    // =========================================================

    [Header("Rotate State")]

    [Range(0f, 1f)]
    public float rotateProgress = 0f;

    public int rotationCount { get; private set; }

    // =========================================================
    // INTERNAL STATE
    // =========================================================

    private bool needFlipEventSent;
    private bool needStirEventSent;
    private bool needRotateEventSent;

    private Material runtimeMaterial;

    public GameObject[] need_gravity;
    public GameObject wadah;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (ingredientRenderer == null)
        {
            ingredientRenderer =
                GetComponentInChildren<Renderer>();
        }

        switch (cookingVisualMode)
        {
            case CookingVisualMode.Material:
                SetupSingleMaterial();
                break;

            case CookingVisualMode.GameObject:
                SetupGameObjects();
                break;

            case CookingVisualMode.RendererArray:
                SetupRendererArray();
                break;
        }

        UpdateCookingVisual();
    }

    // =========================================================
    // SETUP SINGLE MATERIAL
    // =========================================================

    private void SetupSingleMaterial()
    {
        if (ingredientRenderer == null)
            return;

        Material sourceMaterial =
            materialMentah != null
                ? materialMentah
                : ingredientRenderer.sharedMaterial;

        if (sourceMaterial == null)
            return;

        runtimeMaterial = new Material(sourceMaterial);

        // Pertahankan slot material lain jika ada.
        Material[] materials = ingredientRenderer.materials;

        if (materials.Length > 0)
        {
            materials[0] = runtimeMaterial;
            ingredientRenderer.materials = materials;
        }
    }

    // =========================================================
    // SETUP GAMEOBJECT
    // =========================================================

    private void SetupGameObjects()
    {
        if (objectMentah != null)
            objectMentah.SetActive(true);

        if (objectMatang != null)
            objectMatang.SetActive(false);
    }

    // =========================================================
    // SETUP RENDERER ARRAY
    // =========================================================

    private void SetupRendererArray()
    {
        if (ingredientRenderers == null)
            return;

        runtimeMaterialsArray =
            new Material[ingredientRenderers.Length][];

        for (int i = 0; i < ingredientRenderers.Length; i++)
        {
            Renderer rend = ingredientRenderers[i];

            if (rend == null)
                continue;

            Material[] sourceMaterials = rend.sharedMaterials;
            Material[] instances =
                new Material[sourceMaterials.Length];

            for (int j = 0; j < sourceMaterials.Length; j++)
            {
                Material source =
                    materialMentah != null
                        ? materialMentah
                        : sourceMaterials[j];

                if (source == null)
                    continue;

                instances[j] = new Material(source);
            }

            runtimeMaterialsArray[i] = instances;
            rend.materials = instances;
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!isCooking || isCooked)
            return;

        // =====================================================
        // REQUIRE FLIP
        // =====================================================

        if (requiresFlip && !isFlipped)
        {
            cookingProgress += Time.deltaTime * 0.2f;
            cookingProgress = Mathf.Clamp(cookingProgress, 0f, 0.5f);

            UpdateCookingVisual();
            UpdateProgressUI();

            if (!needFlipEventSent)
            {
                needFlipEventSent = true;

                if (CookingManager.Instance != null)
                {
                    CookingManager.Instance.CheckEvent(
                        CookingEventType.ObjectNeedFlipped,
                        gameObject
                    );
                }

                Debug.Log(ingredientName + " MEMBUTUHKAN FLIP");
            }

            return;
        }

        // =====================================================
        // REQUIRE STIR
        // =====================================================

        if (requiresStir && !isStirred)
        {
            Stirable stirable = GetComponent<Stirable>();

            if (stirable != null)
            {
                cookingProgress = Mathf.Clamp01(
                    stirable.stirProgress
                );

                UpdateCookingVisual();
                UpdateProgressUI();

                if (!needStirEventSent)
                {
                    needStirEventSent = true;

                    if (CookingManager.Instance != null)
                    {
                        CookingManager.Instance.CheckEvent(
                            CookingEventType.ObjectNeedStirred,
                            gameObject
                        );
                    }

                    Debug.Log(ingredientName + " MEMBUTUHKAN STIR");
                }
            }

            return;
        }

        // =====================================================
        // REQUIRE ROTATE
        // =====================================================

        if (requiresRotate && !isRotated)
        {
            cookingProgress += Time.deltaTime * 0.2f;

            float maxCookingProgress =
                (float)(rotationCount + 1) /
                Mathf.Max(1, requiredRotations);

            cookingProgress = Mathf.Clamp(
                cookingProgress,
                0f,
                maxCookingProgress
            );

            UpdateCookingVisual();
            UpdateProgressUI();

            if (!needRotateEventSent)
            {
                needRotateEventSent = true;

                if (CookingManager.Instance != null)
                {
                    CookingManager.Instance.CheckEvent(
                        CookingEventType.ObjectNeedRotated,
                        gameObject
                    );
                }

                Debug.Log(ingredientName + " MEMBUTUHKAN ROTATE");
            }

            return;
        }

        // =====================================================
        // NORMAL COOKING
        // =====================================================

        cookingProgress += Time.deltaTime * 0.2f;
        cookingProgress = Mathf.Clamp01(cookingProgress);

        UpdateCookingVisual();
        UpdateProgressUI();

        if (cookingProgress >= 1f)
        {
            FinishCooking();
        }
    }

    // =========================================================
    // ROTATE
    // =========================================================

    public void OnRotated()
    {
        if (!requiresRotate || isRotated)
            return;

        rotationCount++;

        Debug.Log(
            ingredientName + " ROTATE : " +
            rotationCount + "/" + requiredRotations
        );

        rotateProgress = Mathf.Clamp01(
            (float)rotationCount / Mathf.Max(1, requiredRotations)
        );

        if (rotationCount >= requiredRotations)
        {
            isRotated = true;
            needRotateEventSent = false;

            Debug.Log(ingredientName + " SUDAH SELESAI DIROTASI");

            if (CookingManager.Instance != null)
            {
                CookingManager.Instance.CheckEvent(
                    CookingEventType.ObjectRotated,
                    gameObject
                );
            }
        }
        else
        {
            needRotateEventSent = false;
        }
    }

    // =========================================================
    // UPDATE COOKING VISUAL
    // =========================================================

    private void UpdateCookingVisual()
    {
        UpdateCookingMaterial();
    }

    // =========================================================
    // UPDATE COOKING MATERIAL
    // =========================================================

    private void UpdateCookingMaterial()
    {
        float blend = Mathf.Clamp01(cookingProgress);

        // =====================================================
        // MODE 1: SINGLE RENDERER
        // =====================================================

        if (cookingVisualMode == CookingVisualMode.Material)
        {
            if (ingredientRenderer == null ||
                materialMentah == null ||
                materialMatang == null)
                return;

            if (runtimeMaterial == null)
            {
                runtimeMaterial = new Material(materialMentah);

                Material[] materials = ingredientRenderer.materials;

                if (materials.Length == 0)
                    return;

                materials[0] = runtimeMaterial;
                ingredientRenderer.materials = materials;
            }

            runtimeMaterial.Lerp(
                materialMentah,
                materialMatang,
                blend
            );
        }

        // =====================================================
        // MODE 2: GAMEOBJECT
        // =====================================================

        else if (cookingVisualMode == CookingVisualMode.GameObject)
        {
            if (blend >= 1f)
                SetCookedGameObject();
        }

        // =====================================================
        // MODE 3: RENDERER ARRAY
        // =====================================================

        else if (cookingVisualMode == CookingVisualMode.RendererArray)
        {
            if (ingredientRenderers == null ||
                runtimeMaterialsArray == null ||
                materialMentah == null ||
                materialMatang == null)
                return;

            for (int i = 0; i < ingredientRenderers.Length; i++)
            {
                if (ingredientRenderers[i] == null)
                    continue;

                if (runtimeMaterialsArray[i] == null)
                    continue;

                for (int j = 0; j < runtimeMaterialsArray[i].Length; j++)
                {
                    Material mat = runtimeMaterialsArray[i][j];

                    if (mat == null)
                        continue;

                    mat.Lerp(
                        materialMentah,
                        materialMatang,
                        blend
                    );
                }
            }
        }
    }

    // =========================================================
    // SET COOKED GAMEOBJECT
    // =========================================================

    private void SetCookedGameObject()
    {
        if (objectMentah != null)
            objectMentah.SetActive(false);

        if (objectMatang != null)
            objectMatang.SetActive(true);
    }

    // =========================================================
    // UPDATE PROGRESS UI
    // =========================================================

    private void UpdateProgressUI()
    {
        if (CookingProgressUI.Instance == null)
            return;

        CookingProgressUI.Instance.SetProgress(
            gameObject,
            cookingProgress
        );
    }

    // =========================================================
    // START COOKING
    // =========================================================

    public virtual void StartCooking()
    {
        if (isCooked)
            return;

        isCooking = true;

        Debug.Log(ingredientName + " MULAI MEMASAK");

        if (CookingProgressUI.Instance != null)
        {
            CookingProgressUI.Instance.Show(
                gameObject,
                "Memasak " + ingredientName
            );

            CookingProgressUI.Instance.SetProgress(
                gameObject,
                cookingProgress
            );
        }
    }

    // =========================================================
    // STOP COOKING
    // =========================================================

    public virtual void StopCooking()
    {
        isCooking = false;

        Debug.Log(ingredientName + " BERHENTI MEMASAK");
    }

    // =========================================================
    // FLIP
    // =========================================================

    public void OnFlipped()
    {
        if (isFlipped)
            return;

        isFlipped = true;
        needFlipEventSent = false;

        Debug.Log(ingredientName + " SUDAH DIBALIK");
    }

    // =========================================================
    // STIR
    // =========================================================

    public void OnStirred()
    {
        if (isStirred)
            return;

        isStirred = true;
        needStirEventSent = false;

        cookingProgress = 1f;

        UpdateCookingVisual();
        UpdateProgressUI();

        Debug.Log(ingredientName + " SUDAH DIADUK");

        FinishCooking();
    }

    // =========================================================
    // FINISH COOKING
    // =========================================================

    private void FinishCooking()
    {
        if (isCooked)
            return;

        if (requiresFlip && !isFlipped)
            return;

        if (requiresStir && !isStirred)
            return;

        if (requiresRotate && !isRotated)
            return;

        isCooked = true;
        isCooking = false;
        cookingProgress = 1f;

        if (CookingProgressUI.Instance != null)
        {
            CookingProgressUI.Instance.SetProgress(
                gameObject,
                1f
            );

            CookingProgressUI.Instance.ShowComplete(
                gameObject,
                "Memasak Selesai",
                ingredientName + " sudah matang."
            );
        }

        UpdateCookingVisual();

        Debug.Log(ingredientName + " SUDAH MATANG!");

        if (CookingManager.Instance != null)
        {
            CookingManager.Instance.CheckEvent(
                CookingEventType.ObjectCooked,
                gameObject
            );
        }
    }

    // =========================================================
    // RESET COOKING
    // =========================================================

    public void ResetCooking()
    {
        cookingProgress = 0f;

        isCooking = false;
        isCooked = false;

        isFlipped = false;
        isStirred = false;
        isRotated = false;

        rotateProgress = 0f;
        rotationCount = 0;

        needFlipEventSent = false;
        needStirEventSent = false;
        needRotateEventSent = false;

        if (cookingVisualMode == CookingVisualMode.GameObject)
        {
            SetupGameObjects();
        }
        else
        {
            UpdateCookingVisual();
        }
    }

    // =========================================================
    // SET RIG INGREDIENT
    // =========================================================

    public void SetRigIngredient()
    {
        if (wadah != null)
            wadah.SetActive(false);

        if (need_gravity == null)
            return;

        for (int i = 0; i < need_gravity.Length; i++)
        {
            GameObject obj = need_gravity[i];

            if (obj == null)
                continue;

            Rigidbody rb = obj.GetComponent<Rigidbody>();

            if (rb == null)
                continue;

            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }
}
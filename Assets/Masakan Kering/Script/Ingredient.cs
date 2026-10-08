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

    // Apakah ingredient sudah pernah dibalik
    public bool isFlipped { get; private set; }

    // Apakah ingredient sudah pernah diaduk
    public bool isStirred { get; private set; }

    // Apakah ingredient sudah selesai dirotasi
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
        GameObject
    }


    // =========================================================
    // MATERIAL
    // =========================================================

    [Header("Cooking Material")]

    public Material materialMentah;

    public Material materialMatang;

    [Tooltip("Renderer yang digunakan ingredient.")]
    public Renderer ingredientRenderer;


    // =========================================================
    // GAMEOBJECT
    // =========================================================

    [Header("Cooking GameObject")]

    [Tooltip("Object ingredient saat masih mentah.")]
    public GameObject objectMentah;

    [Tooltip("Object ingredient saat sudah matang.")]
    public GameObject objectMatang;


    // =========================================================
    // COOKING SETTINGS
    // =========================================================

    [Header("Cooking Settings")]

    // Apakah ingredient membutuhkan flip
    public bool requiresFlip = false;

    // Apakah ingredient membutuhkan stir
    public bool requiresStir = false;

    // Apakah ingredient membutuhkan rotate
    public bool requiresRotate = false;

    // Berapa kali rotate yang dibutuhkan
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
        // =====================================================
        // CARI RENDERER
        // =====================================================

        if (ingredientRenderer == null)
        {
            ingredientRenderer =
                GetComponentInChildren<Renderer>();
        }


        // =====================================================
        // SETUP MATERIAL
        // =====================================================

        if (cookingVisualMode ==
            CookingVisualMode.Material)
        {
            if (ingredientRenderer != null &&
                ingredientRenderer.sharedMaterial != null)
            {
                runtimeMaterial =
                    new Material(
                        ingredientRenderer.sharedMaterial
                    );

                ingredientRenderer.material =
                    runtimeMaterial;
            }

            UpdateCookingMaterial();
        }


        // =====================================================
        // SETUP GAMEOBJECT
        // =====================================================

        else if (cookingVisualMode ==
                 CookingVisualMode.GameObject)
        {
            SetupGameObjects();
        }
    }


    // =========================================================
    // SETUP GAMEOBJECT
    // =========================================================

    private void SetupGameObjects()
    {
        if (objectMentah != null)
        {
            objectMentah.SetActive(true);
        }

        if (objectMatang != null)
        {
            objectMatang.SetActive(false);
        }
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!isCooking)
            return;

        if (isCooked)
            return;


        // =====================================================
        // REQUIRE FLIP
        // =====================================================

        if (requiresFlip && !isFlipped)
        {
            // Sisi pertama hanya boleh masak sampai 50%

            cookingProgress +=
                Time.deltaTime * 0.2f;

            cookingProgress =
                Mathf.Clamp(
                    cookingProgress,
                    0f,
                    0.5f
                );


            UpdateCookingVisual();
UpdateProgressUI();

            // Kirim event hanya SATU KALI
            if (!needFlipEventSent)
            {
                needFlipEventSent = true;

                CookingManager.Instance.CheckEvent(
                    CookingEventType.ObjectNeedFlipped,
                    gameObject
                );

                Debug.Log(
                    ingredientName +
                    " MEMBUTUHKAN FLIP"
                );
            }

            return;
        }


        // =====================================================
        // REQUIRE STIR
        // =====================================================

    
// =====================================================
// REQUIRE STIR
// =====================================================

if (requiresStir && !isStirred)
{
    Stirable stirable =
        GetComponent<Stirable>();

    if (stirable != null)
    {
        // =================================================
        // COOKING PROGRESS MENGIKUTI STIR PROGRESS
        // =================================================

        cookingProgress =
            stirable.stirProgress;

        cookingProgress =
            Mathf.Clamp01(cookingProgress);


        // =================================================
        // UPDATE VISUAL
        // =================================================

        UpdateCookingVisual();
        UpdateProgressUI();


        // =================================================
        // KIRIM EVENT NEED STIR
        // =================================================

        if (!needStirEventSent)
        {
            needStirEventSent = true;

            CookingManager.Instance.CheckEvent(
                CookingEventType.ObjectNeedStirred,
                gameObject
            );

            Debug.Log(
                ingredientName +
                " MEMBUTUHKAN STIR"
            );
        }
    }

    return;
}



        // =====================================================
        // REQUIRE ROTATE
        // =====================================================

        if (requiresRotate && !isRotated)
        {
            cookingProgress +=
                Time.deltaTime * 0.2f;


            // =================================================
            // BATASI COOKING BERDASARKAN ROTASI
            // =================================================

            float maxCookingProgress =
                (float)(rotationCount + 1) /
                Mathf.Max(
                    1,
                    requiredRotations
                );


            cookingProgress =
                Mathf.Clamp(
                    cookingProgress,
                    0f,
                    maxCookingProgress
                );


            UpdateCookingVisual();
UpdateProgressUI();

            // =================================================
            // KIRIM EVENT NEED ROTATE
            // =================================================

            if (!needRotateEventSent)
            {
                needRotateEventSent = true;

                CookingManager.Instance.CheckEvent(
                    CookingEventType.ObjectNeedRotated,
                    gameObject
                );

                Debug.Log(
                    ingredientName +
                    " MEMBUTUHKAN ROTATE"
                );
            }

            return;
        }


        // =====================================================
        // BOLEH SELESAI MEMASAK
        // =====================================================

        cookingProgress +=
            Time.deltaTime * 0.2f;

        cookingProgress =
            Mathf.Clamp01(
                cookingProgress
            );


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
        if (!requiresRotate)
            return;

        if (isRotated)
            return;


        rotationCount++;


        Debug.Log(
            ingredientName +
            " ROTATE : " +
            rotationCount +
            "/" +
            requiredRotations
        );


        // =====================================================
        // UPDATE ROTATE PROGRESS
        // =====================================================

        rotateProgress =
            (float)rotationCount /
            Mathf.Max(
                1,
                requiredRotations
            );

        rotateProgress =
            Mathf.Clamp01(
                rotateProgress
            );


        // =====================================================
        // CEK SELESAI ROTATE
        // =====================================================

        if (rotationCount >= requiredRotations)
        {
            isRotated = true;

            needRotateEventSent = false;

            Debug.Log(
                ingredientName +
                " SUDAH SELESAI DIROTASI"
            );


            CookingManager.Instance.CheckEvent(
                CookingEventType.ObjectRotated,
                gameObject
            );
        }
        else
        {
            // =================================================
            // BOLEH MEMINTA ROTATE BERIKUTNYA
            // =================================================

            needRotateEventSent = false;
        }
    }


    // =========================================================
    // UPDATE COOKING VISUAL
    // =========================================================

    private void UpdateCookingVisual()
    {
        if (cookingVisualMode ==
            CookingVisualMode.Material)
        {
            UpdateCookingMaterial();
        }
    }


    // =========================================================
    // UPDATE COOKING MATERIAL
    // =========================================================

    private void UpdateCookingMaterial()
    {
        if (ingredientRenderer == null)
            return;

        if (materialMentah == null)
            return;

        if (materialMatang == null)
            return;


        float blend =
            Mathf.Clamp01(
                cookingProgress
            );


        // =====================================================
        // MATERIAL MENTAH
        // =====================================================

        if (blend <= 0f)
        {
            ingredientRenderer.material =
                materialMentah;

            return;
        }


        // =====================================================
        // MATERIAL MATANG
        // =====================================================

        if (blend >= 1f)
        {
            ingredientRenderer.material =
                materialMatang;

            return;
        }


        // =====================================================
        // BLEND MATERIAL
        // =====================================================

        if (runtimeMaterial == null)
        {
            runtimeMaterial =
                new Material(
                    materialMentah
                );

            ingredientRenderer.material =
                runtimeMaterial;
        }


        runtimeMaterial.Lerp(
            materialMentah,
            materialMatang,
            blend
        );
    }


    // =========================================================
    // SET COOKED GAMEOBJECT
    // =========================================================

    private void SetCookedGameObject()
    {
        // =====================================================
        // MATIKAN OBJECT MENTAH
        // =====================================================

        if (objectMentah != null)
        {
            objectMentah.SetActive(false);
        }


        // =====================================================
        // AKTIFKAN OBJECT MATANG
        // =====================================================

        if (objectMatang != null)
        {
            objectMatang.SetActive(true);
        }
    }


    // =========================================================
    // START COOKING
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
public virtual void StartCooking()
{
    if (isCooked)
        return;

    isCooking = true;

    Debug.Log(
        ingredientName +
        " MULAI MEMASAK"
    );

    // =====================================================
    // PROGRESS UI
    // =====================================================

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


        Debug.Log(
            ingredientName +
            " BERHENTI MEMASAK"
        );
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


        Debug.Log(
            ingredientName +
            " SUDAH DIBALIK"
        );
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

    Debug.Log(
        ingredientName +
        " SUDAH DIADUK"
    );

    FinishCooking();
}


    // =========================================================
    // FINISH COOKING
    // =========================================================

    private void FinishCooking()
    {
        if (isCooked)
            return;


        // =====================================================
        // CEK FLIP
        // =====================================================

        if (requiresFlip && !isFlipped)
        {
            return;
        }


        // =====================================================
        // CEK STIR
        // =====================================================

        if (requiresStir && !isStirred)
        {
            return;
        }


        // =====================================================
        // CEK ROTATE
        // =====================================================

        if (requiresRotate && !isRotated)
        {
            return;
        }


        // =====================================================
        // SET MATANG
        // =====================================================

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
        // =====================================================
        // UPDATE VISUAL
        // =====================================================

        if (cookingVisualMode ==
            CookingVisualMode.Material)
        {
            UpdateCookingMaterial();
        }
        else
        {
            SetCookedGameObject();
        }


        // =====================================================
        // DEBUG
        // =====================================================

        Debug.Log(
            ingredientName +
            " SUDAH MATANG!"
        );


        // =====================================================
        // EVENT
        // =====================================================

        CookingManager.Instance.CheckEvent(
            CookingEventType.ObjectCooked,
            gameObject
        );
    }


    // =========================================================
    // RESET
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


        // =====================================================
        // RESET VISUAL
        // =====================================================

        if (cookingVisualMode ==
            CookingVisualMode.Material)
        {
            UpdateCookingMaterial();
        }
        else
        {
            SetupGameObjects();
        }
    }

    public void SetRigIngredient()
    {
        if (wadah != null)
        {
            wadah.SetActive(false);
        }
        if (need_gravity.Length > 1)
        {
            for (int i = 0; i < need_gravity.Length; i++)
            {
                need_gravity[i].GetComponent<Rigidbody>().useGravity = true;
                need_gravity[i].GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

   
}

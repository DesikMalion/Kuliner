using UnityEngine;

public class CookingStationController : MonoBehaviour
{
    private CookingStation cookingStation;

    [Header("Material")]
    public Material material_panas;

    [Header("Particle Panas")]
    public ParticleSystem particlePanas;

    [Header("Heat")]
    [Range(0f, 1f)]
    public float heatLevel = 1f;

    [Header("Cooling")]
    public float coolingSpeed = 0.2f;

    [Header("Heating")]
    public float heatingSpeed = 0.2f;

    private bool isHeating;

    public float max_value_material;
    public bool IsCooled
    {
        get
        {
            return heatLevel <= 0f;
        }
    }
public enum TransformMode
{
    None,
    Position,
    Rotation
}

[Header("Transform Toggle")]
public TransformMode transformMode = TransformMode.None;

public Transform targetTransform;

[Header("ON Position / Rotation")]
public Vector3 onPosition;
public Vector3 onRotation;

private Vector3 offPosition;
private Vector3 offRotation;

private bool transformInitialized = false;

private void ToggleTransform()
{
    if (targetTransform == null)
        return;

    if (!transformInitialized)
    {
        offPosition = targetTransform.localPosition;
        offRotation = targetTransform.localEulerAngles;

        transformInitialized = true;
    }


    // =====================================================
    // POSITION
    // =====================================================

    if (transformMode == TransformMode.Position)
    {
        if (cookingStation.isHot)
        {
            // ON → masuk
            targetTransform.localPosition = onPosition;
        }
        else
        {
            // OFF → kembali ke posisi awal
            targetTransform.localPosition = offPosition;
        }
    }


    // =====================================================
    // ROTATION
    // =====================================================

    else if (transformMode == TransformMode.Rotation)
    {
        if (cookingStation.isHot)
        {
            // ON → rotasi target
            targetTransform.localEulerAngles = onRotation;
        }
        else
        {
            // OFF → kembali ke rotasi awal
            targetTransform.localEulerAngles = offRotation;
        }
    }
}

   private void Awake()
{
    cookingStation =
        GetComponent<CookingStation>();

    // Pastikan particle mulai dari 0
    SetParticleEmission(0f);

    // Simpan kondisi awal sebagai OFF
    if (targetTransform != null)
    {
        offPosition = targetTransform.localPosition;
        offRotation = targetTransform.localEulerAngles;

        transformInitialized = true;
    }
    Color alpha =
                    material_panas.color;

                alpha.a = 0;

                material_panas.color =alpha;
}


    private void Update()
    {
        if (cookingStation == null)
            return;


        // =====================================================
        // HEATING BERTAHAP
        // =====================================================

        if (isHeating)
        {
            heatLevel +=
                Time.deltaTime * heatingSpeed;

            heatLevel =
                Mathf.Clamp01(heatLevel);


            // =================================================
            // MATERIAL PANAS
            // =================================================

            if (material_panas != null)
            {
                Color alpha =
                    material_panas.color;

                alpha.a =
                    heatLevel * max_value_material;

                material_panas.color =
                    alpha;
            }
if (CookingProgressUI.Instance != null) { CookingProgressUI.Instance.SetProgress( gameObject, heatLevel ); }

            // =================================================
            // PARTICLE PANAS
            // 0 → 10
            // =================================================

            SetParticleEmission(
                heatLevel
            );


            // =================================================
            // SUDAH PANAS
            // =================================================

            if (heatLevel >= 1f)
            {
                heatLevel = 1f;

                isHeating = false;
if (CookingProgressUI.Instance != null) { CookingProgressUI.Instance.ShowComplete( gameObject, "Pemanasan Selesai", gameObject.name + " sudah panas." ); }
                SetParticleEmission(1f);

                CookingManager.Instance.CheckEvent(
                    CookingEventType.StationTurnedOn,
                    gameObject
                );

                Debug.Log(
                    gameObject.name +
                    " : SUDAH PANAS"
                );
            }

            return;
        }


        // =====================================================
        // STATION MASIH ON
        // =====================================================

        if (cookingStation.isHot)
            return;


        // =====================================================
        // COOLING
        // =====================================================

        if (IsCooled)
            return;


        heatLevel -=
            Time.deltaTime * coolingSpeed;

        heatLevel =
            Mathf.Clamp01(heatLevel);


        // =====================================================
        // MATERIAL PANAS
        // =====================================================

        if (material_panas != null)
        {
            Color alpha =
                material_panas.color;

            alpha.a =
                heatLevel * max_value_material;

            material_panas.color =
                alpha;
        }


        // =====================================================
        // PARTICLE PANAS
        // 10 → 0
        // =====================================================

        SetParticleEmission(
            heatLevel
        );

if (CookingProgressUI.Instance != null)
{
    float coolingProgress =
        1f - heatLevel;

    CookingProgressUI.Instance.SetProgress(
        gameObject,
        coolingProgress
    );
}
        // =====================================================
        // SELESAI COOLING
        // =====================================================

        if (heatLevel <= 0f)
        {
            heatLevel = 0f;

            SetParticleEmission(0f);

            CoolDownFinished();
        }
    }


    // =========================================================
    // TURN ON
    // =========================================================

    public void TurnOn()
    {
        if (cookingStation == null)
            return;

        if (cookingStation.isHot)
            return;


        cookingStation.isHot = true;

        // Mulai heating dari kondisi sekarang
        isHeating = true;
if (CookingProgressUI.Instance != null) { CookingProgressUI.Instance.Show( gameObject, "Memanaskan " + gameObject.name ); CookingProgressUI.Instance.SetProgress( gameObject, heatLevel ); } else { Debug.LogError( "CookingProgressUI.Instance NULL!" ); }

        Debug.Log(
            gameObject.name +
            " : ON"
        );
    }


    // =========================================================
    // TURN OFF
    // =========================================================

    public void TurnOff()
    {
        if (cookingStation == null)
            return;

        if (!cookingStation.isHot)
            return;


        cookingStation.isHot = false;

        isHeating = false;


        Debug.Log(
            gameObject.name +
            " : OFF"
        );
if (CookingProgressUI.Instance != null) { CookingProgressUI.Instance.Show( gameObject, "Mendinginkan " + gameObject.name ); CookingProgressUI.Instance.SetProgress( gameObject, heatLevel ); }

        CookingManager.Instance.CheckEvent(
            CookingEventType.StationTurnedOff,
            gameObject
        );
    }
public void Toggle()
{
    if (cookingStation == null)
        return;

    if (cookingStation.isHot)
    {
        TurnOff();
    }
    else
    {
        TurnOn();
    }

     ToggleTransform();
     targetTransform.GetComponent<Collider>().enabled = false;
}

    // =========================================================
    // PARTICLE EMISSION
    // =========================================================

    private void SetParticleEmission(float normalizedHeat)
    {
        if (particlePanas == null)
            return;


        normalizedHeat =
            Mathf.Clamp01(normalizedHeat);


        var emission =
            particlePanas.emission;


        // 0 → 10
        float rate =
            normalizedHeat * 10f;


        emission.rateOverTime =
            rate;
    }


    // =========================================================
    // COOLING FINISHED
    // =========================================================

    private void CoolDownFinished()
    {
        Debug.Log(
            gameObject.name +
            " : SUDAH DINGIN"
        );

if (CookingProgressUI.Instance != null) { CookingProgressUI.Instance.ShowComplete( gameObject, "Pendinginan Selesai", gameObject.name + " sudah dingin." ); }
        CookingManager.Instance.CheckEvent(
            CookingEventType.StationCooled,
            gameObject
        );
    }
}
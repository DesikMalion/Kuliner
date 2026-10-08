
using UnityEngine;
using System.Collections;

public class StoveController : MonoBehaviour
{
    private CookingStation cookingStation;


    // =========================================================
    // STOVE
    // =========================================================

    [Header("Stove")]
    public bool stoveOn = false;


    [Header("Stove Button Rotation")]
    public Transform stoveButton;

    public Vector3 stoveRotationAxis =
        new Vector3(0f, 0f, 1f);

    public float stoveRotationAngle = 45f;

    public float stoveRotationDuration = 0.3f;


    // =========================================================
    // FIRE
    // =========================================================

    [Header("Fire")]
    public bool fireOn = false;


    [Header("Fire Particle")]
    public ParticleSystem fireParticle;

    public GameObject fireObject;
[Header("Heat Complete Particle")]
public ParticleSystem heatCompleteParticle;

    [Header("Fire Button Rotation")]
    public Transform fireButton;

    public Vector3 fireRotationAxis =
        new Vector3(0f, 0f, 1f);

    public float fireRotationAngle = 45f;

    public float fireRotationDuration = 0.3f;


    // =========================================================
    // HEAT
    // =========================================================
[Header("Heat particle")]
public GameObject asap;
public GameObject bubble;
    [Header("Heat")]
    [Range(0f, 1f)]
    public float heatLevel = 0f;

    public float heatingSpeed = 0.2f;

    public float coolingSpeed = 0.2f;


    // =========================================================
    // ROTATION STATE
    // =========================================================

    private Quaternion stoveButtonStartRotation;
    private Quaternion fireButtonStartRotation;

    private bool isRotatingStove;
    private bool isRotatingFire;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        cookingStation =
            GetComponent<CookingStation>();


        // =====================================================
        // SIMPAN ROTASI AWAL STOVE BUTTON
        // =====================================================

        if (stoveButton != null)
        {
            stoveButtonStartRotation =
                stoveButton.localRotation;
        }


        // =====================================================
        // SIMPAN ROTASI AWAL FIRE BUTTON
        // =====================================================

        if (fireButton != null)
        {
            fireButtonStartRotation =
                fireButton.localRotation;
        }


        // =====================================================
        // KONDISI AWAL
        // =====================================================

        stoveOn = false;
        fireOn = false;

        heatLevel = 0f;

        SetFire(false);
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (cookingStation == null)
            return;


        // =====================================================
        // API MENYALA
        // =====================================================

        if (fireOn)
        {
            heatLevel +=
                Time.deltaTime *
                heatingSpeed;

            heatLevel =
                Mathf.Clamp01(
                    heatLevel
                );

                if (CookingProgressUI.Instance != null)
{
    CookingProgressUI.Instance.SetProgress(
        gameObject,
        heatLevel
    );

    
}


            if (heatLevel >= 1)
            {
                if (CookingProgressUI.Instance != null)
{
    CookingProgressUI.Instance.ShowComplete(
        gameObject,
        "Pemanasan Selesai",
        gameObject.name + " sudah panas."
    );

    bubble.SetActive(true);
    asap.SetActive(true);
}
 if (heatCompleteParticle != null &&
            !heatCompleteParticle.isPlaying)
        {
            heatCompleteParticle.Play();
        }
            }

        }


        // =====================================================
        // API MATI
        // =====================================================

        else
        {
            if (heatLevel > 0f)
            {
                heatLevel -=
                    Time.deltaTime *
                    coolingSpeed;

                heatLevel =
                    Mathf.Clamp01(
                        heatLevel
                    );

                    float coolingProgress =
    1f - heatLevel;

if (CookingProgressUI.Instance != null)
{
    CookingProgressUI.Instance.SetProgress(
        gameObject,
        coolingProgress
    );
}

if (heatLevel <= 0f)
                {
                    if (CookingProgressUI.Instance != null)
{
    CookingProgressUI.Instance.ShowComplete(
        gameObject,
        "Pendinginan Selesai",
        gameObject.name + " sudah dingin."
    );
    bubble.SetActive(false);
    asap.SetActive(false);
}
if (heatCompleteParticle != null)
    {
        heatCompleteParticle.Stop();
    }
                }
            }
        }


        // =====================================================
        // STATION HOT MENGIKUTI API
        // =====================================================

        cookingStation.isHot =
            fireOn;
    }


    // =========================================================
    // STOVE ON / OFF
    // =========================================================

    public void ToggleStove()
    {
        if (stoveOn)
        {
            TurnStoveOff();
        }
        else
        {
            TurnStoveOn();
        }
    }


    // =========================================================
    // STOVE ON
    // =========================================================

    public void TurnStoveOn()
    {
        if (stoveOn)
            return;


        stoveOn = true;


        // Stove ON button berputar
        RotateStoveButton(true);
CookingManager.Instance.CheckEvent(
        CookingEventType.StoveOn,
        gameObject
    );
CookingManager.Instance.NextActiveObject();
        Debug.Log(
            gameObject.name +
            " : STOVE ON"
        );
    }


    // =========================================================
    // STOVE OFF
    // =========================================================

    public void TurnStoveOff()
    {
        if (!stoveOn)
            return;


        stoveOn = false;


        // Kalau stove dimatikan,
        // api juga ikut mati.
        if (fireOn)
        {
            fireOn = false;

            SetFire(false);

            RotateFireButton(false);
        }


        // Stove ON button kembali
        RotateStoveButton(false);
if (CookingProgressUI.Instance != null)
{
    CookingProgressUI.Instance.Show(
        gameObject,
        "Mendinginkan " + gameObject.name
    );

    CookingProgressUI.Instance.SetProgress(
        gameObject,
        1f - heatLevel
    );
}

    CookingManager.Instance.CheckEvent(
        CookingEventType.StoveOff,
        gameObject
    );

        Debug.Log(
            gameObject.name +
            " : STOVE OFF"
        );
    }


    // =========================================================
    // FIRE ON / OFF
    // =========================================================

    public void ToggleFire()
    {
        if (!stoveOn)
        {
            Debug.Log(
                "Stove harus ON terlebih dahulu."
            );

            return;
        }


        if (fireOn)
        {
            TurnFireOff();
        }
        else
        {
            TurnFireOn();
        }
    }


    // =========================================================
    // FIRE ON
    // =========================================================

public void TurnFireOn()
{
    if (!stoveOn)
        return;

    if (fireOn)
        return;

    fireOn = true;

    SetFire(true);

    // Fire button berputar
    RotateFireButton(true);

if (CookingProgressUI.Instance != null)
{
    CookingProgressUI.Instance.Show(
        gameObject,
        "Memanaskan " + gameObject.name
    );

    CookingProgressUI.Instance.SetProgress(
        gameObject,
        heatLevel
    );
}
    // =====================================================
    // EVENT COOKING
    // =====================================================

    CookingManager.Instance.CheckEvent(
        CookingEventType.FireOn,
        gameObject
    );


    Debug.Log(
        gameObject.name +
        " : FIRE ON"
    );
}




    // =========================================================
    // FIRE OFF
    // =========================================================

    public void TurnFireOff()
    {
        if (!fireOn)
            return;


        fireOn = false;


        SetFire(false);
CookingManager.Instance.NextActiveObject();


        // Fire button kembali
        RotateFireButton(false);

   CookingManager.Instance.CheckEvent(
        CookingEventType.FireOff,
        gameObject
    );


        Debug.Log(
            gameObject.name +
            " : FIRE OFF"
        );
    }


    // =========================================================
    // STOVE BUTTON ROTATION
    // =========================================================

    private void RotateStoveButton(
        bool turnOn
    )
    {
        if (stoveButton == null)
            return;


        Quaternion targetRotation;


        if (turnOn)
        {
            targetRotation =
                stoveButtonStartRotation *
                Quaternion.AngleAxis(
                    stoveRotationAngle,
                    stoveRotationAxis.normalized
                );
        }
        else
        {
            targetRotation =
                stoveButtonStartRotation;
        }


        StartCoroutine(
            RotateStoveRoutine(
                targetRotation
            )
        );
    }


    // =========================================================
    // STOVE ROTATION ROUTINE
    // =========================================================

    private IEnumerator RotateStoveRoutine(
        Quaternion targetRotation
    )
    {
        if (isRotatingStove)
            yield break;


        isRotatingStove = true;


        Quaternion startRotation =
            stoveButton.localRotation;


        float elapsed = 0f;


        while (elapsed < stoveRotationDuration)
        {
            elapsed +=
                Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    stoveRotationDuration
                );


            t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );


            stoveButton.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );


            yield return null;
        }


        stoveButton.localRotation =
            targetRotation;


        isRotatingStove = false;
    }


    // =========================================================
    // FIRE BUTTON ROTATION
    // =========================================================

    private void RotateFireButton(
        bool turnOn
    )
    {
        if (fireButton == null)
            return;


        Quaternion targetRotation;


        if (turnOn)
        {
            targetRotation =
                fireButtonStartRotation *
                Quaternion.AngleAxis(
                    fireRotationAngle,
                    fireRotationAxis.normalized
                );
        }
        else
        {
            targetRotation =
                fireButtonStartRotation;
        }


        StartCoroutine(
            RotateFireRoutine(
                targetRotation
            )
        );
    }


    // =========================================================
    // FIRE ROTATION ROUTINE
    // =========================================================

    private IEnumerator RotateFireRoutine(
        Quaternion targetRotation
    )
    {
        if (isRotatingFire)
            yield break;


        isRotatingFire = true;


        Quaternion startRotation =
            fireButton.localRotation;


        float elapsed = 0f;


        while (elapsed < fireRotationDuration)
        {
            elapsed +=
                Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    fireRotationDuration
                );


            t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );


            fireButton.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );


            yield return null;
        }


        fireButton.localRotation =
            targetRotation;


        isRotatingFire = false;
    }


    // =========================================================
    // FIRE PARTICLE
    // =========================================================

    private void SetFire(bool active)
    {
        if (fireObject != null)
        {
            fireObject.SetActive(active);
        }


        if (fireParticle != null)
        {
            if (active)
            {
                fireParticle.Play();
            }
            else
            {
                fireParticle.Stop();
            }
        }
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetStove()
    {
        StopAllCoroutines();


        stoveOn = false;
        fireOn = false;

        heatLevel = 0f;


        isRotatingStove = false;
        isRotatingFire = false;


        if (cookingStation != null)
        {
            cookingStation.isHot = false;
        }


        // Kembalikan Stove Button
        if (stoveButton != null)
        {
            stoveButton.localRotation =
                stoveButtonStartRotation;
        }


        // Kembalikan Fire Button
        if (fireButton != null)
        {
            fireButton.localRotation =
                fireButtonStartRotation;
        }


        SetFire(false);
    }
}


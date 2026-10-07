using UnityEngine;
using UnityEngine.Events;

public class FireExtinguisherSpray : MonoBehaviour
{
    public UnityEvent OnExtinguished;
    public UnityEvent OnSpray;

    public bool isFireExtinguished = true;
    [Header("Raycast")]
    [SerializeField] private float sprayDistance = 5f;
    [SerializeField] private LayerMask fireLayer;

    [Header("Spray")]
    [SerializeField] private bool spraying = false;

    [Header("Debug")]
    [SerializeField] private bool showRay = true;

    [SerializeField] private float extinguishTime = 4f;
    [SerializeField] private float maxScale = 1f;

    private float extinguishProgress;
    private Vector3 initialScale = Vector3.one;
    //private bool extinguished;
    GameObject fireObject = null;
    GameObject sprayObject = null;

    private void Awake()
    {

    }

    private void Update()
    {
        if (!spraying)
        {
            fireObject = null;
            return;
        }

        if (isFireExtinguished)
        {
            DetectFire();
        }
        else {
            Spraying();
        }

    }

    private void DetectFire()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            sprayDistance,
            fireLayer))
        {

           GameObject gameObject = hit.collider.gameObject;
            if (gameObject != fireObject)
            {
                fireObject = gameObject;
                initialScale = fireObject.transform.parent.localScale;
                extinguishProgress = 0f;
                extinguishTime = 4f;
                
            }

            Extinguish(Time.deltaTime);

        }

       // currentFire = null;
    }

    void Spraying()
    {

        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            sprayDistance,
            fireLayer))
        {
            sprayObject = hit.collider.transform.parent.gameObject;
            OnSpray.Invoke();

        }
        else { 
        if (sprayObject != null)
            {
                sprayObject = null;
            }

        }

    }

    public GameObject GetSprayObject()
    {
        return sprayObject;
    }

    public void StartSpray()
    {
        spraying = true;
    }

    public void StopSpray()
    {
        spraying = false;
        //currentFire = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showRay)
            return;

        Gizmos.color = Color.cyan;

        Gizmos.DrawRay(
            transform.position,
            transform.forward * sprayDistance
        );
    }

    public void Extinguish(float deltaTime)
    {
       // if (extinguished)
        //    return;

        extinguishProgress += deltaTime;

        float progress = Mathf.Clamp01(extinguishProgress / extinguishTime);

        // Api mengecil
        fireObject.transform.parent.localScale = initialScale * (1f - progress);

        if (fireObject.transform.parent.localScale.x<=0.5f)
        {
            Extinguished();
        }
    }

    public void StopExtinguishing()
    {
        // Kalau ingin progress tetap tersimpan,
        // tidak perlu melakukan apa-apa di sini.
    }

    private void Extinguished()
    {
      // extinguished = true;

        fireObject.transform.parent.localScale = Vector3.zero;

        Debug.Log("Api berhasil dipadamkan!");
        OnExtinguished.Invoke();

        // Kalau nanti ada Particle System,
        // bisa dimatikan di sini.
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stream : MonoBehaviour
{
    private LineRenderer lineRenderer = null;
    private ParticleSystem splashParticle = null;

    private Coroutine pourRoutine = null;
    private Vector3 targetPosition = Vector3.zero;

    public GameObject HitObj;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        splashParticle = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);
    }

    public void Begin()
    {
        //StartCoroutine(UpdateParticle());
        pourRoutine = StartCoroutine(BeginPour());
    }

    private IEnumerator BeginPour()
    {
        while (gameObject.activeSelf)
        {
            targetPosition = FindEndPoint();

            MoveToPosition(0, transform.position);
            AnimateToPosition(1, targetPosition);

            splashParticle.transform.position = targetPosition;

            if (!splashParticle.gameObject.activeSelf)
            splashParticle.gameObject.SetActive(true);

            yield return null;
        }
    }

    private Vector3 FindEndPoint()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        Physics.Raycast(ray, out hit, 5.0f);

        Vector3 endPoint = hit.collider
            ? hit.point
            : ray.GetPoint(2.0f);

        if (HitObj == null)
        {
            HitObj = hit.collider.gameObject;
        }
        else if (hit.collider.gameObject != HitObj) {

            HitObj = hit.collider.gameObject;
        }
            
                return endPoint;
    }

    public void End()
    {
        if (pourRoutine != null)
        {
            StopCoroutine(pourRoutine);
            pourRoutine = StartCoroutine(EndPour());
        }
    }

    IEnumerator EndPour()
    {
        while (!HasReachPosition(0,targetPosition))
        {
            AnimateToPosition(0, targetPosition);
            AnimateToPosition(1, targetPosition);

            if (splashParticle.gameObject.activeSelf)
                splashParticle.gameObject.SetActive(false);

            yield return null;
        }
    }

    private void MoveToPosition(int index, Vector3 targetPosition)
    {
        lineRenderer.SetPosition(index, targetPosition);
    }

    private void AnimateToPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPosition = lineRenderer.GetPosition(index);
        Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * 20f);
        MoveToPosition(index, newPosition);
    }

    bool HasReachPosition(int index, Vector3 targetPos)
    {
                Vector3 currentPosition = lineRenderer.GetPosition(index);
        return currentPosition==targetPos;
    }

    private IEnumerator UpdateParticle()
    {
        while (gameObject.activeSelf)
        {
            splashParticle.transform.position = targetPosition;
            bool isHitting = HasReachPosition(1, targetPosition);
            splashParticle.gameObject.SetActive(isHitting);
            yield return null;
        }
    }
}
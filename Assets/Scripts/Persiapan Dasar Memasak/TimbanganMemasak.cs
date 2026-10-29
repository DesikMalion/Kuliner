using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimbanganMemasak : MonoBehaviour
{

    [Header("Scale Settings")]
    public Transform weighingArea;
    public TMP_Text weightText;

    [Tooltip("Jika aktif, hanya menghitung Rigidbody pada objek yang berada di dalam area timbangan.")]
    public bool useTriggerDetection = true;

    [Header("Display")]
    //public string unit = "g";
    public float massMultiplier = 1000f;
    public int decimalPlaces = 1;

    private float totalMass;
    private float nowMass;

    public Collider[] objWeighingCol;
    public UnityEvent OnUpdateDisplay;

    private void OnTriggerStay(Collider other)
    {
        if (!useTriggerDetection)
            return;

        CalculateWeight();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!useTriggerDetection)
            return;

        CalculateWeight();
    }

    private void Update()
    {
        if (!useTriggerDetection)
            CalculateWeight();
    }

    private void CalculateWeight()
    {
        totalMass = 0f;

        if (weighingArea == null)
            return;

        Collider[] colliders = Physics.OverlapBox(
            weighingArea.position,
            weighingArea.localScale * 0.5f,
            weighingArea.rotation
        );

        objWeighingCol = colliders;

        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;

            if (rb != null)
            {
                totalMass += rb.mass;
            }
        }

        if(totalMass!= nowMass)
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (weightText == null)
            return;

        float displayedMass = totalMass;

        weightText.text = displayedMass.ToString("F" + decimalPlaces);

        nowMass = totalMass;
        OnUpdateDisplay.Invoke();
    }

    public float GetWeightInGrams()
    {
        return totalMass * 1000f;
    }

}

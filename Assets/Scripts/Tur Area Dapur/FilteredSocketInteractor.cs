using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FilteredSocketInteractor : XRSocketInteractor
{
    [Header("Filtering Settings")]
    [Tooltip("Tuliskan Tag dari pisau yang diizinkan masuk ke lubang ini.")]
    public string targetTag = "Untagged";

    // Fungsi ini mengecek apakah objek diizinkan untuk sekadar menempel/highlight di socket
    public override bool CanHover(IXRHoverInteractable interactable)
    {
        // Akan mengembalikan nilai True HANYA JIKA interaksi bawaan valid DAN tag objek sesuai
        return base.CanHover(interactable) && interactable.transform.CompareTag(targetTag);
    }

    // Fungsi ini mengecek apakah objek benar-benar diizinkan masuk/snap ke dalam socket
    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && interactable.transform.CompareTag(targetTag);
    }
}
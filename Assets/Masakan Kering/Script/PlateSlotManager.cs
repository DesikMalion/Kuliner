
using UnityEngine;

public class PlateSlotManager : MonoBehaviour
{
    [Header("Urutan Slot Piring")]
    public PlateSlot[] slots;

    private bool[] occupied;

    private void Awake()
    {
        occupied = new bool[slots.Length];
    }

    public PlateSlot GetNextAvailableSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (occupied[i])
                continue;

            if (slots[i] == null)
                continue;

            return slots[i];
        }

        return null;
    }

    public bool TryOccupySlot(PlateSlot slot)
    {
        if (slot == null)
            return false;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != slot)
                continue;

            if (occupied[i])
                return false;

            occupied[i] = true;
            return true;
        }

        return false;
    }

    public void ResetSlots()
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            occupied[i] = false;
        }
    }
}

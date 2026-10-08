using UnityEngine;

public class CookingEvent : MonoBehaviour
{
    public static CookingEvent Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Debug.Log("INI MANA");
    }

    public void Trigger(CookingEventType eventType, GameObject target)
    {
        CookingManager.Instance.CheckEvent(eventType, target);
    }
}

public enum CookingEventType
{
    WashHand,

    FishPick,
    GrillPick,
    PenjepitPick,
    PlatePick,

    ObjectSeasoned,
    ObjectPrepared,

    // =====================================================
    // GENERIC COOKING EVENTS
    // =====================================================

    ObjectPickedUp,
    ObjectPlaced,
    ObjectFlipped,
    ObjectNeedFlipped,
    ObjectStirred,
    ObjectCooked,

    ObjectPlacedOnPlate,

    // =====================================================
    // STATION EVENTS
    // =====================================================

    StationTurnedOn,
    StationTurnedOff,
    StationCooled,
    StationCleaned,


    // =====================================================
    // OLD EVENTS
    // Jangan dihapus dulu.
    // Kita hapus setelah semua sistem selesai.
    // =====================================================

    FishPlacedOnPlate,
    FishPickedUp,
    FishPlacedOnGrill,
    FishFlipped,
    FishCooked,

    GrillTurnedOn,
    GrillTurnedOff,
    GrillCooled,

    GrillCleaned,

    ObjectNeedStirred,

    ObjectNeedRotated,
    ObjectRotated,

    OvenDoorOpened,
    OvenDoorClosed,
    UseGlove,

    FireOn,
    FireOff,
    StoveOn,
    StoveOff
}
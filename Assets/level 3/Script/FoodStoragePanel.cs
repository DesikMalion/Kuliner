using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Tambahkan ke objek Chiller, Freezer, atau Gudang Kering.
// Panel dibuat otomatis saat Play; skrip lama tidak perlu diubah.
[DisallowMultipleComponent]
public class FoodStoragePanel : MonoBehaviour
{
    [Header("Referensi")]
    public FoodStorageArea storageArea;
    public FoodStorageManager manager;

    [Header("Posisi relatif terhadap objek ini")]
    public Vector3 panelLocalPosition = new Vector3(0f, 2f, 0f);
    public Vector3 panelLocalEulerAngles = Vector3.zero;
    public Vector2 panelSize = new Vector2(720f, 600f);
    public float panelScale = 0.002f;
public Sprite panelSprite;
public Color panelTint = Color.white;
    [Header("Tampilan")]
    public string titleOverride = "";
    public TMP_FontAsset font;
    [Tooltip("Opsional: drag kamera XR jika panel perlu mengikuti pandangan.")]
    public Transform viewer;
    public bool faceViewer;

    private RectTransform panel;
    private TMP_Text label;
    private float nextRefresh;
    private string lastText;
    private readonly List<FoodIngredient> stock = new List<FoodIngredient>();
    private readonly StringBuilder text = new StringBuilder();

    private void Awake()
    {
        if (storageArea == null)
            storageArea = GetComponentInParent<FoodStorageArea>();
        if (manager == null && storageArea != null)
            manager = storageArea.manager;
        BuildPanel();
    }

    private void OnEnable()
    {
        nextRefresh = 0f;
        if (panel != null) panel.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (panel != null) panel.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (panel != null) Destroy(panel.gameObject);
    }

    private void LateUpdate()
    {
        if (faceViewer && viewer != null && panel != null)
        {
            Vector3 direction = panel.position - viewer.position;
            if (direction.sqrMagnitude > 0.0001f)
                panel.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        if (Time.unscaledTime < nextRefresh) return;
        nextRefresh = Time.unscaledTime + 0.2f;
        RefreshPanel();
    }

    private void BuildPanel()
    {
        GameObject root = new GameObject("Storage Info Panel", typeof(RectTransform), typeof(Canvas), typeof(Image));
        panel = root.GetComponent<RectTransform>();
        panel.SetParent(transform, false);
        panel.localPosition = panelLocalPosition;
        panel.localRotation = Quaternion.Euler(panelLocalEulerAngles);
        panel.localScale = Vector3.one * Mathf.Max(0.000001f, panelScale);
        panel.sizeDelta = panelSize;
        root.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
       Image background = root.GetComponent<Image>();

if (panelSprite != null)
{
    background.sprite = panelSprite;
    background.color = panelTint;
    background.type = Image.Type.Simple;
}
else
{
    background.color = new Color(0.035f, 0.075f, 0.12f, 0.96f);
}

background.raycastTarget = false;

        GameObject content = new GameObject("Stock List", typeof(RectTransform), typeof(TextMeshProUGUI));
        RectTransform rect = content.GetComponent<RectTransform>();
        rect.SetParent(panel, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(30f, 24f);
        rect.offsetMax = new Vector2(-30f, -24f);
        label = content.GetComponent<TextMeshProUGUI>();
        if (font != null) label.font = font;
        label.fontSize = 25f;
        label.enableAutoSizing = true;
        label.fontSizeMin = 12f;
        label.fontSizeMax = 25f;
        label.color = Color.white;
        label.richText = true;
        label.alignment = TextAlignmentOptions.TopLeft;
        label.raycastTarget = false;
    }

    public void RefreshPanel()
    {
        if (label == null) return;
        if (storageArea == null)
        {
            SetText("Hubungkan Storage Area di Inspector.");
            return;
        }
        if (manager == null) manager = storageArea.manager;
        if (manager == null)
        {
            SetText("Hubungkan Food Storage Manager di Inspector.");
            return;
        }

        FoodIngredient.StorageType type;
        string title;
        switch (storageArea.areaType)
        {
            case FoodStorageArea.StorageAreaType.Chilling:
                type = FoodIngredient.StorageType.Chilling;
                title = "CHILLER";
                break;
            case FoodStorageArea.StorageAreaType.Freezing:
                type = FoodIngredient.StorageType.Freezing;
                title = "FREEZER";
                break;
            default:
                type = FoodIngredient.StorageType.GudangKering;
                title = "GUDANG KERING";
                break;
        }
        if (!string.IsNullOrWhiteSpace(titleOverride)) title = titleOverride;

        stock.Clear();
        if (manager.ingredients != null)
        {
            foreach (FoodIngredient item in manager.ingredients)
            {
                // Sama dengan lingkup FIFO manager: berdasarkan jenis storage.
                if (item != null && item.isStored && !item.isTaken && item.requiredStorage == type)
                    stock.Add(item);
            }
        }
        stock.Sort(CompareStock);
        bool ready = manager.AllIngredientsStored();
        text.Clear();
        text.Append("<b><size=125%>").Append(Safe(title)).Append("</size></b>\n");
        text.Append(manager.rotationMode).Append("  |  Stok: ").Append(stock.Count).Append(" bahan\n\n");
        if (stock.Count == 0)
        {
            text.Append("Belum ada bahan tersimpan.");
        }
        else
        {
            text.Append("<b>URUTAN PENGAMBILAN</b>\n");
            for (int i = 0; i < stock.Count; i++)
            {
                FoodIngredient item = stock[i];
                bool canTake = manager.CanTakeIngredient(item, false);
                if (canTake) text.Append("<color=#70EDBC><b>");
                text.Append(i + 1).Append(". ").Append(Safe(item.ingredientName));
                if (canTake) text.Append("  [AMBIL BERIKUTNYA]");
                if (canTake) text.Append("</b></color>");
                if (manager.rotationMode == FoodStorageManager.RotationMode.FEFO)
                    text.Append("\n    Kedaluwarsa: hari ").Append(item.expiryDay);
                text.Append('\n');
            }
        }
        text.Append("\n<size=85%>");
        text.Append(ready ? "Ambil sesuai urutan pada tiap jenis penyimpanan." : "Simpan semua bahan layak pakai dahulu.");
        text.Append("</size>");
        SetText(text.ToString());
    }

    private int CompareStock(FoodIngredient a, FoodIngredient b)
    {
        if (manager.rotationMode == FoodStorageManager.RotationMode.FEFO)
        {
            int expiry = a.expiryDay.CompareTo(b.expiryDay);
            if (expiry != 0) return expiry;
        }
        return a.arrivalOrder.CompareTo(b.arrivalOrder);
    }

    private static string Safe(string value)
    {
        return string.IsNullOrEmpty(value) ? "Tanpa nama" : value.Replace("<", "\u2039").Replace(">", "\u203a");
    }

    private void SetText(string value)
    {
        if (lastText == value) return;
        lastText = value;
        label.text = value;
    }
}

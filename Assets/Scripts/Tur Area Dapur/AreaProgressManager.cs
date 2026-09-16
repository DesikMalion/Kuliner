using UnityEngine;
using UnityEngine.Events;

public class AreaProgressManager : MonoBehaviour
{
    [Header("Pengaturan UI")]
    public GameObject uiPanel;
    public AreaSequenceManager uiSequence;

    [Header("Pengaturan Target Buka Kunci")]
    [Tooltip("Target index awal. Biarkan 1 untuk membuka Penyimpanan pertama kali.")]
    public int indexAreaSelanjutnya = 1;

    [Header("Progress Area Penerimaan")]
    public int totalTugasPenerimaan = 2;
    private int tugasSelesaiPenerimaan = 0;

    public UnityEvent onAreaPenerimaanSelesai;

    public void TambahTugasSelesai()
    {
        tugasSelesaiPenerimaan++;

        if (tugasSelesaiPenerimaan >= totalTugasPenerimaan)
            AreaPenerimaanTuntas();
    }

    public void AreaPenerimaanTuntas()
    {
        // 1. Membuka area sesuai index target saat ini
        if (uiSequence != null)
        {
            uiSequence.UnlockNextArea(indexAreaSelanjutnya);
        }

        // 2. Menjalankan event
        if (onAreaPenerimaanSelesai != null)
        {
            onAreaPenerimaanSelesai.Invoke();
        }

        // --- TAMBAHAN UNTUK 1 GAMEOBJECT ---
        // 3. Reset hitungan agar siap menghitung tugas di area berikutnya
        tugasSelesaiPenerimaan = 0;

        // 4. Naikkan target index secara otomatis (1 -> 2 -> 3 dst)
        indexAreaSelanjutnya++;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
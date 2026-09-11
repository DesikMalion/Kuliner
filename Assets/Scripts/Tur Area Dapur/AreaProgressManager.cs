using UnityEngine;
using UnityEngine.Events;

public class AreaProgressManager : MonoBehaviour
{
    [Header("Pengaturan UI")]
    public GameObject uiPanel;
    public AreaSequenceManager uiSequence;

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
        uiSequence.UnlockNextArea(1);
        if (onAreaPenerimaanSelesai != null)
        {
            onAreaPenerimaanSelesai.Invoke();
        }
        //uiPanel.SetActive(true);
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

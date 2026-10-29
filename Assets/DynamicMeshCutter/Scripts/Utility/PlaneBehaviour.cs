
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DynamicMeshCutter
{
    public class PlaneBehaviour : CutterBehaviour
    {
        public bool AutoCut = false;
        public UnityEvent OnCreatedObjects;
        public MeshTarget ObjectsToCut;
        public MeshTarget[] CuttedObjects;
        public float[] CuttedRatio;
        bool isCutting = false;
        //public float DebugPlaneLength = 2;
        public void Cut()
        {
            var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                if (!root.activeInHierarchy)
                    continue;
                var targets = root.GetComponentsInChildren<MeshTarget>();
                foreach (var target in targets)
                {
                    if (!target.IsKnifeCuttable_)
                    {
                        Cut(target, transform.position, transform.forward, null, OnCreated);

                    }
                    else if (target.IsKnifeCuttable_ && target.IsKnifeTouch_)
                    {
                        if (!target.IsCutted)
                        {
                            target.IsCutted = true;

                            Cut(target, target.KnifeObj.transform.position, target.KnifeObj.transform.forward, null, OnCreated);
                            target.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        void OnCreated(Info info, MeshCreationData cData)
        {
            MeshCreation.TranslateCreatedObjects(info, cData.CreatedObjects, cData.CreatedTargets, Separation);
            CuttedObjects = cData.CreatedTargets;
            for (int i = 0; i < cData.CreatedTargets.Length; i++)
            {
                var target = cData.CreatedTargets[i];
                MeshCollider meshCollider = target.GetComponent<MeshCollider>();
                if (meshCollider != null)
                {
                    meshCollider.convex = true;
                    /*
                    meshCollider.isTrigger = true;

                    MeshCollider meshCollider2 =  target.gameObject.AddComponent<MeshCollider>();
                    meshCollider2.convex = true;
                    meshCollider2.isTrigger = false;
                    */
                }
            }

            CompareMeshSizes();
        
            OnCreatedObjects.Invoke();

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Target") && !isCutting && AutoCut)
            //if (other.CompareTag("Target")  && AutoCut)
            {
                Debug.Log("AutoCutting");
                ObjectsToCut = other.GetComponent<MeshTarget>();
                if (ObjectsToCut != null)
                {
                    if (!ObjectsToCut.IsCutted)
                    {
                        StartCoroutine(WaitForNextCut());
                    }
                }
            }
        }

        IEnumerator WaitForNextCut()
        {
            isCutting = true;
            yield return new WaitForSeconds(.1f);
            Cut();
            yield return new WaitForSeconds(.5f);
            isCutting = false;

        }

        private void OnTriggerExit(Collider other)
        {

        }



        [ContextMenu("Compare Mesh Sizes")]
        public void CompareMeshSizes()
        {
            if (ObjectsToCut == null)
            {
                Debug.LogWarning("ObjectsToCut belum diatur.");
                return;
            }

            MeshFilter originalMesh = ObjectsToCut.GetComponent<MeshFilter>();
            Rigidbody originalRb = ObjectsToCut.GetComponent<Rigidbody>();

            if (originalMesh == null || originalMesh.sharedMesh == null ||
                originalRb == null)
            {
                Debug.LogWarning("MeshFilter atau Rigidbody objek asal tidak ditemukan.");
                return;
            }

            if (CuttedObjects == null || CuttedObjects.Length == 0)
                return;

            if (CuttedRatio == null || CuttedRatio.Length != CuttedObjects.Length)
                CuttedRatio = new float[CuttedObjects.Length];

            ObjectsToCut.gameObject.SetActive(false);

            float[] volumes = new float[CuttedObjects.Length];
            float totalCutVolume = 0f;

            // Hitung volume bounding box setiap potongan.
            for (int i = 0; i < CuttedObjects.Length; i++)
            {
                MeshTarget cutObject = CuttedObjects[i];

                if (cutObject == null)
                    continue;

                MeshFilter mesh = cutObject.GetComponent<MeshFilter>();

                if (mesh == null || mesh.sharedMesh == null)
                    continue;

                volumes[i] = GetWorldBoundsVolume(mesh);
                totalCutVolume += volumes[i];
            }

            if (totalCutVolume <= 0f)
            {
                Debug.LogWarning("Total volume potongan adalah nol.");
                return;
            }

            float totalRatio = 0f;
            float totalMass = 0f;

            // Normalisasi rasio dan distribusi massa.
            for (int i = 0; i < CuttedObjects.Length; i++)
            {
                MeshTarget cutObject = CuttedObjects[i];

                if (cutObject == null || volumes[i] <= 0f)
                {
                    CuttedRatio[i] = 0f;
                    continue;
                }

                float ratio = volumes[i] / totalCutVolume;
                CuttedRatio[i] = ratio;

                Rigidbody cutRb = cutObject.GetComponent<Rigidbody>();

                if (cutRb != null)
                {
                    cutRb.mass = originalRb.mass * ratio;
                    totalMass += cutRb.mass;
                }

                totalRatio += ratio;

                Debug.Log(
                    $"{cutObject.name} | " +
                    $"Volume: {volumes[i]:F6} | " +
                    $"Ratio: {ratio:P2} | " +
                    $"Mass: {(cutRb != null ? cutRb.mass : 0f):F6}"
                );
            }

            Debug.Log($"Total Ratio: {totalRatio:P2}");
            Debug.Log($"Total Mass: {totalMass:F6}");
        }

        private float GetWorldBoundsVolume(MeshFilter meshFilter)
        {
            Bounds bounds = meshFilter.sharedMesh.bounds;
            Vector3 scale = meshFilter.transform.lossyScale;

            float x = bounds.size.x * Mathf.Abs(scale.x);
            float y = bounds.size.y * Mathf.Abs(scale.y);
            float z = bounds.size.z * Mathf.Abs(scale.z);

            return x * y * z;
        }

    }
}
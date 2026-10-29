using DynamicMeshCutter;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CutedShapeMgr : MonoBehaviour
{
    
    //public GameObject cutedShapePrefabs;
    public PlaneBehaviour planeBehaviour;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ObjCutted() {

        StartCoroutine(WaitCuttedObjInteract(planeBehaviour.CuttedObjects));

    }

    IEnumerator WaitCuttedObjInteract(MeshTarget[] CuttedObjects)
    {
        yield return new WaitForSeconds(1f);

        //CuttedObjects = planeBehaviour.CuttedObjects;

        for (int i = 0; i < CuttedObjects.Length; i++)
        {
            if (CuttedObjects[i] != null)
            {
                MeshTarget target = CuttedObjects[i];
                GameObject parent_ = target.transform.parent.gameObject;
                CuttedObjects[i].gameObject.name = parent_.name;
                CuttedObjects[i].transform.parent = null;
                parent_.SetActive(false);
                //CuttedObjects[i].gameObject.AddComponent<Rigidbody>();
                XRGrabInteractable XRGrabInteractable_ = CuttedObjects[i].gameObject.AddComponent<XRGrabInteractable>();
                XRGrabInteractable_.attachTransform = CuttedObjects[i].transform;
                //XRGrabInteractable_.predictedVisualsTransform = CuttedObjects[i].transform;
                XRGrabInteractable_.smoothPosition = true;
                XRGrabInteractable_.smoothRotation = true;
                XRGrabInteractable_.smoothScale = true;
                XRGrabInteractable_.useDynamicAttach = true;
            }

        }
    }

}

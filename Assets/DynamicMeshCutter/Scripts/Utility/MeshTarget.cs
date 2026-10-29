using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DynamicMeshCutter
{
    public class MeshTarget : MonoBehaviour
    {
        public bool IsKnifeCuttable_ = true;
        public bool IsKnifeTouch_ = false;
        public bool IsCutted = false;
        public Transform KnifeObj ;

        //basic both
        [Tooltip("This represents the root of the object and the gameobject that will be destroyed if \"DestroyTargets\" is enabled in the cutter. ")]
        public GameObject GameobjectRoot;
        public Material OverrideFaceMaterial;
        public bool SeparateMeshes;
        public bool ApplyTranslation = true;

        //basic single
        public Behaviour[] DefaultBehaviour = new Behaviour[2] { Behaviour.Stone, Behaviour.Stone };

        public DynamicRagdoll DynamicRagdoll;
        public Animator Animator;

        public List<GroupBehaviours> GroupBehaviours = new List<GroupBehaviours>();

        //stone
        public bool[] CreateRigidbody = new bool[2] { true, true };
        public bool[] CreateMeshCollider = new bool[2] { true, true };

        //ragdoll
        public RagdollPhysics[] Physics = new RagdollPhysics[2] { RagdollPhysics.NonKinematic, RagdollPhysics.NonKinematic };

        //inheritance
        public bool[] Inherit = new bool[2] { false, false };

        private MeshRenderer _meshRenderer;
        private SkinnedMeshRenderer _skinnedMeshRenderer;


        private void OnEnable()
        {

            MeshTargetShephard.RegisterMeshTarget(this);

            StartCoroutine(ZeroForce());
        }

        IEnumerator ZeroForce()
        {
            yield return new WaitForSeconds(0.1f);
            //zero all rigidbody velocities and force to avoid ragdoll parts flying away when cut
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
                rb = transform.parent.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
                yield return new WaitForSeconds(0.1f);
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.angularDamping = 0;
                rb.isKinematic = false;
            }

        }

        private void OnDisable()
        {
            MeshTargetShephard.UnRegisterMeshTarget(this);
        }

        public MeshRenderer MeshRenderer
        {
            get
            {
                if (_meshRenderer == null)
                    _meshRenderer = GetComponent<MeshRenderer>();
                return _meshRenderer;
            }
        }
        public SkinnedMeshRenderer SkinnedMeshRenderer
        {
            get
            {
                if (_skinnedMeshRenderer == null)
                    _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
                return _skinnedMeshRenderer;
            }
        }
        public bool IsSkinned => SkinnedMeshRenderer != null ? true : false;
        public Renderer Renderer => IsSkinned ? (Renderer)SkinnedMeshRenderer : (Renderer)MeshRenderer;
        public Material FaceMaterial
        {
            get
            {
                if (OverrideFaceMaterial != null)
                    return OverrideFaceMaterial;
                return TryGetMaterial();

            }
        }
        public bool RequireLocal => GetComponent<SkinnedMeshRenderer>() != null;

        protected virtual void Start()
        {
            if (DynamicRagdoll == null)
                DynamicRagdoll = GetComponentInParent<DynamicRagdoll>();
        }
        public Material TryGetMaterial()
        {
            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
                return renderer.material;
            var sRenderer = GetComponent<SkinnedMeshRenderer>();
            if (sRenderer != null)
                return sRenderer.material;
            return null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PisauSayur") && !IsKnifeTouch_)
            {
                IsKnifeTouch_  = true;
                KnifeObj = other.transform;
                Debug.Log("Knife Touch: " + KnifeObj.name);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("PisauSayur") && IsKnifeTouch_)
            {
                IsKnifeTouch_ = false;
            }
        }
    }
}
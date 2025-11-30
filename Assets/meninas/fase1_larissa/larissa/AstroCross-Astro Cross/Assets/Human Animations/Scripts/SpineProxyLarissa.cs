// -- SPINE PROXY 1.0 | Kevin Iglesias --
// Modified for AstroCross project

using UnityEngine;

namespace KevinIglesias
{
    public class SpineProxyLarissa : MonoBehaviour
    {
        [SerializeField] private Transform originalSpine;
        private Quaternion rotationOffset = Quaternion.identity;

#if UNITY_EDITOR
        // Tenta encontrar o osso da espinha original automaticamente.
        void OnValidate()
        {
            if(originalSpine == null)
            {
                Transform parent = transform.parent;
                if(parent != null)
                {
                    Transform hips = parent.Find("B-hips");
                    if(hips != null)
                    {
                        Transform spine = hips.Find("B-spine");
                        if(spine != null)
                        {
                            originalSpine = spine;
                        }
                    }
                }
            }
        }  
#endif
        
        // Calcula o offset de rotação inicial em relação à espinha original.
        void Awake()
        {
            if(originalSpine != null)
            {
                rotationOffset = Quaternion.Inverse(transform.rotation) * originalSpine.rotation;
            }
        }

        // Aplica a rotação calculada ao osso da espinha original a cada frame.
        void LateUpdate()
        {
            if(originalSpine == null)
            {
                return;
            }
            originalSpine.rotation = transform.rotation * rotationOffset;
        }
    }
}
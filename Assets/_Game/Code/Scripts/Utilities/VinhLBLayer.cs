using UnityEngine;

namespace VinhLB
{
    public static class VinhLBLayer
    {
        public static readonly int Default = LayerMask.NameToLayer("Default");
        public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        public static readonly int UI = LayerMask.NameToLayer("UI");
        public static readonly int Draggable = LayerMask.NameToLayer("Draggable");
        public static readonly int Wall = LayerMask.NameToLayer("Wall");

        public static LayerMask GetLayerMask(int layer)
        {
            return (1 << layer);
        }
        
        public static LayerMask GetPhysicsLayerMask(int layer)
        {
            int layerMask = 0;
            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(layer, i))
                {
                    layerMask |= (1 << i);
                }
            }
            
            return layerMask;
        }
        
        public static LayerMask GetPhysics2DLayerMask(int layer)
        {
            int layerMask = 0;
            for (int i = 0; i < 32; i++)
            {
                if (!Physics2D.GetIgnoreLayerCollision(layer, i))
                {
                    layerMask |= (1 << i);
                }
            }
            
            return layerMask;
        }

        public static bool ContainsLayer(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }
    }
}
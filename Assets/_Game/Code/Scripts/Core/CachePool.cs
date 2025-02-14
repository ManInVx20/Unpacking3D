using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public static class CachePool
    {
        private static Dictionary<GameObject, DraggableItem> _cachedItemDict = new();
        private static Dictionary<GameObject, DraggableSlot> _cachedSlotDict = new();
        
        public static void Clear()
        {
            _cachedItemDict.Clear();
            _cachedSlotDict.Clear();
        }

        public static bool TryGetItem(GameObject gameObject, out DraggableItem item)
        {
            return TryGetComponent(gameObject, out item, _cachedItemDict);
        }
        
        public static bool TryGetItem(Transform transform, out DraggableItem item)
        {
            return TryGetItem(transform.gameObject, out item);
        }
        
        public static bool TryGetSlot(GameObject gameObject, out DraggableSlot slot)
        {
            return TryGetComponent(gameObject, out slot, _cachedSlotDict);
        }
        
        public static bool TryGetSlot(Transform transform, out DraggableSlot slot)
        {
            return TryGetSlot(transform.gameObject, out slot);
        }
        
        private static bool TryGetComponent<T>(GameObject gameObject, out T component, 
            Dictionary<GameObject, T> cachedDict) where T : Component
        {
            if (cachedDict.TryGetValue(gameObject, out component))
            {
                return true;
            }
            
            if (gameObject.TryGetComponent(out component))
            {
                cachedDict[gameObject] = component;
                
                return true;
            }
            
            return false;
        }
    }
}
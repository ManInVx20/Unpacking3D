using UnityEngine;

namespace VinhLB
{
    public class BaseController : MonoBehaviour
    {
        public bool IsActive { get; protected set; }
        public bool InUse { get; protected set; }

        public virtual void SetActive(bool value)
        {
            IsActive = value;
        }

        public static bool IsAnyControllerInUse(BaseController[] controllers)
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i].InUse)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
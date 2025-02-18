using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VinhLB
{
    public class Init : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
        
        private void Start()
        {
            PanelManager.Instance.CreatePanel<InGamePanel>(nameof(InGamePanel), true).Forget();
        }
    }
}
using Cysharp.Threading.Tasks;

namespace VinhLB
{
    public class InGamePanel : Panel
    {
        public void OnPauseButton()
        {
            PanelManager.Instance.CreatePanel<PopupPanel>(nameof(PausePanel), true).Forget();
        }
    }   
}
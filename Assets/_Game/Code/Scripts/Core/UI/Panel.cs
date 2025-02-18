using System;
using DG.Tweening;
using UnityEngine;

namespace VinhLB
{
    public class Panel : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private float _openAnimDuration;
        [SerializeField]
        private float _closeAnimDuration;
        
        public string PanelName { get; private set; }
        public bool CanBack { get; private set; }
        public float OpenAnimDuration => _openAnimDuration;
        public float CloseAnimDuration => _closeAnimDuration;
        
        public void Initialize(string panelName, bool canBack)
        {
            PanelName = panelName;
            CanBack = canBack;
        }
        
        public virtual void Open()
        {
            gameObject.SetActive(true);
            
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1f, _openAnimDuration)
                .OnComplete(() => _canvasGroup.interactable = true);
        }
        
        public virtual void Close()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.DOFade(0, _closeAnimDuration)
                .OnComplete(OnCloseCompleted);
        }
        
        public virtual void CloseImmediately() => OnCloseCompleted();
        
        public virtual void OnCloseButton() => Close();
        
        protected virtual void OnCloseCompleted()
        {
            Destroy(gameObject);
        }
    }
}
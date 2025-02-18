using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class PopupPanel : Panel
    {
        [Header("Background")]
        [SerializeField]
        private bool _hasBackground = true;
        [SerializeField]
        private Image _background;
        [SerializeField]
        private float _targetAlpha = 0.5f;
        
        [Header("Popup")]
        [SerializeField]
        private CanvasGroup _popupCanvasGroup;
        [SerializeField]
        private Transform _popupTransform;
        [SerializeField]
        private float _startSize = 0.5f;

        protected virtual void OnValidate()
        {
            if (_hasBackground && _background != null)
            {
                _background.ChangeAlpha(_targetAlpha);
            }
        }
        
        public override void Open()
        {
            gameObject.SetActive(true);
            
            if (_hasBackground)
            {
                _background.ChangeAlpha(0);
                _background.DOFade(_targetAlpha, OpenAnimDuration);
            }
            
            _popupCanvasGroup.interactable = false;
            _popupCanvasGroup.alpha = 0;
            _popupCanvasGroup.DOFade(1f, OpenAnimDuration)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => _popupCanvasGroup.interactable = true);
            
            _popupTransform.localScale = Vector3.one * _startSize;
            _popupTransform.DOScale(1f, OpenAnimDuration)
                .SetEase(Ease.OutBack);
        }
        
        public override void Close()
        {
            if (_hasBackground)
            {
                _background.DOKill();
                _background.DOFade(0, CloseAnimDuration)
                    .SetEase(Ease.OutCubic);
            }
            
            _popupCanvasGroup.interactable = false;
            _popupCanvasGroup.DOKill();
            _popupCanvasGroup.DOFade(0, CloseAnimDuration)
                .SetEase(Ease.OutCubic);
            
            _popupTransform.DOKill();
            _popupTransform.DOScale(_startSize, OpenAnimDuration)
                .SetEase(Ease.InBack)
                .OnComplete(OnCloseCompleted);
        }
    }
}
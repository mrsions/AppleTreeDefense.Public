using UnityEngine;
using TMPro;
using DG.Tweening;

namespace ATD
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DamageText : MonoBehaviour
    {
        private TextMeshProUGUI textComponent;
        private RectTransform rectTransform;
        private DamageTextPool pool;
        
        private float floatHeight;
        private float floatDuration;
        private Ease floatEase;
        
        private Tween currentTween;

        public void Initialize(DamageTextPool damagePool, float height, float duration, Ease ease)
        {
            pool = damagePool;
            floatHeight = height;
            floatDuration = duration;
            floatEase = ease;
            
            textComponent = GetComponent<TextMeshProUGUI>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void Show(string damageText, Vector2 canvasPosition, Color textColor)
        {
            // 텍스트 설정
            textComponent.text = damageText;
            textComponent.color = textColor;
            
            // 위치 설정 (Canvas 로컬 좌표계 사용)
            rectTransform.anchoredPosition = canvasPosition;
            
            // 초기 상태 설정
            rectTransform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            
            // 애니메이션 시퀀스
            Sequence sequence = DOTween.Sequence();
            
            // 1. 팝업 애니메이션 (0.15초)
            sequence.Append(rectTransform.DOScale(1.3f, 0.1f).SetEase(Ease.OutBack));
            sequence.Append(rectTransform.DOScale(1f, 0.05f).SetEase(Ease.InOutQuad));
            
            // 2. 위로 떠오르며 페이드 아웃 (설정한 지속시간)
            Vector2 endPosition = canvasPosition + Vector2.up * floatHeight;
            sequence.Join(rectTransform.DOAnchorPosY(endPosition.y, floatDuration).SetEase(floatEase));
            
            // 3. 약간의 좌우 흔들림 효과 추가
            float randomOffset = Random.Range(-20f, 20f);
            sequence.Join(rectTransform.DOAnchorPosX(canvasPosition.x + randomOffset, floatDuration * 0.3f)
                .SetEase(Ease.OutQuad));
            
            // 4. 페이드 아웃 (끝부분에서)
            sequence.Join(textComponent.DOFade(0f, floatDuration * 0.6f).SetDelay(floatDuration * 0.4f));
            
            // 5. 완료 후 풀로 반환
            sequence.OnComplete(() => ReturnToPool());
            
            currentTween = sequence;
        }

        private void ReturnToPool()
        {
            if (currentTween != null)
            {
                currentTween.Kill();
                currentTween = null;
            }
            
            // 상태 초기화
            textComponent.color = Color.white;
            rectTransform.localScale = Vector3.one;
            
            pool.ReturnToPool(this);
        }

        private void OnDestroy()
        {
            if (currentTween != null)
            {
                currentTween.Kill();
            }
        }
    }
}
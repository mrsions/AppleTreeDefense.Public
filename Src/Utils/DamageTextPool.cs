using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ATD
{
    public class DamageTextPool : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private int poolSize = 20;
        [SerializeField] private Canvas damageCanvas;

        [Header("Text Settings")]
        [SerializeField] private float floatHeight = 100f;
        [SerializeField] private float floatDuration = 1f;
        [SerializeField] private DG.Tweening.Ease floatEase = DG.Tweening.Ease.OutQuad;

        private static DamageTextPool s_Instance;
        public static DamageTextPool Instance => s_Instance ??= FindObjectOfType<DamageTextPool>();

        private Queue<DamageText> damageTextPool = new Queue<DamageText>();

        private void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
                InitializePool();
            }
            else if (s_Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializePool()
        {
            // Canvas가 없으면 생성
            if (damageCanvas == null)
            {
                GameObject canvasObject = new GameObject("DamageTextCanvas");
                canvasObject.transform.SetParent(transform);
                damageCanvas = canvasObject.AddComponent<Canvas>();
                damageCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                damageCanvas.sortingOrder = 100;

                CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
            }

            // 풀 초기화
            for (int i = 0; i < poolSize; i++)
            {
                CreateNewDamageText();
            }
        }

        private void CreateNewDamageText()
        {
            GameObject textObj;

            if (damageTextPrefab != null)
            {
                textObj = Instantiate(damageTextPrefab, damageCanvas.transform);
            }
            else
            {
                // 기본 TMP 텍스트 오브젝트 생성
                textObj = new GameObject("DamageText");
                textObj.transform.SetParent(damageCanvas.transform, false);

                TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
                textComponent.text = "0";
                textComponent.fontSize = 32;
                textComponent.color = Color.red;
                textComponent.alignment = TextAlignmentOptions.Center;

                // 게임에서 사용하는 폰트 설정
                textComponent.font = Resources.Load<TMP_FontAsset>("DNFBitBitv2 SDF");
                if (textComponent.font == null)
                {
                    // 게임 폰트가 없으면 기본 TMP 폰트 사용
                    textComponent.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
                }

                // 아웃라인 효과 설정
                textComponent.outlineWidth = 0.3f;
                textComponent.outlineColor = Color.black;
            }

            DamageText damageText = textObj.GetComponent<DamageText>();
            if (damageText == null)
            {
                damageText = textObj.AddComponent<DamageText>();
            }

            damageText.Initialize(this, floatHeight, floatDuration, floatEase);
            damageText.gameObject.SetActive(false);
            damageTextPool.Enqueue(damageText);
        }

        public void ShowDamageText(Vector3 worldPosition, int damage, Color? textColor = null)
        {
            ShowDamageText(worldPosition, damage.ToString(), textColor);
        }

        public void ShowDamageText(Vector3 worldPosition, string text, Color? textColor = null)
        {
            if (damageTextPool.Count == 0)
            {
                CreateNewDamageText();
            }

            DamageText damageText = damageTextPool.Dequeue();

            // 월드 좌표를 Canvas의 앵커드 포지션으로 변환
            Vector2 canvasPosition = WorldToCanvasPosition(worldPosition);

            damageText.Show(text, canvasPosition, textColor ?? Color.red);
        }

        private Vector2 WorldToCanvasPosition(Vector3 worldPosition)
        {
            // 1. 월드 좌표를 스크린 좌표로 변환
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            // 2. 스크린 좌표를 Canvas의 RectTransform 로컬 좌표로 변환
            RectTransform canvasRectTransform = damageCanvas.GetComponent<RectTransform>();
            Vector2 localPoint;
            
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform, 
                screenPosition, 
                Camera.main, 
                out localPoint))
            {
                return localPoint;
            }
            
            // 변환 실패시 기본값 반환
            return Vector2.zero;
        }

        public void ReturnToPool(DamageText damageText)
        {
            damageText.gameObject.SetActive(false);
            damageTextPool.Enqueue(damageText);
        }

        // 다양한 데미지 타입을 위한 편의 메서드들
        public void ShowPlayerDamage(Vector3 worldPosition, int damage)
        {
            ShowDamageText(worldPosition, damage, Color.white);
        }

        public void ShowMonsterDamage(Vector3 worldPosition, int damage)
        {
            ShowDamageText(worldPosition, damage, Color.yellow);
        }

        public void ShowHealingText(Vector3 worldPosition, int healing)
        {
            ShowDamageText(worldPosition, "+" + healing, Color.green);
        }

        public void ShowCriticalDamage(Vector3 worldPosition, int damage)
        {
            ShowDamageText(worldPosition, damage + "!", new Color(1.0f, 0.5f, 0f));
        }
    }
}
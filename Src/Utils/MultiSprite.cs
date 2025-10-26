using UnityEngine;
using UnityEngine.UI;

namespace ATD.Data
{
    [ExecuteInEditMode]
    public class MultiSprite : MonoBehaviour
    {
        public Sprite[] sprites;

        [SerializeField]
        private int m_index;

        public float autoSpeed = 0;
        public bool autoDestroy;

        public int Index
        {
            get => m_index;
            set
            {
                m_index = value;
                Refresh();
            }
        }

        private void Awake()
        {
            Refresh();
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void OnValidate()
        {
            Refresh();
        }

        private void OnDidApplyAnimationProperties()
        {
            Refresh();
        }

        private void Refresh()
        {
            if (sprites?.Length == 0)
            {
                return;
            }

            var renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = sprites[m_index];
                return;
            }

            var img = GetComponent<Image>();
            if (img != null)
            {
                img.sprite = sprites[m_index];
            }
        }

        private float time;
        private void Update()
        {
            if (autoSpeed <= 0) return;

            int idx = (int)(time / autoSpeed);
            if (idx < sprites.Length)
            {
                Index = idx;
                time += Time.deltaTime;
            }
            else if (autoDestroy)
            {
                Destroy(gameObject);
            }
            else
            {
                time = 0;
            }
        }
    }
}
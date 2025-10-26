using UnityEngine;

namespace ATD
{
    public class BgmPlay : MonoBehaviour
    {
        [Header("BGM Settings")]
        [SerializeField] private int bgmIndex = 0;
        [SerializeField] private bool playOnStart = false;

        private void Start()
        {
            if (playOnStart)
            {
                PlayBgm();
            }
        }

        private void OnEnable()
        {
            if (!playOnStart)
            {
                PlayBgm();
            }
        }

        private void PlayBgm()
        {
            if (BgmManager.Instance != null)
            {
                BgmManager.Instance.PlayBgm(bgmIndex);
                Debug.Log($"[BgmPlay] Requested BGM {bgmIndex} from {gameObject.name}");
            }
            else
            {
                Debug.LogError("[BgmPlay] BgmManager instance not found!");
            }
        }

        public void SetBgmIndex(int index)
        {
            bgmIndex = index;
        }

        public void PlaySpecificBgm(int index)
        {
            bgmIndex = index;
            PlayBgm();
        }

        // Inspector에서 테스트용으로 호출할 수 있는 메서드
        [ContextMenu("Play BGM")]
        public void PlayBgmFromEditor()
        {
            PlayBgm();
        }
    }
}
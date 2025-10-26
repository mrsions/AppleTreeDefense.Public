using System.Collections;
using UnityEngine;

namespace ATD
{
    public class BgmManager : MonoBehaviour
    {
        private static BgmManager s_Instance;
        public static BgmManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    GameObject bgmManagerObj = new GameObject("BgmManager");
                    s_Instance = bgmManagerObj.AddComponent<BgmManager>();
                    DontDestroyOnLoad(bgmManagerObj);
                    s_Instance.Initialize();
                }
                return s_Instance;
            }
        }

        [Header("BGM Settings")]
        [SerializeField] private AudioClip[] bgmClips = new AudioClip[4]; // 0, 1, 2, 3 음악
        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private float volume = 1f;

        [SerializeField] private AudioSource primaryAudioSource;
        [SerializeField] private AudioSource secondaryAudioSource;
        private int currentBgmIndex = -1;
        private bool isTransitioning = false;

        private void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (s_Instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }

        private void Initialize()
        {
            // Primary AudioSource 생성
            if (!primaryAudioSource) primaryAudioSource = gameObject.AddComponent<AudioSource>();
            primaryAudioSource.loop = true;
            primaryAudioSource.volume = volume;
            primaryAudioSource.playOnAwake = false;

            // Secondary AudioSource 생성 (크로스페이드용)
            if (!secondaryAudioSource) secondaryAudioSource = gameObject.AddComponent<AudioSource>();
            secondaryAudioSource.loop = true;
            secondaryAudioSource.volume = 0f;
            secondaryAudioSource.playOnAwake = false;
        }

        public void PlayBgm(int bgmIndex, bool forceImmediate = false)
        {
            if (bgmIndex < 0 || bgmIndex >= bgmClips.Length)
            {
                Debug.LogWarning($"[BgmManager] Invalid BGM index: {bgmIndex}");
                return;
            }

            if (bgmClips[bgmIndex] == null)
            {
                Debug.LogWarning($"[BgmManager] BGM clip at index {bgmIndex} is null");
                return;
            }

            // 같은 음악이 이미 재생 중이면 무시
            if (currentBgmIndex == bgmIndex && primaryAudioSource.isPlaying)
            {
                return;
            }

            // 최초 재생이거나 강제 즉시 재생인 경우
            if (currentBgmIndex == -1 || forceImmediate)
            {
                PlayImmediately(bgmIndex);
            }
            else
            {
                // 트랜지션과 함께 재생
                StartCoroutine(PlayWithTransition(bgmIndex));
            }
        }

        private void PlayImmediately(int bgmIndex)
        {
            primaryAudioSource.clip = bgmClips[bgmIndex];
            primaryAudioSource.volume = volume;
            primaryAudioSource.Play();
            currentBgmIndex = bgmIndex;

            Debug.Log($"[BgmManager] Playing BGM {bgmIndex} immediately");
        }

        private IEnumerator PlayWithTransition(int bgmIndex)
        {
            if (isTransitioning) yield break;

            isTransitioning = true;

            Debug.Log($"[BgmManager] Transitioning from BGM {currentBgmIndex} to {bgmIndex}");

            // Secondary에 새로운 음악 설정
            secondaryAudioSource.clip = bgmClips[bgmIndex];
            secondaryAudioSource.volume = 0f;
            secondaryAudioSource.Play();

            float elapsedTime = 0f;
            float startVolume = primaryAudioSource.volume;

            // 크로스페이드 진행
            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;
                primaryAudioSource.volume = Mathf.Lerp(startVolume, 0f, t);
                secondaryAudioSource.volume = Mathf.Lerp(0f, volume, t);
                print($"primary {primaryAudioSource.volume}, second {secondaryAudioSource.volume}");

                yield return null;
            }

            // Primary와 Secondary AudioSource 교체
            print($"stop primary");
            primaryAudioSource.Stop();

            var temp = primaryAudioSource;
            primaryAudioSource = secondaryAudioSource;
            secondaryAudioSource = temp;

            secondaryAudioSource.volume = 0f;

            currentBgmIndex = bgmIndex;
            isTransitioning = false;

            Debug.Log($"[BgmManager] Transition to BGM {bgmIndex} completed");
        }

        public void StopBgm()
        {
            primaryAudioSource.Stop();
            secondaryAudioSource.Stop();
            currentBgmIndex = -1;
            isTransitioning = false;
        }

        public void SetVolume(float newVolume)
        {
            volume = Mathf.Clamp01(newVolume);
            if (!isTransitioning)
            {
                primaryAudioSource.volume = volume;
            }
        }

        public int GetCurrentBgmIndex()
        {
            return currentBgmIndex;
        }

        public bool IsPlaying()
        {
            return primaryAudioSource.isPlaying;
        }
    }
}
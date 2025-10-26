using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ATD
{
    /// <summary>
    /// 타이틀 화면을 관리하고 게임 시작 시 인트로 비디오를 재생하는 클래스
    /// </summary>
    public class TitleScene : MonoBehaviour
    {
        public VideoPlayer video; // 인트로 비디오 플레이어
        public Image preview; // 비디오 재생 전 표시되는 미리보기 이미지
        public string sceneName; // 로드할 다음 씬 이름

        /// <summary>
        /// 게임을 시작하고 인트로 비디오 재생 프로세스를 시작
        /// </summary>
        public void StartGame()
        {
            StartCoroutine(Proc());
        }

        /// <summary>
        /// 인트로 비디오를 재생하고 완료 후 다음 씬으로 전환하는 코루틴
        /// </summary>
        /// <returns>코루틴 이터레이터</returns>
        private IEnumerator Proc()
        {
            var op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            preview.DOFade(0, 0.3f);
            video.Play();

            while (Math.Abs(video.clip.length - video.time) > 0.1)
            {
                yield return null;
            }

            op.allowSceneActivation = true;
        }

        /// <summary>
        /// 애플리케이션을 종료 (에디터에서는 플레이 모드 종료)
        /// </summary>
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
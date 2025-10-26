using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ATD
{
    /// <summary>
    /// 게임 시작 시 표시되는 스플래시 화면을 관리하는 클래스
    /// </summary>
    public class SplashScene : MonoBehaviour
    {
        public GameObject m_LoadingFrame; // 로딩 화면 프레임
        public GameObject m_ClickToPlayFrame; // 클릭하여 시작 안내 프레임
        public Button m_ClickToPlayBtn; // 클릭하여 시작 버튼

        /// <summary>
        /// 초기화 시 모든 UI 프레임을 비활성화
        /// </summary>
        private void Awake()
        {
            m_LoadingFrame.SetActiveSafe(false);
            m_ClickToPlayFrame.SetActiveSafe(false);
            m_ClickToPlayBtn.enabled = false;
        }

        /// <summary>
        /// 스플래시 화면 시퀀스를 실행하는 코루틴
        /// </summary>
        /// <returns>코루틴 이터레이터</returns>
        IEnumerator Start()
        {
            m_LoadingFrame.SetActiveSafe(true);
            m_ClickToPlayFrame.SetActiveSafe(false);

            // fake loading
            yield return new WaitForSeconds(2);
            m_ClickToPlayFrame.GetComponent<TMP_Text>().DOFade(0, 0.5f).SetLoops(-1, LoopType.Yoyo);

            m_LoadingFrame.SetActiveSafe(false);
            m_ClickToPlayFrame.SetActiveSafe(true);
            m_ClickToPlayBtn.enabled = true;
        }
    }
}
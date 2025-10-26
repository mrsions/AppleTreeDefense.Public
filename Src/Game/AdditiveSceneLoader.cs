using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace ATD
{
    public class AdditiveSceneLoader : MonoBehaviour
    {
        private string lastLoadedSceneName; // 최근 로드한 씬 이름 저장

        // 버튼에서 이 메서드를 호출하면서 씬 이름을 전달
        public void LoadAdditiveScene(string sceneName)
        {
            lastLoadedSceneName = sceneName; // 이름 저장

            // 비동기로 Additive 로드
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive)
                .completed += (op) =>
                {
                    // 방금 로드된 씬 참조
                    Scene loadedScene = SceneManager.GetSceneByName(sceneName);

                    if (loadedScene.IsValid())
                    {
                        // 씬 안의 루트 오브젝트들 가져오기
                        GameObject[] rootObjects = loadedScene.GetRootGameObjects();

                        foreach (var root in rootObjects)
                        {
                            // EventSystem이면 비활성화
                            if (root.GetComponent<EventSystem>() != null)
                            {
                                root.SetActive(false);
                            }

                            // AudioListener 예시
                            if (root.GetComponent<AudioListener>() != null)
                            {
                                root.GetComponent<AudioListener>().enabled = false;
                            }

                            // 필요하면 다른 타입도 여기서 처리 가능
                            // 예: UIManager, GameManager 등
                        }
                    }
                };
        }

        // 최근 로드한 Additive 씬 언로드
        public void UnloadLastLoadedScene()
        {
            if (!string.IsNullOrEmpty(lastLoadedSceneName))
            {
                if (SceneManager.GetSceneByName(lastLoadedSceneName).isLoaded)
                {
                    SceneManager.UnloadSceneAsync(lastLoadedSceneName)
                        .completed += (op) =>
                        {
                            Debug.Log($"씬 언로드 완료: {lastLoadedSceneName}");
                            lastLoadedSceneName = null;
                        };
                }
                else
                {
                    Debug.LogWarning($"씬 '{lastLoadedSceneName}'은(는) 로드되어 있지 않습니다.");
                }
            }
            else
            {
                Debug.LogWarning("언로드할 씬 이름이 없습니다. 먼저 LoadAdditiveScene을 호출하세요.");
            }
        }
    }
}

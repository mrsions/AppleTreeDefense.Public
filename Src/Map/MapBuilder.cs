using Unity.AI.Navigation;
using UnityEngine;

namespace ATD
{
    /// <summary>
    /// NavMesh를 빌드하고 관리하는 클래스
    /// </summary>
    public class MapBuilder : MonoBehaviour
    {
        public static MapBuilder Instance; // 싱글톤 인스턴스

        /// <summary>
        /// 활성화 시 인스턴스를 등록하고 NavMesh를 빌드
        /// </summary>
        private void OnEnable()
        {
            Instance = this;
            Bake();
        }

        /// <summary>
        /// 비활성화 시 인스턴스를 해제
        /// </summary>
        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// 모든 NavMeshSurface 컴포넌트를 활성화하고 NavMesh를 빌드
        /// </summary>
        public void Bake()
        {
            foreach (var surface in GetComponents<NavMeshSurface>())
            {
                surface.enabled = true;
                surface.BuildNavMesh();
            }
        }

    }
}
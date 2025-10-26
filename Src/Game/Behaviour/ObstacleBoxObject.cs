using System.Collections;
using System.Linq;
using UnityEngine;

namespace ATD
{
    /// <summary>
    /// 장애물 상자 오브젝트 (NavMesh 영향)
    /// NavMesh를 변경하는 장애물로 배치/제거 시 맵을 재구성합니다
    /// </summary>
    public class ObstacleBoxObject : PlayerObjectBehaviour
    {
        /// <summary>장애물은 탐지 불가능 (항상 false)</summary>
        public override bool isDetectable { get => false; set { } }

        /// <summary>
        /// 활성화 시 NavMesh 재구성
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            MapBuilder.Instance?.Bake();
        }

        /// <summary>
        /// 비활성화 시 NavMesh 재구성
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            MapBuilder.Instance?.Bake();
        }

        /// <summary>
        /// 시작 시 NavMesh 재구성
        /// </summary>
        protected override void Start()
        {
            MapBuilder.Instance?.Bake();
        }

        /// <summary>
        /// 사망 시 오브젝트를 파괴합니다
        /// </summary>
        /// <param name="sender">공격을 가한 오브젝트</param>
        public override void OnDeath(ObjectBehaviour sender)
        {
            base.OnDeath(sender);
            Destroy(gameObject);
        }
    }
}
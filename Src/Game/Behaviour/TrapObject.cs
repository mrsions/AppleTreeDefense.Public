using System.Collections;
using UnityEngine;

namespace ATD
{
    /// <summary>
    /// 함정 오브젝트 (은폐형 방어 구조물)
    /// 몬스터에게 탐지되지 않는 방어 오브젝트입니다
    /// </summary>
    public class TrapObject : PlayerObjectBehaviour
    {
        /// <summary>함정은 탐지 불가능 (항상 false)</summary>
        public override bool isDetectable { get => false; set { } }

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
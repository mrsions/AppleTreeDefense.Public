using System.Collections;
using UnityEngine;

namespace ATD
{
    /// <summary>
    /// 상자 오브젝트 (방어 구조물)
    /// 간단한 방어용 오브젝트로 파괴 시 제거됩니다
    /// </summary>
    public class BoxObject : PlayerObjectBehaviour
    {
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
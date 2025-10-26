using UnityEngine;

namespace ATD
{
    /// <summary>
    /// 플레이어가 소유한 오브젝트의 기반 클래스
    /// 사과나무의 글로벌 스탯을 공격력과 방어력에 적용합니다
    /// </summary>
    public abstract class PlayerObjectBehaviour : ObjectBehaviour
    {
        /// <summary>사과나무의 글로벌 공격력을 포함한 계산된 공격력</summary>
        public override int calcDamage => GameManager.Instance.appleTree.Global_Damage;
        /// <summary>사과나무의 글로벌 방어력을 포함한 계산된 방어력</summary>
        public override int calcDefense => GameManager.Instance.appleTree.Global_Defense;
    }
}
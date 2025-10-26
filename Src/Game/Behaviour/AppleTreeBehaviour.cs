#nullable enable

using System;
using ATD.Data;
using UnityEngine;

namespace ATD
{
    /// <summary>
    /// 사과나무 오브젝트의 행동을 제어하는 클래스
    /// 게임의 중심 오브젝트로 파괴 시 게임오버 처리됩니다
    /// </summary>
    public class AppleTreeBehaviour : PlayerObjectBehaviour
    {
        [SerializeField] private AudioClip sfxOnHit; // 피격 효과음

        public int Global_Damage { get; set; } // 전역 공격력
        public int Global_Defense { get; set; } // 전역 방어력
        public int attack { get; set; } // 기본 공격력
        public int defense { get; set; } // 기본 방어력

        public int levelHp { get; private set; } = -1; // HP 레벨
        public int levelAttack { get; private set; } = -1; // 공격력 레벨
        public int levelDefense { get; private set; } = -1; // 방어력 레벨

        public GameObject[] models; // 모델 배열
        //public MultiSprite sprites;
        public AppleTreeData treeData { get; set; } // 사과나무 데이터


        /// <summary>
        /// 초기화 및 스탯 설정
        /// </summary>
        protected override void Start()
        {
            base.Start();
            treeData = GameData.Instance.appleTree;
            InitializeStats();
        }

        /// <summary>
        /// 사과나무의 스탯을 초기화합니다
        /// </summary>
        private void InitializeStats()
        {
            maxHp = treeData.defaultHp;
            currentHp = maxHp;
            attack = treeData.defaultAttack;
            defense = treeData.defaultDefense;

            Global_Damage = 0;
            Global_Defense = 0;

            Debug.Log($"[AppleTreeBehaviour] Initialize HP: {maxHp}, Attack: {Global_Damage}, Defense: {Global_Defense}");
        }

        /// <summary>
        /// 데미지를 받고 효과음을 재생합니다
        /// </summary>
        /// <param name="sender">공격을 가한 오브젝트</param>
        /// <param name="damage">데미지</param>
        /// <returns>실제 입은 데미지</returns>
        public override int OnHit(ObjectBehaviour sender, int damage)
        {
            SimpleAudio.Play(sfxOnHit);
            return base.OnHit(sender, damage);
        }

        /// <summary>
        /// 사망 시 게임오버 처리를 수행합니다
        /// </summary>
        /// <param name="sender">공격을 가한 오브젝트</param>
        public override void OnDeath(ObjectBehaviour sender)
        {
            base.OnDeath(sender);

            Debug.Log("[AppleTreeBehaviour] Tree has been destroyed!");
            GameManager.Instance.OnGameOver(GameOverType.Ending1_Defeat);
        }


        /// <summary>
        /// 업그레이드 가능할때 객체를 return합니다. 불가능하거나 max일때는 null을 리턴합니다.
        /// </summary>
        /// <returns>다음 레벨 HP 데이터 또는 null</returns>
        public LevelUpData? GetHp_NextLevel() => levelHp + 1 < treeData.upgradesHp.Length ? treeData.upgradesHp[levelHp + 1] : null;

        /// <summary>
        /// 다음 레벨 공격력 업그레이드 데이터를 반환합니다
        /// </summary>
        /// <returns>다음 레벨 공격력 데이터 또는 null</returns>
        public LevelUpData? GetAttack_NextLevel() => levelAttack + 1 < treeData.upgradesAttack.Length ? treeData.upgradesAttack[levelAttack + 1] : null;

        /// <summary>
        /// 다음 레벨 방어력 업그레이드 데이터를 반환합니다
        /// </summary>
        /// <returns>다음 레벨 방어력 데이터 또는 null</returns>
        public LevelUpData? GetDefense_NextLevel() => levelDefense + 1 < treeData.upgradesDefense.Length ? treeData.upgradesDefense[levelDefense + 1] : null;

        /// <summary>
        /// HP를 레벨업합니다
        /// </summary>
        [ContextMenu("AddHp_LevelUp")]
        public void AddHp_LevelUp()
        {
            var level = GetHp_NextLevel() ?? throw new NotImplementedException();
            levelHp++;
            maxHp += level.value;
            currentHp += level.value;
        }

        /// <summary>
        /// 공격력을 레벨업합니다
        /// </summary>
        [ContextMenu("AddAttack_LevelUp")]
        public void AddAttack_LevelUp()
        {
            var level = GetAttack_NextLevel() ?? throw new NotImplementedException();
            levelAttack++;
            Global_Damage += level.value;
        }

        /// <summary>
        /// 방어력을 레벨업합니다
        /// </summary>
        [ContextMenu("AddDefense_LevelUp")]
        public void AddDefense_LevelUp()
        {
            var level = GetDefense_NextLevel() ?? throw new NotImplementedException();
            levelDefense++;
            Global_Defense += level.value;
        }

        /// <summary>
        /// 최대 체력의 일정 비율만큼 회복합니다
        /// </summary>
        /// <param name="percent">회복 비율 (0.0 ~ 1.0)</param>
        public void Heal(float percent)
        {
            var healHp = (int)(maxHp * percent);
            if (currentHp + healHp > maxHp)
            {
                healHp = maxHp - currentHp;
            }
            currentHp += healHp;
            DamageTextPool.Instance.ShowHealingText(transform.position, healHp);
        }
    }
}
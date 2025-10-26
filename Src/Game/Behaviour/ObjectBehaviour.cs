using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ATD
{
    /// <summary>
    /// 게임 내 모든 상호작용 가능한 오브젝트의 기반 클래스
    /// 자동 등록/해제 기능을 제공하며 HP 시스템과 데미지 처리를 담당합니다
    /// </summary>
    public abstract class ObjectBehaviour : MonoBehaviour
    {
        [SerializeField] private int m_currentHp; // 현재 체력
        [SerializeField] private int m_maxHp; // 최대 체력

        public virtual bool isDetectable { get; set; } = true; // 탐지 가능 여부
        public virtual int calcDamage => 0; // 계산된 공격력
        public virtual int calcDefense => 0; // 계산된 방어력

        /// <summary>체력이 변경될 때 호출됩니다 (이전HP, 현재HP, 이전최대HP, 현재최대HP)</summary>
        public event Action<ObjectBehaviour, int, int, int, int> onChangedHp;
        /// <summary>오브젝트가 사망할 때 호출됩니다</summary>
        public event Action<ObjectBehaviour> onDeath;

        /// <summary>
        /// 현재 체력 (0 이하가 되면 사망 상태로 전환)
        /// </summary>
        public virtual int currentHp
        {
            get => m_currentHp;
            set
            {
                var befhp = currentHp;
                m_currentHp = Mathf.Clamp(value, 0, maxHp);
                if (m_currentHp <= 0)
                {
                    m_currentHp = 0;
                    IsDeath = true;
                }
                else
                {
                    IsDeath = false;
                }
                onChangedHp?.Invoke(this, befhp, currentHp, maxHp, maxHp);
            }
        }

        /// <summary>
        /// 최대 체력
        /// </summary>
        public virtual int maxHp
        {
            get => m_maxHp;
            set
            {
                var befhp = maxHp;
                m_maxHp = value;
                onChangedHp?.Invoke(this, currentHp, currentHp, befhp, maxHp);
            }
        }

        /// <summary>사망 여부</summary>
        public bool IsDeath { get; private set; }

        /// <summary>
        /// 오브젝트 초기화 시 ObjectManager에 자동 등록
        /// </summary>
        protected virtual void Start()
        {
            GameManager.Instance.objectManager?.Register(this);
        }

        /// <summary>
        /// 오브젝트 활성화 시 ObjectManager에 자동 등록
        /// </summary>
        protected virtual void OnEnable()
        {
            GameManager.Instance.objectManager?.Register(this);
        }

        /// <summary>
        /// 오브젝트 비활성화 시 ObjectManager에서 자동 해제
        /// </summary>
        protected virtual void OnDisable()
        {
            GameManager.Instance?.objectManager?.Unregister(this);
        }

        /// <summary>
        /// 데미지를 받고 체력을 감소시킵니다
        /// </summary>
        /// <param name="sender">공격을 가한 오브젝트</param>
        /// <param name="damage">기본 데미지</param>
        /// <returns>실제 입은 데미지</returns>
        public virtual int OnHit(ObjectBehaviour sender, int damage)
        {
            if (IsDeath) return 0;

            int maxDmg = currentHp;
            int actualDamage = Mathf.Clamp(damage - calcDefense, 1, damage);
            var newHp = currentHp - actualDamage;

            currentHp = newHp;

            // 데미지 텍스트 표시
            Vector3 damagePosition = transform.position + Vector3.up * 1.5f; // 오브젝트 위쪽에 표시

            if (this is MonsterBehaviour)
            {
                DamageTextPool.Instance?.ShowMonsterDamage(damagePosition, actualDamage);
            }
            else if (this is PlayerObjectBehaviour)
            {
                DamageTextPool.Instance?.ShowPlayerDamage(damagePosition, actualDamage);
            }
            else
            {
                DamageTextPool.Instance?.ShowDamageText(damagePosition, actualDamage);
            }

            if (IsDeath)
            {
                OnDeath(sender);
                return maxDmg;
            }
            else
            {
                return actualDamage;
            }
        }

        /// <summary>
        /// 오브젝트 사망 처리를 수행합니다
        /// </summary>
        /// <param name="sender">공격을 가한 오브젝트</param>
        public virtual void OnDeath(ObjectBehaviour sender)
        {
            onDeath?.Invoke(this);
            GameManager.Instance.objectManager.Unregister(this);
        }
    }
}
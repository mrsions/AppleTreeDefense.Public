using System.Collections;
using ATD.Data;
using UnityEngine;
using UnityEngine.AI;

namespace ATD
{
    /// <summary>
    /// 몬스터 엔티티 행동을 제어하는 클래스
    /// NavMesh 기반 이동, 타겟 탐색 및 공격 기능을 담당합니다
    /// </summary>
    public class MonsterBehaviour : ObjectBehaviour
    {
        public MonsterData data { get; set; } // 몬스터 데이터
        /// <summary>몬스터 데이터에서 가져온 최대 체력</summary>
        public override int maxHp => data?.hp ?? 0;

        [Header("Components")]
        public NavMeshAgent agent; // NavMesh 에이전트
        [SerializeField] private Animator m_Anim; // 애니메이터
        [SerializeField] private AudioClip sfxOnHit; // 피격 효과음

        [Header("Attack")]
        [SerializeField] private float m_attackRange; // 공격 범위
        [SerializeField] private bool m_useAnimHit=true; // 피격 애니메이션 사용 여부
        [SerializeField] private bool m_useAnim; // 애니메이션 사용 여부
        [SerializeField] private Transform m_model; // 모델 트랜스폼
        [SerializeField] private Transform m_firePoint; // 발사 지점
        [SerializeField] private GameObject m_projectilePrefab; // 투사체 프리팹
        [SerializeField] private float m_projectileSpeed; // 투사체 속도
        [SerializeField] private float m_attackCooldown = 1f; // 공격 쿨다운
        [SerializeField] private float m_attackDuration = 0.6f; // 공격 지속 시간

        /// <summary>몬스터 데이터에서 가져온 공격력</summary>
        public override int calcDamage => data.attack;
        /// <summary>몬스터 데이터에서 가져온 방어력</summary>
        public override int calcDefense => data.defense;

        private ObjectBehaviour m_currentTarget; // 현재 공격 대상
        private float m_lastAttackTime; // 마지막 공격 시간
        private bool m_isAttacking; // 공격 중 여부

        /// <summary>
        /// 초기화 및 스탯 설정
        /// </summary>
        protected override void Start()
        {
            base.Start();
            currentHp = data.hp;
            agent.speed = data.moveSpeed;
        }

        private void Update()
        {
            if (GameManager.Instance.isGameOver || IsDeath)
            {
                agent.enabled = false;
                enabled = false;
                return;
            }

            // Find new target if current target is dead or null
            if (m_currentTarget == null || m_currentTarget.IsDeath)
            {
                FindNewTarget();
            }

            // Update destination if we have a target
            if (m_currentTarget != null && agent.enabled)
            {
                agent.destination = m_currentTarget.transform.position;

                // Check if we're close enough to attack
                float distanceToTarget = Vector3.Distance(transform.position, m_currentTarget.transform.position);
                if (distanceToTarget <= m_attackRange && CanAttack())
                {
                    agent.enabled = false;
                    m_lastAttackTime = Time.time;
                    if (m_useAnim)
                    {
                        m_Anim.SetBool("attack", true);
                    }
                    else
                    {
                        AttackTarget();
                    }
                }
                else
                {
                    if (m_useAnim)
                    {
                        m_Anim.SetBool("move", true);
                    }
                }
                
                // Handle sprite flipping based on movement direction
                HandleSpriteFlipping();
            }
        }

        /// <summary>
        /// 새로운 공격 대상을 찾습니다 (플레이어 오브젝트 우선, 없으면 사과나무)
        /// </summary>
        private void FindNewTarget()
        {
            // First try to find the nearest user building (including turrets)
            m_currentTarget = GameManager.Instance.objectManager.FindNearestPlayerObjects(transform.position);

            // If no user buildings, target the apple tree as fallback
            if (m_currentTarget == null)
            {
                m_currentTarget = GameManager.Instance.appleTree;
            }
        }

        /// <summary>
        /// 공격 가능 여부를 확인합니다
        /// </summary>
        /// <returns>공격 가능 여부</returns>
        private bool CanAttack()
        {
            return Time.time - m_lastAttackTime >= m_attackCooldown;
        }

        /// <summary>
        /// 타겟을 공격합니다
        /// </summary>
        private void AttackTarget()
        {
            if (m_currentTarget == null) return;

            StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                float endTime = m_lastAttackTime + m_attackDuration;
                float remainTime = Mathf.Max(0, endTime - Time.time);

                if (m_projectilePrefab != null && m_firePoint != null)
                {
                    agent.enabled = false;
                    StartCoroutine(FireProjectile());
                }
                else
                {
                    // Direct hit if no projectile
                    m_currentTarget.OnHit(this, calcDamage);
                    Debug.Log($"[MonsterBehaviour] Direct hit on {m_currentTarget.name} for {calcDamage} damage");
                }

                yield return new WaitForSeconds(remainTime);

                agent.enabled = true;
                m_Anim.SetBool("attack", false);

                // If target dies, find new target immediately
                if (m_currentTarget.IsDeath)
                {
                    m_currentTarget = null;
                    FindNewTarget();
                }
            }
        }

        /// <summary>
        /// 투사체를 발사합니다
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator FireProjectile()
        {
            m_isAttacking = true;

            if (m_projectilePrefab)
            {
                GameObject projectile = Instantiate(m_projectilePrefab, m_firePoint.position, m_firePoint.rotation);

                var projectileBehaviour = projectile.GetComponent<ProjectileBehaviour>() ?? projectile.AddComponent<ProjectileBehaviour>();
                projectileBehaviour.Setup(this, m_currentTarget, calcDamage, m_projectileSpeed);
                projectileBehaviour.onComplete += () => { if (!IsDeath) agent.enabled = true; };
            }
            else
            {
                if (!IsDeath) agent.enabled = true;
            }

            m_isAttacking = false;
            yield return null;
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnTriggerEnter(collision.collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            var target = other.GetComponentInParent<PlayerObjectBehaviour>();
            if (target)
            {
                int damaged = target.OnHit(this, currentHp);
                if (damaged > 0) OnHit(target, damaged);
            }
        }

        /// <summary>
        /// 데미지를 받고 피격 애니메이션을 재생합니다
        /// </summary>
        /// <param name="sender">공격을 가한 오브젝트</param>
        /// <param name="damage">데미지</param>
        /// <returns>실제 입은 데미지</returns>
        public override int OnHit(ObjectBehaviour sender, int damage)
        {
            SimpleAudio.Play(sfxOnHit);

            var result = base.OnHit(sender, damage);
            if (m_useAnimHit && !IsDeath)
            {
                agent.enabled = false;
                m_Anim.SetTrigger("hit");
            }
            return result;
        }

        /// <summary>
        /// 피격 애니메이션 종료 콜백
        /// </summary>
        public void OnEndHitAnim()
        {
            if(!IsDeath) agent.enabled = true;
        }

        /// <summary>
        /// 사망 처리 및 사망 애니메이션 재생
        /// </summary>
        /// <param name="sender">공격을 가한 오브젝트</param>
        public override void OnDeath(ObjectBehaviour sender)
        {
            base.OnDeath(sender);
            agent.enabled = false;
            if (m_useAnim)
            {
                m_Anim.SetBool("dead", true);
            }
            else
            {
                OnEndDeath();
            }
        }

        /// <summary>
        /// 사망 애니메이션 종료 콜백 (오브젝트 파괴)
        /// </summary>
        public void OnEndDeath()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// 이동 방향에 따라 모델의 좌우를 뒤집습니다
        /// </summary>
        private void HandleSpriteFlipping()
        {
            if (m_model == null || agent == null) return;

            // Get movement velocity from NavMeshAgent
            Vector3 velocity = agent.velocity;
            
            // Only flip if there's significant horizontal movement
            if (Mathf.Abs(velocity.x) > 0.1f)
            {
                Vector3 currentScale = m_model.localScale;
                float targetScaleX;
                
                if (velocity.x < 0) // Moving left
                {
                    targetScaleX = -Mathf.Abs(currentScale.x);
                }
                else // Moving right
                {
                    targetScaleX = Mathf.Abs(currentScale.x);
                }
                
                // Only change scale if different to avoid unnecessary updates
                if (!Mathf.Approximately(currentScale.x, targetScaleX))
                {
                    m_model.localScale = new Vector3(targetScaleX, currentScale.y, currentScale.z);
                }
            }
        }
    }
}
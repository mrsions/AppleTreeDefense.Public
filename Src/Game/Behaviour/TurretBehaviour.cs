using System.Collections;
using UnityEngine;

namespace ATD
{
    /// <summary>
    /// 터렛 오브젝트의 행동을 제어하는 클래스
    /// 범위 내 몬스터를 자동으로 추적하고 공격합니다
    /// </summary>
    public class TurretBehaviour : PlayerObjectBehaviour
    {
        [Header("Turret Stats")]
        [SerializeField] private int m_attack = 10; // 기본 공격력
        [SerializeField] private int m_defense = 10; // 기본 방어력
        [SerializeField] private float m_attackRange = 5f; // 공격 범위
        [SerializeField] private float m_attackCooldown = 1f; // 공격 쿨다운

        [Header("Sounds")]
        public AudioClip sfxAttack; // 공격 효과음
        public AudioClip sfxAttackHit; // 공격 명중 효과음

        [Header("Components")]
        [SerializeField] private Animator m_Anim; // 애니메이터
        [SerializeField] private Transform m_header; // 터렛 헤더 (회전 부분)

        [Header("Projectile")]
        [SerializeField] private GameObject m_projectilePrefab; // 투사체 프리팹
        [SerializeField] private Transform m_firePoint; // 발사 지점
        [SerializeField] private float m_projectileSpeed = 10f; // 투사체 속도
        [SerializeField] public bool m_useAttackAnim = true; // 공격 애니메이션 사용 여부

        private MonsterBehaviour m_currentTarget; // 현재 공격 대상
        private float m_lastAttackTime; // 마지막 공격 시간
        private bool m_isAttacking; // 공격 중 여부

        /// <summary>터렛 기본 공격력 + 글로벌 공격력</summary>
        public override int calcDamage => m_attack + base.calcDamage;
        /// <summary>터렛 기본 방어력 + 글로벌 방어력</summary>
        public override int calcDefense => m_defense + base.calcDefense;
        /// <summary>공격 범위</summary>
        public float attackRange => m_attackRange;
        /// <summary>공격 쿨다운</summary>
        public float attackCooldown => m_attackCooldown;

        void Update()
        {
            if (IsDeath) return;

            // Find target if we don't have one or current target is dead/out of range
            if (m_currentTarget == null || m_currentTarget.IsDeath ||
                Vector3.Distance(transform.position, m_currentTarget.transform.position) > m_attackRange)
            {
                FindTarget();
            }

            // Always track the target with header if we have one
            if (m_currentTarget != null && m_header != null)
            {
                TrackTarget();
            }

            // Attack target if possible
            if (m_currentTarget != null && CanAttack())
            {
                if (m_useAttackAnim)
                {
                    m_lastAttackTime = Time.time;
                    m_Anim.SetTrigger("attack");
                }
                else
                {
                    AttackTarget();
                }
            }
        }

        /// <summary>
        /// 범위 내에서 가장 가까운 몬스터를 찾습니다
        /// </summary>
        private void FindTarget()
        {
            m_currentTarget = GameManager.Instance.objectManager.FindNearestMonster(transform.position, m_attackRange);
        }

        /// <summary>
        /// 공격 가능 여부를 확인합니다
        /// </summary>
        /// <returns>공격 가능 여부</returns>
        private bool CanAttack()
        {
            return !m_isAttacking && Time.time - m_lastAttackTime >= m_attackCooldown;
        }

        /// <summary>
        /// 터렛 헤더를 타겟 방향으로 회전시킵니다
        /// </summary>
        private void TrackTarget()
        {
            if (m_currentTarget == null || m_header == null) return;

            Vector3 direction = m_currentTarget.transform.position - m_header.position;
            direction.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            m_header.rotation = Quaternion.Slerp(m_header.rotation, targetRotation, Time.deltaTime * 5f);

        }

        /// <summary>
        /// 타겟을 공격합니다
        /// </summary>
        private void AttackTarget()
        {
            if (m_currentTarget == null) return;

            m_lastAttackTime = Time.time;
            SimpleAudio.Play(sfxAttack);

            if (m_projectilePrefab != null && m_firePoint != null)
            {
                StartCoroutine(FireProjectile());
            }
            else
            {
                // Direct hit if no projectile
                m_currentTarget.OnHit(this, calcDamage);
                Debug.Log($"[TurretBehaviour] Direct hit on {m_currentTarget.data.name} for {calcDamage} damage");
            }
        }

        /// <summary>
        /// 투사체를 발사합니다
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator FireProjectile()
        {
            m_isAttacking = true;

            GameObject projectile = Instantiate(m_projectilePrefab, m_firePoint.position, m_firePoint.rotation);

            var projectileBehaviour = projectile.GetComponent<ProjectileBehaviour>() ?? projectile.AddComponent<ProjectileBehaviour>();
            projectileBehaviour.Setup(this, m_currentTarget, calcDamage, m_projectileSpeed);
            projectileBehaviour.sfxHit = sfxAttackHit;

            m_isAttacking = false;
            yield return null;
        }

        /// <summary>
        /// 사망 처리 및 오브젝트 파괴
        /// </summary>
        /// <param name="sender">공격을 가한 오브젝트</param>
        public override void OnDeath(ObjectBehaviour sender)
        {
            base.OnDeath(sender);
            Destroy(gameObject);
        }

        void OnDrawGizmosSelected()
        {
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_attackRange);

            // Draw line from header to current target
            if (m_currentTarget != null)
            {
                Gizmos.color = Color.yellow;
                Vector3 headerPos = m_header != null ? m_header.position : transform.position;
                Gizmos.DrawLine(headerPos, m_currentTarget.transform.position);

                // Draw header direction
                if (m_header != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(m_header.position, m_header.forward * 2f);
                }
            }
        }
    }
}
using System;
using UnityEngine;

namespace ATD
{
    /// <summary>
    /// 투사체 오브젝트의 행동을 제어하는 클래스
    /// 타겟을 추적하여 이동하고 명중 시 데미지를 입힙니다
    /// </summary>
    public class ProjectileBehaviour : MonoBehaviour
    {
        private ObjectBehaviour m_target; // 투사체 타겟
        private ObjectBehaviour m_sender; // 투사체 발사자
        private int m_damage; // 데미지
        private float m_speed; // 이동 속도
        private bool m_isInitialized; // 초기화 여부

        /// <summary>투사체 완료 이벤트</summary>
        public event Action onComplete;
        public AudioClip sfxHit; // 명중 효과음
        private Animator anim; // 애니메이터
        public GameObject hitFx; // 명중 이펙트

        /// <summary>
        /// 투사체 초기화
        /// </summary>
        /// <param name="sender">발사자</param>
        /// <param name="target">타겟</param>
        /// <param name="damage">데미지</param>
        /// <param name="speed">속도 (음수일 경우 즉시 명중)</param>
        public void Setup(ObjectBehaviour sender, ObjectBehaviour target, int damage, float speed)
        {
            m_sender = sender;
            m_target = target;
            m_damage = damage;
            m_speed = speed;
            m_isInitialized = true;
        }

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (!m_isInitialized) return;

            // Destroy if target is dead or null
            if (m_target == null || m_target.IsDeath)
            {
                Destroy(gameObject);
                return;
            }

            // ���ǵ尡 ������� �ٷ� �����Ѵ�.
            if (m_speed < 0)
            {
                transform.position = m_target.transform.position;
                if(anim == null)
                {
                    Attack();
                    Complete();
                }
            }
            else
            {
                // Move towards target
                Vector3 direction = (m_target.transform.position - transform.position).normalized;
                transform.position += direction * m_speed * Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(direction);

                // Check if we hit the target
                if (Vector3.Distance(transform.position, m_target.transform.position) < 0.5f)
                {
                    Attack();
                    Complete();
                }
            }
        }

        /// <summary>
        /// 타겟을 공격하고 이펙트를 생성합니다
        /// </summary>
        public void Attack()
        {
            SimpleAudio.Play(sfxHit);

            m_target.OnHit(m_sender, m_damage);

            if(hitFx)
            {
                Instantiate(hitFx, transform.position, Quaternion.identity, transform.parent);
            }

            Debug.Log($"[ProjectileBehaviour] Hit {m_target.name} for {m_damage} damage");
        }

        /// <summary>
        /// 투사체를 완료하고 파괴합니다
        /// </summary>
        public void Complete()
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        }
    }
}
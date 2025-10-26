using UnityEngine;

namespace ATD
{
    /// <summary>
    /// DamageTextPool 사용법 예시 코드 (TMP_Text 버전)
    /// 이 스크립트를 게임 오브젝트에 붙이고 테스트해볼 수 있습니다.
    /// </summary>
    public class DamageTextExample : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private KeyCode testKey = KeyCode.Space;
        [SerializeField] private Transform testTarget;
        
        private void Update()
        {
            // 스페이스바를 누르면 테스트 데미지 텍스트 표시
            if (Input.GetKeyDown(testKey))
            {
                Vector3 testPosition = testTarget != null ? testTarget.position : transform.position;
                ShowRandomDamageText(testPosition);
            }
        }
        
        private void ShowRandomDamageText(Vector3 position)
        {
            int randomDamage = Random.Range(10, 100);
            int damageType = Random.Range(0, 5);
            
            switch (damageType)
            {
                case 0:
                    DamageTextPool.Instance?.ShowPlayerDamage(position, randomDamage);
                    break;
                case 1:
                    DamageTextPool.Instance?.ShowMonsterDamage(position, randomDamage);
                    break;
                case 2:
                    DamageTextPool.Instance?.ShowCriticalDamage(position, randomDamage);
                    break;
                case 3:
                    DamageTextPool.Instance?.ShowHealingText(position, randomDamage);
                    break;
                case 4:
                    DamageTextPool.Instance?.ShowDamageText(position, "MISS!", Color.gray);
                    break;
            }
        }
        
        /// <summary>
        /// 코드에서 직접 호출하는 사용법 예시들
        /// </summary>
        public void ExampleUsage()
        {
            Vector3 hitPosition = transform.position;
            
            // 기본 데미지 텍스트
            DamageTextPool.Instance?.ShowDamageText(hitPosition, 50);
            
            // 플레이어가 입은 데미지 (빨간색)
            DamageTextPool.Instance?.ShowPlayerDamage(hitPosition, 25);
            
            // 몬스터가 입은 데미지 (노란색)
            DamageTextPool.Instance?.ShowMonsterDamage(hitPosition, 75);
            
            // 크리티컬 데미지 (오렌지색)
            DamageTextPool.Instance?.ShowCriticalDamage(hitPosition, 150);
            
            // 힐링 텍스트 (초록색)
            DamageTextPool.Instance?.ShowHealingText(hitPosition, 30);
            
            // 커스텀 텍스트와 색상
            DamageTextPool.Instance?.ShowDamageText(hitPosition, "BLOCKED!", Color.blue);
            DamageTextPool.Instance?.ShowDamageText(hitPosition, "EVADED", Color.cyan);
        }
    }
    
    /// <summary>
    /// 몬스터나 플레이어 스크립트에서 사용하는 예시
    /// </summary>
    public static class DamageTextUsageExamples
    {
        /// <summary>
        /// 몬스터가 데미지를 받았을 때
        /// </summary>
        public static void OnMonsterTakeDamage(Vector3 position, int damage, bool isCritical = false)
        {
            if (isCritical)
            {
                DamageTextPool.Instance?.ShowCriticalDamage(position, damage);
            }
            else
            {
                DamageTextPool.Instance?.ShowMonsterDamage(position, damage);
            }
        }
        
        /// <summary>
        /// 플레이어 유닛이 데미지를 받았을 때
        /// </summary>
        public static void OnPlayerTakeDamage(Vector3 position, int damage)
        {
            DamageTextPool.Instance?.ShowPlayerDamage(position, damage);
        }
        
        /// <summary>
        /// 플레이어가 체력을 회복했을 때
        /// </summary>
        public static void OnPlayerHeal(Vector3 position, int healAmount)
        {
            DamageTextPool.Instance?.ShowHealingText(position, healAmount);
        }
        
        /// <summary>
        /// 공격이 빗나갔을 때
        /// </summary>
        public static void OnAttackMiss(Vector3 position)
        {
            DamageTextPool.Instance?.ShowDamageText(position, "MISS!", Color.gray);
        }
        
        /// <summary>
        /// 공격이 막혔을 때
        /// </summary>
        public static void OnAttackBlocked(Vector3 position)
        {
            DamageTextPool.Instance?.ShowDamageText(position, "BLOCKED", Color.blue);
        }
    }
}
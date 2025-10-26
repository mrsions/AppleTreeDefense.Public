using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ATD.Data;
using TMPro;
using UnityEngine;

namespace ATD
{
    public class NightMode : MonoBehaviour
    {
        public float time;

        public TMP_Text timeText;
        public string timeFormat = "Clear in {0} seconds";
        private List<MonsterBehaviour> spawns = new();
        private bool isSpawnComplete;
        private int aliveCount;

        public IEnumerator Run(RoundData round)
        {
            Debug.Log($"[NightMode] Starting night mode - Duration: {round.duration}s, Monsters: {round.monsters.Length}");
            gameObject.SetActiveSafe(true);

            var spawningCoroutine = StartCoroutine(Spawning(round));

            isSpawnComplete = false;
            spawns.Clear();
            aliveCount = 0;
            time = 0f;

            while (time < round.duration && (!isSpawnComplete || aliveCount > 0)) 
            {
                timeText.text = string.Format(timeFormat, (int)(round.duration - time));
                yield return null;
                time += Time.deltaTime;

                if (GameManager.Instance.isGameOver) break;
            }

            if (GameManager.Instance.isGameOver) yield break;

            Debug.Log($"[NightMode] Timer completed - Remaining monsters: {aliveCount}");
            if(aliveCount > 0)
            {
                Debug.Log("[NightMode] Game Over - Timeout (monsters remaining)");
                GameManager.Instance.OnGameOver(GameOverType.Timeover);
                StopCoroutine(spawningCoroutine);
            }

            gameObject.SetActiveSafe(false);
        }

        private IEnumerator Spawning(RoundData round)
        {
            yield return new WaitForSeconds(round.spawnStartDelay);

            var points = GameManager.Instance.spawnPoints;

            foreach (var monsterId in round.monsters)
            {
                var monsterData = GameData.Instance.GetMonster(monsterId);

                var bounds = points[Random.Range(0, points.Length)].bounds;
                float radius = Mathf.Max(bounds.extents.x, bounds.extents.y);
				var distance = Random.Range(0, radius);
                var y = Random.Range(0, 360);
                var position = bounds.center + Quaternion.Euler(0, y, 0) * new Vector3(0, 0, distance);

                var behaviour = Instantiate(monsterData.prefab, position, Quaternion.identity, GameManager.Instance.spawnContainer);
                behaviour.data = monsterData;
                behaviour.onDeath += (sender) =>
                {
                    aliveCount--;
                };
                aliveCount++;
                spawns.Add(behaviour);
                Debug.Log($"[NightMode] Spawned monster: {monsterData.name} at {position}");

                yield return new WaitForSeconds(round.spawnInterval);
            }
            isSpawnComplete = true;
            Debug.Log($"[NightMode] All monsters spawned");
        }

        public void AttackAll()
        {
            Debug.Log($"[NightMode] AttackAll");

            var sp = GameManager.Instance.objectManager.GetMonstersInRange(Vector3.zero, 20);
            foreach(var mon in sp)
            {
                if (mon) mon.OnHit(GameManager.Instance.appleTree, 10);
            }
        }

        public void SetGameSpeed(float speed)
        {
            Time.timeScale = speed;
        }
    }
}
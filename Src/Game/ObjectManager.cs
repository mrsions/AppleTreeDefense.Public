using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ATD
{
    public class ObjectManager : MonoBehaviour
    {
        public List<ObjectBehaviour> playerObjects = new List<ObjectBehaviour>();
        public List<MonsterBehaviour> monsters = new List<MonsterBehaviour>();

        public void Register(ObjectBehaviour obj)
        {
            if (obj is MonsterBehaviour monster)
            {
                if (!monsters.Contains(monster))
                {
                    monsters.Add(monster);
                    monster.onDeath += (sender) => Unregister(monster);
                }
            }
            else if (obj is PlayerObjectBehaviour pobj)
            {
                if (!playerObjects.Contains(pobj))
                {
                    playerObjects.Add(pobj);
					pobj.onDeath += (sender) => Unregister(sender);
                }
            }
        }

        public void Unregister(ObjectBehaviour obj)
        {
            if (obj is MonsterBehaviour monster)
            {
                monsters.Remove(monster);
            }
            else if (obj is PlayerObjectBehaviour pobj)
            {
                playerObjects.Remove(pobj);
            }
        }

        public ObjectBehaviour FindNearestPlayerObjects(Vector3 position, float maxRange = float.MaxValue)
        {
            ObjectBehaviour nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var obj in playerObjects)
            {
                if (obj == null || obj.IsDeath || !obj.isDetectable) continue;

                float distance = Vector3.Distance(position, obj.transform.position);
                if (distance < nearestDistance && distance <= maxRange)
                {
                    nearest = obj;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }

        public MonsterBehaviour FindNearestMonster(Vector3 position, float maxRange = float.MaxValue)
        {
            MonsterBehaviour nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var obj in monsters)
            {
                if (obj == null || obj.IsDeath || !obj.isDetectable) continue;

                float distance = Vector3.Distance(position, obj.transform.position);
                if (distance < nearestDistance && distance <= maxRange)
                {
                    nearest = obj;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }

        public List<MonsterBehaviour> GetMonstersInRange(Vector3 position, float range)
        {
            List<MonsterBehaviour> monstersInRange = new List<MonsterBehaviour>();

            foreach (var obj in monsters)
            {
                if (obj == null || obj.IsDeath || !obj.isDetectable) continue;

                float distance = Vector3.Distance(position, obj.transform.position);
                if (distance <= range)
                {
                    monstersInRange.Add(obj);
                }
            }

            return monstersInRange;
        }

        public List<ObjectBehaviour> GetPlayerObjectsInRange(Vector3 position, float range)
        {
            List<ObjectBehaviour> buildingsInRange = new List<ObjectBehaviour>();

            foreach (var obj in playerObjects)
            {
                if (obj == null || obj.IsDeath || !obj.isDetectable) continue;

                float distance = Vector3.Distance(position, obj.transform.position);
                if (distance <= range)
                {
                    buildingsInRange.Add(obj);
                }
            }

            return buildingsInRange;
        }

        public int GetAlivePlayerObjectsCount()
        {
            return playerObjects.Count(static b => b != null && !b.IsDeath);
        }

        public int GetAliveMonstersCount()
        {
            return monsters.Count(static m => m != null && !m.IsDeath);
        }
    }
}
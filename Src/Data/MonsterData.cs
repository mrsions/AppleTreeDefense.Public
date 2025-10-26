using System;
using UnityEngine;

namespace ATD.Data
{
    /// <summary>
    /// 몬스터의 기본 정보를 정의하는 데이터 클래스입니다.
    /// </summary>
    [Serializable]
    public class MonsterData
    {
        public int id; // 몬스터 고유 ID
        public Sprite icon; // 몬스터 아이콘
        public string name; // 몬스터 이름
        public int hp; // 몬스터 체력
        public int attack = 1; // 몬스터 공격력
        public int defense = 0; // 몬스터 방어력
        public float moveSpeed; // 몬스터 이동 속도
        public MonsterBehaviour prefab; // 몬스터 프리팹
    }
}
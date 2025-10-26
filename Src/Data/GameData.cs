using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ATD.Data
{
    /// <summary>
    /// 게임 전체의 데이터를 관리하는 ScriptableObject 클래스입니다.
    /// </summary>
    public class GameData : ScriptableObject
    {
        private static GameData s_Instance;
        /// <summary>
        /// GameData의 싱글톤 인스턴스를 가져옵니다.
        /// </summary>
        public static GameData Instance => s_Instance ??= Resources.Load<GameData>("GameData");

        public AppleTreeData appleTree; // 사과나무 데이터
        public RoundData[] rounds; // 라운드별 데이터 배열
        public MonsterData[] monsters; // 몬스터 데이터 배열
        public EndingData[] endings; // 엔딩 데이터 배열
        public BuildingData[] buildings; // 건물 데이터 배열

        [Header("build money table")]
        public int turretPrise = 3; // 포탑 설치 비용
        public int dummyPrise = 3; // 더미 설치 비용
        public int trapPrise = 3; // 함정 설치 비용

        [Header("time spend table")]
        public int farmingTime = 5; // 채집 소요 시간
        public int statTime = 2; // 스탯 업그레이드 소요 시간
        public int buildingTime = 1; // 건물 설치 소요 시간
        public int upgradeTime = 3; // 건물 업그레이드 소요 시간

        [Header("upgrade cost table")]
        [Header("first upgrade prise")]
        public int turretUp_1_Prise = 3; // 포탑 1단계 업그레이드 비용
        public int dummyUp_1_Prise = 3; // 더미 1단계 업그레이드 비용
        public int trapUp_1_Prise = 3; // 함정 1단계 업그레이드 비용
        [Header("second upgrade prise")]
        public int turretUpFactor = 6; // 포탑 2단계 업그레이드 비용 배수
        public int dummyUpFactor = 6; // 더미 2단계 업그레이드 비용 배수
        public int trapUpFactor = 6; // 함정 2단계 업그레이드 비용 배수

        [Header("farming distance (longer)")]
        [Header("0 is first distance, 1 is second distance 2 is... ")]
        public int[] farmingDistance; // 채집 거리 배열

        [Header("mapzise(half radius")]
        public int[] mapsize; // 맵 크기 (반지름)

        /// <summary>
        /// 라운드 ID로 라운드 데이터를 가져옵니다.
        /// </summary>
        /// <param name="id">라운드 ID</param>
        /// <returns>해당 라운드 데이터</returns>
        public RoundData GetRound(int id) => GetById(rounds, id, v => v.round);

        /// <summary>
        /// 몬스터 ID로 몬스터 데이터를 가져옵니다.
        /// </summary>
        /// <param name="id">몬스터 ID</param>
        /// <returns>해당 몬스터 데이터</returns>
        public MonsterData GetMonster(int id) => GetById(monsters, id, v => v.id);

        /// <summary>
        /// 엔딩 ID로 엔딩 데이터를 가져옵니다.
        /// </summary>
        /// <param name="id">엔딩 ID</param>
        /// <returns>해당 엔딩 데이터</returns>
        public EndingData GetEnding(int id) => GetById(endings, id, v => v.id);

        /// <summary>
        /// 건물 ID로 건물 데이터를 가져옵니다.
        /// </summary>
        /// <param name="id">건물 ID</param>
        /// <returns>해당 건물 데이터</returns>
        public BuildingData GetBuilding(int id) => GetById(buildings, id, v => v.id);

        /// <summary>
        /// 배열에서 ID로 특정 데이터를 검색합니다.
        /// </summary>
        /// <typeparam name="T">데이터 타입</typeparam>
        /// <param name="array">검색할 배열</param>
        /// <param name="id">찾을 ID</param>
        /// <param name="GetId">ID를 가져오는 함수</param>
        /// <returns>찾은 데이터</returns>
        public T GetById<T>(IEnumerable<T> array, int id, Func<T, int> GetId)
        {
            if (array == null || id < 1)
            {
                throw new InvalidOperationException($"Invalid Ending ID: {id}");
            }

            foreach (var val in array)
            {
                if (GetId(val) == id)
                {
                    return val;
                }
            }
            throw new System.ArgumentException($"Ending with ID {id} not found");
        }

        /// <summary>
        /// 사과나무 업그레이드 데이터를 자동 생성합니다.
        /// </summary>
        [ContextMenu("Create")]
        public void AutoGen()
        {
            appleTree.upgradesHp = Enumerable.Range(0, 20).Select(v => new LevelUpData() { level = v + 1, name = "정화", description = "사과 나무 체력 +{0}", cost = v + 1, value = v + 10 }).ToArray();
            appleTree.upgradesDefense = Enumerable.Range(0, 20).Select(v => new LevelUpData() { level = v + 1, name = "매력", description = "전체 방어력 +{0}", cost = v + 1, value = v + 1 }).ToArray();
            appleTree.upgradesAttack = Enumerable.Range(0, 20).Select(v => new LevelUpData() { level = v + 1, name = "무력", description = "전체 공격력 +{0}", cost = v + 1, value = v + 1 }).ToArray();
        }
    }
}
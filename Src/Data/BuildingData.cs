using System;

namespace ATD.Data
{
    /// <summary>
    /// 건물의 기본 정보를 정의하는 데이터 클래스입니다.
    /// </summary>
    [Serializable]
    public class BuildingData
    {
        public int id; // 건물 고유 ID
        public string name; // 건물 이름
        public string description; // 건물 설명
        public BuildingLevelData[] levels; // 건물 레벨별 데이터
    }

    /// <summary>
    /// 건물의 레벨별 정보를 정의하는 데이터 클래스입니다.
    /// </summary>
    [Serializable]
    public class BuildingLevelData
    {
        public int level; // 건물 레벨
        public string name; // 레벨별 이름
        public string description; // 레벨별 설명
        public int upgradeCost; // 업그레이드 비용
        public int installCost; // 설치 비용
        public PlayerObjectBehaviour prefab; // 레벨별 프리팹
    }

}
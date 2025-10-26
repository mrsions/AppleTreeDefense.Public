using System;
using UnityEngine;

namespace ATD.Data
{
    /// <summary>
    /// 레벨업 정보를 정의하는 데이터 클래스입니다.
    /// </summary>
    [Serializable]
    public class LevelUpData
    {
        public int level; // 레벨
        public string name; // 레벨업 이름
        [SerializeField] private string m_description; // 설명 포맷 문자열
        public int cost; // 레벨업 비용
        public int value; // 레벨업 시 증가량
        /// <summary>
        /// 포맷팅된 설명 문자열을 가져오거나 설정합니다.
        /// </summary>
        public string description { get => string.Format(m_description, value); set => m_description = value; }


    }

    /// <summary>
    /// 사과나무의 기본 스탯과 업그레이드 정보를 정의하는 데이터 클래스입니다.
    /// </summary>
    [Serializable]
    public class AppleTreeData
    {
        public int defaultHp; // 기본 체력
        public int defaultAttack; // 기본 공격력
        public int defaultDefense; // 기본 방어력
        public LevelUpData[] upgradesHp; // 체력 업그레이드 데이터 배열
        public LevelUpData[] upgradesDefense; // 방어력 업그레이드 데이터 배열
        public LevelUpData[] upgradesAttack; // 공격력 업그레이드 데이터 배열
    }
}
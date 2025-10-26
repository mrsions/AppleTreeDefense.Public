using System;
using UnityEngine;

namespace ATD.Data
{
    /// <summary>
    /// 라운드별 게임 진행 정보를 정의하는 데이터 클래스입니다.
    /// </summary>
    [Serializable]
    public class RoundData
    {
        public int round; // 라운드 번호

        [Header("spawn")]
        public float duration = 60; // 라운드 지속 시간 (초)
        public float spawnStartDelay = 0; // 스폰 시작 지연 시간 (초)
        public float spawnInterval = 1; // 몬스터 스폰 간격 (초)
        public int[] monsters; // 스폰될 몬스터 ID 배열

        [Header("tree")]
        public int tree_imageIdx; // 사과나무 이미지 인덱스
        public int tree_addHp; // 라운드 시작 시 추가될 체력
        [Range(0,1)]
        public float tree_heal = 0.1f; // 체력 회복 비율
        [Range(0,1)]
        public float desaturate = 1; // 채도 감소 정도
    }
}
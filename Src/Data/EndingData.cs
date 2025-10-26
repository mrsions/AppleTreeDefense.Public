using System;
using UnityEngine;

namespace ATD.Data
{
    /// <summary>
    /// 게임 엔딩 정보를 정의하는 데이터 클래스입니다.
    /// </summary>
    [Serializable]
    public class EndingData
    {
        public int id; // 엔딩 고유 ID
        public Sprite icon; // 엔딩 아이콘
        public string name; // 엔딩 이름
        public string description; // 엔딩 설명
    }
}
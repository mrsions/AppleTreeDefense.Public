using System;
using System.Collections.Generic;
using UnityEngine;

namespace ATD.Data
{
    /// <summary>
    /// 플레이어의 게임 진행 데이터를 관리하는 클래스입니다.
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        private const string KEY_PLAYER_DATA = "player"; // PlayerPrefs 저장 키

        private static PlayerData s_Instance;
        /// <summary>
        /// PlayerData의 싱글톤 인스턴스를 가져옵니다.
        /// </summary>
        public static PlayerData Instance
            => s_Instance ??= (PlayerPrefs.HasKey(KEY_PLAYER_DATA)
            ? JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(KEY_PLAYER_DATA))
            : new PlayerData());

        [SerializeField]
        public List<int> clearEndings = new(); // 클리어한 엔딩 ID 목록

        [SerializeField]
        public List<string> completedTutorials = new(); // 완료한 튜토리얼 ID 목록

        /// <summary>
        /// 새로운 엔딩을 클리어 목록에 추가합니다.
        /// </summary>
        /// <param name="id">엔딩 ID</param>
        public void AddEnding(int id)
        {
            if (clearEndings.Contains(id)) return;
            clearEndings.Add(id);
            Save();
        }

        /// <summary>
        /// 튜토리얼을 완료 상태로 표시합니다.
        /// </summary>
        /// <param name="tutorialId">튜토리얼 ID</param>
        public void CompleteTutorial(string tutorialId)
        {
            if (string.IsNullOrEmpty(tutorialId)) return;
            if (completedTutorials.Contains(tutorialId)) return;
            completedTutorials.Add(tutorialId);
            Save();
        }

        /// <summary>
        /// 특정 튜토리얼의 완료 여부를 확인합니다.
        /// </summary>
        /// <param name="tutorialId">튜토리얼 ID</param>
        /// <returns>완료했으면 true, 그렇지 않으면 false</returns>
        public bool HasCompletedTutorial(string tutorialId)
        {
            if (string.IsNullOrEmpty(tutorialId)) return false;
            return completedTutorials.Contains(tutorialId);
        }

        /// <summary>
        /// 모든 튜토리얼 완료 기록을 초기화합니다.
        /// </summary>
        public void ResetAllTutorials()
        {
            completedTutorials.Clear();
            Save();
        }

        /// <summary>
        /// 현재 플레이어 데이터를 PlayerPrefs에 저장합니다.
        /// </summary>
        public static void Save()
        {
            if (s_Instance == null) return;

            PlayerPrefs.SetString("player", JsonUtility.ToJson(s_Instance));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 저장된 플레이어 데이터를 삭제하고 인스턴스를 초기화합니다.
        /// </summary>
        public static void Clear()
        {
            PlayerPrefs.DeleteKey(KEY_PLAYER_DATA);
            s_Instance = null;
        }
    }
}
using System;
using System.Collections;
using ATD.Data;
using UnityEngine;

namespace ATD
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager s_Instance;
        public static GameManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindObjectOfType<GameManager>();
                }
                return s_Instance;
            }
        }

        [Header("components")]
        public DayMode day;
        public GameObject intro;
        public NightMode night;
        public GameObject endings;
        public GameObject dialgues;
        public SphereCollider[] spawnPoints;
        public AppleTreeBehaviour appleTree;
        public Transform spawnContainer;
        public ObjectManager objectManager;


        [Header("Gold")]
        public int gold;

        private const string GoldKey = "PlayerGold";

        public int currentRoundIndex { get; set; }
        public RoundData currentRound { get; set; }
        public bool isDay { get; private set; } = true;
        public GameOverType gameOverType { get; set; } = GameOverType.None;
        public bool isNewEnding { get; set; }
        public bool isGameOver => gameOverType != GameOverType.None;


        public bool isDead => appleTree != null && appleTree.currentHp <= 0;

        void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
            }
            else if (s_Instance != this)
            {
                Debug.LogWarning("Multiple GameManager instances detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            objectManager = gameObject.AddComponent<ObjectManager>();

            // 골드 로드
            LoadGold();
        }

        private void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
            }
            Time.timeScale = 1;
        }

        IEnumerator Start()
        {
            Debug.Log("[GameManager] Start() - Initializing game");
            VolumeAccesible.SetWeight(VolumeAccesible.VolumeType.Day, 1);
            VolumeAccesible.SetWeight(VolumeAccesible.VolumeType.Night, 0);
            VolumeAccesible.SetWeight(VolumeAccesible.VolumeType.Desaturate, 1);

            intro.SetActiveSafe(true);
            day.gameObject.SetActiveSafe(false);
            night.gameObject.SetActiveSafe(false);

            dialgues.gameObject.SetActiveSafe(true);
            foreach (Transform child in dialgues.transform) child.gameObject.SetActiveSafe(false);

            endings.gameObject.SetActiveSafe(true);
            foreach (Transform child in endings.transform) child.gameObject.SetActiveSafe(false);

            Debug.Log("[GameManager] Waiting for intro to complete");
            while (intro.activeSelf) yield return null;

            Debug.Log("[GameManager] Starting game loop");
            currentRoundIndex = 0;
            while (currentRoundIndex < GameData.Instance.rounds.Length && gameOverType == GameOverType.None)
            {
                currentRound = GameData.Instance.rounds[currentRoundIndex];
                Debug.Log($"[GameManager] Starting round {currentRoundIndex}");

                appleTree.models.SetActiveOnly(currentRound.tree_imageIdx);
                appleTree.maxHp += currentRound.tree_addHp;
                appleTree.currentHp += currentRound.tree_addHp;
                appleTree.Heal(currentRound.tree_heal);

                var dialogue = dialgues.transform.GetChild(currentRoundIndex);
                if (dialogue)
                {
                    var dialogueGO = dialogue.gameObject;
                    dialogueGO.SetActiveSafe(true);
                    Debug.Log("[GameManager] Showing round dialogue");
                    while (dialogueGO.activeSelf) yield return null;
                }

                VolumeAccesible.SetWeight(VolumeAccesible.VolumeType.Night, 0, 0.5f);
                VolumeAccesible.SetWeight(VolumeAccesible.VolumeType.Desaturate, currentRound.desaturate, 0.5f);

                isDay = true;
                Debug.Log("[GameManager] Starting day phase");
                yield return day.Run();

                VolumeAccesible.SetWeight(VolumeAccesible.VolumeType.Night, 1, 0.5f);
                isDay = false;
                Debug.Log("[GameManager] Starting night phase");
                yield return night.Run(currentRound);

                currentRoundIndex++;
            }

            Debug.Log($"[GameManager] Game loop ended - gameOverType: {gameOverType}");
            gameOverType = ChangeGameOver(gameOverType);
            int endingIdx = (int)ChangeGameOver(gameOverType);
            if (!PlayerData.Instance.clearEndings.Contains(endingIdx))
            {
                PlayerData.Instance.clearEndings.Add(endingIdx);
                isNewEnding = true;
                Debug.Log($"[GameManager] New ending unlocked: {endingIdx}");
            }

            endings.transform.Find(gameOverType.ToString()).gameObject.SetActiveSafe(true);

            PlayerData.Save();
        }

        private GameOverType ChangeGameOver(GameOverType gameOverType)
        {
            if (gameOverType != GameOverType.None)
            {
                return gameOverType;
            }

            return EndingCalculator.Compute(gameOverType);
        }

        public void OnGameOver(GameOverType type)
        {
            Debug.Log($"[GameManager] OnGameOver - gameOverType: {gameOverType}");
            gameOverType = type;
        }

        #region Gold
        public void AddGold(int amount)
        {
            gold += amount;
            SaveGold();
            Debug.Log($"골드 추가: {amount}, 현재 골드: {gold}");
        }

        public bool SpendGold(int amount)
        {
            if (gold >= amount)
            {
                gold -= amount;
                SaveGold();
                Debug.Log($"골드 사용: {amount}, 남은 골드: {gold}");
                return true;
            }
            Debug.Log("골드 부족!");
            return false;
        }

        private void SaveGold()
        {
            //PlayerPrefs.SetInt(GoldKey, gold);
            //PlayerPrefs.Save();
        }

        private void LoadGold()
        {
            //gold = PlayerPrefs.GetInt(GoldKey, 0); // 저장된 값 없으면 0
        }

        void OnApplicationQuit() // 게임 종료 시 골드 저장
        {
            SaveGold();
        }



        #endregion


    } // class GameManager
}
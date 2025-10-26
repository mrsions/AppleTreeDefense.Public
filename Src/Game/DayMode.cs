using System.Collections;
using DG.Tweening;
using ATD.Data;
using TMPro;
using UnityEngine;


namespace ATD
{
    public class DayMode : MonoBehaviour
    {
        public int startTime = 10;
        public static int day = 0;
        public string remainTimeFormat = "remain {0} hour";

        [Header("UI")]
        public TMP_Text remainTimeText;
        public TMP_Text paymentText; // 구매 처리 여부 텍스트. 비용 부족시 빨간색으로 표시
        // 예 ) "Not enough gold!" or "Purchased!"
        public GameObject layout;

        [Header("Cost")]
        public int farmingTimeCost = 4;
        public int buildTimeCost = 1;
        public int upgradeTimeCost = 2;
        public int treeUpTimeCost = 1;

        [Header("upgrade Cost")]
        public int turretUpgradeCost = 3;
        public int dummyUpgradeCost = 3;
        public int trapUpgradeCost = 3;

        [Header("install Cost")]
        public int turretInstallCost = 3;
        public int dummyInstallCost = 3;
        public int trapInstallCost = 3;

        [Header("Script")]
        [SerializeField] public AdditiveSceneLoader sceneLoader;
        [SerializeField] public BuildingPlacer buildingPlacer;
        [SerializeField] public StatAndDayUi dayUi;

        [Header("install")]
        public GameObject installPanel;

        [Header("upgrade")]
        public GameObject upgradePanel;

        [Header("tree upgrade")]
        public GameObject treeUpgradePanel;


        [Header("Btn Text TreeUP")]
        public TMP_Text HP_Text;
        public TMP_Text Attack_Text;
        public TMP_Text Defence_Text;


        [Header("Btn Text upgrade")]
        public TMP_Text Upgrade0_Text;
        public TMP_Text Upgrade1_Text;
        public TMP_Text Upgrade2_Text;

        [Header("Btn Text upgrade")]
        public TMP_Text Install0_Text;
        public TMP_Text Install1_Text;
        public TMP_Text Install2_Text;

        string hpName;
        string hpDesc;
        string hpCost;

        string atName;
        string atDesc;
        string atCost;

        string deName;
        string deDesc;
        string deCost;

        [HideInInspector]
        public static bool endOfFarmingGelion = false;

        public int remainTime { get; set; }


        private void Awake()
        {
            buildingPlacer = FindFirstObjectByType<BuildingPlacer>();
        }

        private void Start()
        {

            itemLevels = new int[GameData.Instance.buildings.Length];
        }


        public IEnumerator Run()
        {
            Debug.Log($"[DayMode] Starting day mode - Initial time: {startTime} hours");
            
            gameObject.SetActiveSafe(true);
            day += 1; dayUi.TimeIsDay();
            remainTime = startTime;
            Refresh();

            while (remainTime > 0)
            {
                yield return null;
            }

            Debug.Log("[DayMode] Day phase completed - All time consumed");
            dayUi.TimeIsNight();
            gameObject.SetActiveSafe(false);
        }

        public void OnClickFarming() // 파밍
        {
            //if (!Consume(farmingTimeCost)) return; // 시간 부족 시 종료

            if (remainTime < farmingTimeCost) return;

            endOfFarmingGelion = false;

            if (sceneLoader == null)
            {
                Debug.LogError("[DayMode] AdditiveSceneLoader reference is missing!");
                return;
            }
            sceneLoader.LoadAdditiveScene("Mincheol_FarmingScene");

            Debug.Log("[DayMode] Farming action completed");
        }

        private void Update()
        {
            if(endOfFarmingGelion == true)
            {
                endOfFarmingGelion = false;
                Consume(farmingTimeCost);
            }
        }

        private void UpdateHpUp() // 업그레이드 시 정보 갱신
        {
            var hpUpgrade = GameManager.Instance.appleTree.GetHp_NextLevel();

            hpName = hpUpgrade.name;
            hpDesc = hpUpgrade.description;
            hpCost = hpUpgrade.cost.ToString();

            dayUi.StatUpdate();
            HP_Text.text = $"{hpName}\n{hpDesc}\n가격 : {hpCost}";
        }

        private void UpdateAttackUp() // 업그레이드 시 정보 갱신
        {
            var atUpgrade = GameManager.Instance.appleTree.GetAttack_NextLevel();

            atName = atUpgrade.name;
            atDesc = atUpgrade.description;
            atCost = atUpgrade.cost.ToString();

            dayUi.StatUpdate();
            Attack_Text.text = $"{atName}\n{atDesc}\n가격 : {atCost}";
        }

        private void UpdateDefenceUp() // 업그레이드 시 정보 갱신
        {
            var deUpgrade = GameManager.Instance.appleTree.GetDefense_NextLevel();

            deName = deUpgrade.name;
            deDesc = deUpgrade.description;
            deCost = deUpgrade.cost.ToString();

            dayUi.StatUpdate();
            Defence_Text.text = $"{deName}\n{deDesc}\n가격 : {deCost}";
        }


        public void OnClickTreeUp() // 나무 강화 버튼들 활성화
        {

            if (treeUpgradePanel != null)
            {
                treeUpgradePanel.SetActive(true);
                upgradePanel.SetActiveSafe(false);
                installPanel.SetActiveSafe(false);


                var hpUpgrade = GameManager.Instance.appleTree.GetHp_NextLevel();
                var atUpgrade = GameManager.Instance.appleTree.GetAttack_NextLevel();
                var deUpgrade = GameManager.Instance.appleTree.GetDefense_NextLevel();

                hpName = hpUpgrade.name;
                hpDesc = hpUpgrade.description;
                hpCost = hpUpgrade.cost.ToString();

                atName = atUpgrade.name;
                atDesc = atUpgrade.description;
                atCost = atUpgrade.cost.ToString();

                deName = deUpgrade.name;
                deDesc = deUpgrade.description;
                deCost = deUpgrade.cost.ToString();

                HP_Text.text = $"{hpName}\n{hpDesc}\n가격 : {hpCost}";
                Attack_Text.text = $"{atName}\n{atDesc}\n가격 : {atCost}";
                Defence_Text.text = $"{deName}\n{deDesc}\n가격 : {deCost}";
            }
        }


        public void OnClickUpgrade() // 업그레이드 버튼들 활성화
        {

            if (upgradePanel != null)
            {
                upgradePanel.SetActiveSafe(true);
                installPanel.SetActiveSafe(false);
                treeUpgradePanel.SetActive(false);

                var item = GameData.Instance.buildings[0].levels[itemLevels[0]];
                Upgrade0_Text.text = $"{item.name}\n{item.upgradeCost}G";
                item = GameData.Instance.buildings[1].levels[itemLevels[1]];
                Upgrade1_Text.text = $"{item.name}\n{item.upgradeCost}G";
                item = GameData.Instance.buildings[2].levels[itemLevels[2]];
                Upgrade2_Text.text = $"{item.name}\n{item.upgradeCost}G";
            }
            else
            {
                Debug.LogError("[DayMode] Upgrade panel reference is missing!");
            }

            Debug.Log("[DayMode] Upgrade action completed");
        }

        public void OnClickBuilding() // 설치 버튼들 활성화
        {

            if (installPanel != null)
            {
                installPanel.SetActiveSafe(true);
                upgradePanel.SetActiveSafe(false);
                treeUpgradePanel.SetActive(false);

                var item = GameData.Instance.buildings[0].levels[itemLevels[0]];
                Install0_Text.text = $"{item.name}\n{item.installCost}G";
                item = GameData.Instance.buildings[1].levels[itemLevels[1]];
                Install1_Text.text = $"{item.name}\n{item.installCost}G";
                item = GameData.Instance.buildings[2].levels[itemLevels[2]];
                Install2_Text.text = $"{item.name}\n{item.installCost}G";
            }
            else
            {
                Debug.LogError("[DayMode] Install panel reference is missing!");
            }

            Debug.Log("[DayMode] Building action completed");
        }

        public void OnClickSkip()
        {
            remainTime = 0;
        }

        public bool Consume(int cost)
        {
            if (cost > remainTime)
            {
                Debug.LogError($"[DayMode] Not enough time - Required: {cost}, Available: {remainTime}");
                return false;
            }

            remainTime -= cost;
            Refresh();
            ConsumeTime();
            return true;
        }

        public void Refresh()
        {
            remainTimeText.text = string.Format(remainTimeFormat, remainTime);
        }

        public void ConsumeTime()
        {
            remainTimeText.color = Color.red;
            remainTimeText.DOColor(Color.white, 0.3f);
        }

        // -------------------------------------------------
        // Panel Button Methods
        // -------------------------------------------------


        public int[] itemLevels;

        public void UpgradeTurretClick()
        {
            UpgradeItem(0);
        }

        public void UpgradeDummyClick()
        {
            UpgradeItem(1);
        }

        public void UpgradeTrapClick()
        {
            UpgradeItem(2);
        }

        public void UpgradeItem(int index)
        {
            var item = GameData.Instance.buildings[index];
            ref var itemLevel = ref itemLevels[index];
            var lv = item.levels[itemLevel];

            if (itemLevel + 1 < item.levels.Length)
            {
                if (GameManager.Instance.gold < lv.upgradeCost) return;
                if (remainTime < upgradeTimeCost) return;

                itemLevel++;

                GameManager.Instance.SpendGold(turretUpgradeCost);
                Consume(upgradeTimeCost);
            }

            OnClickUpgrade();
        }

        public void InstallTurretClick() => InstallItem(0);
        public void InstallDummyClick() => InstallItem(1);
        public void InstallTrapClick() => InstallItem(2);

        private bool InstallItem(int index)
        {
            var item = GameData.Instance.buildings[index];
            ref var itemLevel = ref itemLevels[index];
            var lv = item.levels[itemLevel];

            // 1. 골드 체크
            if (GameManager.Instance.gold < lv.installCost) return false;

            // 2. 시간 체크 (Consume 대신 단순 비교)
            if (remainTime < buildTimeCost) return false;

            // 3. 골드 차감 후 설치 모드 진입
            GameManager.Instance.SpendGold(lv.installCost);
            buildingPlacer.StartPlacement(lv.prefab.gameObject, lv.installCost, buildTimeCost, this);
            return true;
        }

        //----------------------------------------------------
        // Tree Upgrade
        // ---------------------------------------------------

        public void HP_Up()
        {
            int cost = int.Parse(hpCost);
            if (GameManager.Instance.gold < cost) return;

            GameManager.Instance.SpendGold(cost);
            GameManager.Instance.appleTree.AddHp_LevelUp();
            UpdateHpUp();
            Consume(treeUpTimeCost);
        }

        public void Attack_Up()
        {
            int cost = int.Parse(atCost);
            if (GameManager.Instance.gold < cost) return;

            GameManager.Instance.SpendGold(cost);
            GameManager.Instance.appleTree.AddAttack_LevelUp();
            UpdateAttackUp();
            Consume(treeUpTimeCost);
        }

        public void Defence_Up()
        {
            int cost = int.Parse(deCost);
            if (GameManager.Instance.gold < cost) return;

            GameManager.Instance.SpendGold(cost);
            GameManager.Instance.appleTree.AddDefense_LevelUp();
            UpdateDefenceUp();
            Consume(treeUpTimeCost);
        }




        public void CloseInstallPanel()
        {
            if (installPanel != null)
            {
                installPanel.SetActiveSafe(false);
            }
        }

        public void CloseUpgradePanel()
        {
            if (upgradePanel != null)
            {
                upgradePanel.SetActiveSafe(false);
            }
        }

        public void CloseTreeUpPanel()
        {
            if (treeUpgradePanel != null)
            {
                treeUpgradePanel.SetActiveSafe(false);
            }
        }


    } // class DayMode
}
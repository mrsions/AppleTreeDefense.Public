using UnityEngine;
using ATD.Data;
using TMPro;


namespace ATD
{
    public class StatAndDayUi : MonoBehaviour
    {


        public TMP_Text statText;
        public TMP_Text dayText; 
        
        public void TimeIsDay()
        {
            int day = DayMode.day;
            dayText.text = $"{day} 일 낮";
        }

        public void TimeIsNight()
        {
            int day = DayMode.day;
            dayText.text = $"{day} 일 밤";
        }

        public void StatUpdate()
        {
            // appleTree 에서 각 스탯의 레벨을 받아온다.

            var hpLevel = GameManager.Instance.appleTree.levelHp;
            var atLevel = GameManager.Instance.appleTree.levelAttack;
            var deLevel = GameManager.Instance.appleTree.levelDefense;

            statText.text = $"정화 {hpLevel+1} / 무력 {atLevel+1} / 매력 {deLevel+1}";
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StatUpdate();
        }

    }
}

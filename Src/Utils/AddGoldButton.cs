using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ATD.Data
{
    [RequireComponent(typeof(Button))]
    public class AddGoldButton : MonoBehaviour
    {
        public int amount;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            GameManager.Instance.AddGold(amount);
        }
    }
}
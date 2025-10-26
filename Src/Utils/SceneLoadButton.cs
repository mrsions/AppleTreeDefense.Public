using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ATD.Data
{
    [RequireComponent(typeof(Button))]
    public class SceneLoadButton :MonoBehaviour
    {
        public string sceneName;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ATD
{
    public static class Extensions
    {
        public static void SetActiveSafe(this GameObject gameObject, bool active)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(active);
            }
        }

        public static void SetActiveOnly(this GameObject[] objs, int index)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i]?.SetActive(index == i);
            }
        }
    }
}
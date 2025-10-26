using System;
using System.Collections;
using ATD.Data;
using UnityEngine;
using UnityEngine.Rendering;

namespace ATD
{
    public enum GameOverType
    {
        None = 0,
        Timeover,
        Ending1_Defeat,
        Ending2,
        Ending3,
        Ending4,
        Ending5,
        Ending6,
        Ending7,
        Ending8,
        Ending9
    }

    public class EndingCalculator : MonoBehaviour
    {
        public static GameOverType Compute(GameOverType type)
        {
            var gm = GameManager.Instance;
            var tree = gm.appleTree;
            var cha = tree.GetDefense_NextLevel().level - 1;
            var pur = tree.GetHp_NextLevel().level - 1;
            var pow = tree.GetAttack_NextLevel().level - 1;

            if (cha >= 6 && each_up_and_down(pur, pow, 6))
            {
                return GameOverType.Ending2;
            }
            else if (cha >= 6 && pur < 6 && pow < 6)
            {
                return GameOverType.Ending3;
            }
            else if (pur >= 6 && each_up_and_down(cha, pow, 6))
            {
                return GameOverType.Ending4;
            }
            else if (pur >= 6 && cha < 6 && pow < 6)
            {
                return GameOverType.Ending5;
            }
            else if (pow >= 6 && each_up_and_down(cha, pur, 6))
            {
                return GameOverType.Ending6;
            }
            else if (pow >= 6 && cha <= 6 && pur < 6)
            {
                return GameOverType.Ending7;
            }
            else if(max(pow,max(cha,pur)) < 6)
            {
                return GameOverType.Ending8;
            }
            else if(min(pow,min(cha,pur)) >= 6)
            {
                return GameOverType.Ending9;
            }

            Debug.LogError($"Cha:{cha}, Pur:{pur}, Pow:{pow}");

            return GameOverType.None;
        }

        static int max(int a, int b) => a > b ? a : b;
        static int min(int a, int b) => a > b ? b : a;

        // 둘중 한개는 c보다 높고  둘중 한개는 c보다 낮을때
        static bool each_up_and_down(int a, int b, int c) => max(a, b) >= c && min(a, b) < c;
    }
}
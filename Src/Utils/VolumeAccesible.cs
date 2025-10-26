using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace ATD
{
    public class VolumeAccesible : MonoBehaviour
    {
        public enum VolumeType
        {
            Day,
            Night,
            Desaturate
        }

        public static Volume[] s_volumes = new Volume[3];

        public static void SetWeight(VolumeType type, float weight)
        {
            var volume = s_volumes[(int)type];
            if(volume)
            {
                volume.weight = weight;
            }
        }
        public static void SetWeight(VolumeType type, float weight, float duration)
        {
            int idx = (int)type;
            DOTween.To(() => s_volumes[idx].weight, v => s_volumes[idx].weight = v, weight, duration);
        }

        public VolumeType type;
        private Volume volume;

        private void OnEnable()
        {
            s_volumes[(int)type] = volume = GetComponent<Volume>();
        }

        private void OnDisable()
        {
            if (s_volumes[(int)type] == volume)
            {
                s_volumes[(int)type] = null;
            }
        }
    }
}
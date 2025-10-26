using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ATD.Data
{
    public class TypewriteText : MonoBehaviour
    {
        const float speed = 0.05f;
        public UnityEvent onNext;
        private TweenerCore<int, int, NoOptions> tweener;

        private void OnEnable()
        {
            var text = GetComponent<TMP_Text>();
            var message = text.text;
            float duration = message.Length * speed;
            text.maxVisibleCharacters = 0;

            tweener = DOTween.To(() => text.maxVisibleCharacters, x => text.maxVisibleCharacters = x, message.Length, duration)
                .OnComplete(() => tweener = null);

        }

        public void Complete()
        {
            if (tweener != null)
            {
                tweener.Complete();
                tweener = null;

                var text = GetComponent<TMP_Text>();
                text.maxVisibleCharacters = text.text.Length;
            }
            else
            {
                onNext?.Invoke();
            }
        }
    }
}
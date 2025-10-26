using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ATD
{
    public class HpBarUpdater : MonoBehaviour
    {
        private ObjectBehaviour owner;
        public TMP_Text text;
        public Image fill;

        private void Awake()
        {
            owner = GetComponentInParent<ObjectBehaviour>();
            OnChangedHP(owner, owner.currentHp, owner.currentHp, owner.maxHp, owner.maxHp);
            owner.onChangedHp += OnChangedHP;
        }

        private void OnDestroy()
        {
            if (owner)
            {
                owner.onChangedHp -= OnChangedHP;
            }
        }

        private void OnChangedHP(ObjectBehaviour owner, int befHp, int newHp, int befMaxHp, int newMaxHp)
        {
            if (text != null)
            {
                text.text = ((int)newHp).ToString();
            }
            if (fill != null)
            {
                if (newMaxHp == 0)
                {
                    fill.fillAmount = 1;
                }
                else
                {
                    fill.DOFillAmount(Mathf.Clamp01((float)newHp / newMaxHp), 0.2f);
                }
            }
        }
    }
}
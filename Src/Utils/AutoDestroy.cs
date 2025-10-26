using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ATD.Data
{
    public class AutoDestroy : MonoBehaviour
    {
        public float delay;
        IEnumerator Start()
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
}
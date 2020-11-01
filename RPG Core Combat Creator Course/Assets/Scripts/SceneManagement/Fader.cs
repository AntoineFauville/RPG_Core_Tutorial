using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        public void FadeOutImmediate()
        {
            GetComponent<CanvasGroup>().alpha = 1;
        }

        public IEnumerator FadeOut(float time)
        {
            while (GetComponent<CanvasGroup>().alpha < 1)
            {
                GetComponent<CanvasGroup>().alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            while (GetComponent<CanvasGroup>().alpha > 0)
            {
                GetComponent<CanvasGroup>().alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Future
{
    public class Fade : MonoBehaviour
    {
        [SerializeField] CanvasGroup m_CanvasGroup;
        [SerializeField] UnityEngine.Events.UnityEvent m_OnFadeOut;
        [SerializeField] UnityEngine.Events.UnityEvent m_OnFadeIn;

        [SerializeField] float m_FadeTimeInSeconds;

        public void FadeInOut()
        {
            StartCoroutine(Anim());
        }

        IEnumerator Anim()
        {
            float progress = 0f;
            while (progress < 1f)
            {
                progress += Time.deltaTime / m_FadeTimeInSeconds;
                m_CanvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);

                yield return null;
            }

            m_CanvasGroup.alpha = 1f;
            m_OnFadeOut?.Invoke();

            progress = 0f;
            while (progress < 1f)
            {
                progress += Time.deltaTime / m_FadeTimeInSeconds;
                m_CanvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

                yield return null;
            }

            m_CanvasGroup.alpha = 0f;
            m_OnFadeIn?.Invoke();
        }
    }
}

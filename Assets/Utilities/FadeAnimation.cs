using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Future
{
    public class FadeAnimation : MonoBehaviour
    {
        [SerializeField] SpriteRenderer m_SpriteRenderer;
        [SerializeField] float m_FadeTimeInSeconds;

        bool m_IsAnimating = false;

        public void Show()
        {
            if (m_IsAnimating)
                return;

            StartCoroutine(FadeAnim(0f, 1f));
        }

        public void Hide()
        {
            if (m_IsAnimating)
                return;

            StartCoroutine(FadeAnim(1f, 0f));
        }

        IEnumerator FadeAnim(float fromAlpha, float toAlpha)
        {
            m_IsAnimating = true;

            float progress = 0f;
            while (progress < 1f)
            {
                progress += Time.deltaTime / m_FadeTimeInSeconds;
                m_SpriteRenderer.color = new Color(
                    m_SpriteRenderer.color.r,
                    m_SpriteRenderer.color.g,
                    m_SpriteRenderer.color.b,
                    Mathf.Lerp(fromAlpha, toAlpha, progress));
                yield return null;
            }

            m_SpriteRenderer.color = new Color(
                   m_SpriteRenderer.color.r,
                   m_SpriteRenderer.color.g,
                   m_SpriteRenderer.color.b, toAlpha);

            m_IsAnimating = true;
        }
    }
}
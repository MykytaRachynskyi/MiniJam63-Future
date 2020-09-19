using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Future
{
    public class PencilStrokeAnimation : MonoBehaviour
    {
        [SerializeField] bool m_Looping;
        [SerializeField] Sprite[] m_States;
        [SerializeField] SpriteRenderer m_Sprite;

        [SerializeField] float m_TimeInSecondsBetweenStateChange;
        [SerializeField] bool m_PlayOnAwake;

        Coroutine m_Animation;

        private void Awake()
        {
            if (m_PlayOnAwake)
                StartAnimation();
        }

        public void StartAnimation()
        {
            StopAnimation();
            m_Animation = StartCoroutine(Animation());
        }

        public void StopAnimation()
        {
            if (m_Animation != null)
                StopCoroutine(m_Animation);
        }

        IEnumerator Animation()
        {
            if (m_Looping)
            {
                while (true)
                {
                    int currentState = 0;
                    while (currentState < m_States.Length)
                    {
                        float progress = 0f;

                        m_Sprite.sprite = m_States[currentState];

                        while (progress < m_TimeInSecondsBetweenStateChange)
                        {
                            progress += Time.deltaTime;

                            yield return null;
                        }

                        currentState++;
                    }
                }
            }
            else
            {
                int currentState = 0;
                while (currentState < m_States.Length)
                {
                    float progress = 0f;

                    m_Sprite.sprite = m_States[currentState];

                    while (progress < m_TimeInSecondsBetweenStateChange)
                    {
                        progress += Time.deltaTime;

                        yield return null;
                    }

                    currentState++;
                }
            }
        }
    }
}

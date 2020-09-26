using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

namespace Future
{
    public class IntVarPresenter : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshProUGUI m_Text;
        [SerializeField] IntVariable m_VariableToPresent;
        [SerializeField] float m_AnimationTimeInSeconds;
        [SerializeField] string m_Prefix;

        [SerializeField] bool m_UseInitialValue;
        [SerializeField] int m_InitialValue;

        [SerializeField] Color m_PositiveValueColor;
        [SerializeField] Color m_NegativeValueColor;

        System.Text.StringBuilder m_StringBuilder = new System.Text.StringBuilder();
        int m_PreviousValue;

        private void Awake()
        {
            if (m_UseInitialValue)
                m_VariableToPresent.Value = m_InitialValue;

            m_PreviousValue = m_VariableToPresent.Value;
            m_VariableToPresent.AddListener(UpdateText);

            SetText(m_VariableToPresent.Value);
        }

        private void OnDestroy()
        {
            m_VariableToPresent.RemoveListener(UpdateText);
        }

        void UpdateText()
        {
            StartCoroutine(AnimateValue(m_PreviousValue, m_VariableToPresent.Value));
            m_PreviousValue = m_VariableToPresent.Value;
        }

        void SetText(int value)
        {
            m_StringBuilder.Clear();
            m_StringBuilder.Append(m_Prefix);
            m_StringBuilder.Append(value.ToString());
            m_Text.text = m_StringBuilder.ToString();

            m_Text.color = value >= 0 ? m_PositiveValueColor : m_NegativeValueColor;
        }

        IEnumerator AnimateValue(int fromValue, int toValue)
        {
            float progress = 0f;
            while (progress < 1f)
            {
                progress += Time.deltaTime / m_AnimationTimeInSeconds;
                int value = (int)Mathf.Lerp(fromValue, toValue, progress);

                SetText(value);

                yield return null;
            }

            SetText(toValue);
        }
    }
}
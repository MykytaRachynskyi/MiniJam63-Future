using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Future
{
    public class FullMessagePopupController : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshProUGUI m_MainText;

        public void SetText(string text)
        {
            m_MainText.text = text;
        }
    }
}
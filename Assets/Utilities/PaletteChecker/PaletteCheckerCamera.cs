using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Future.Utilities
{
    public class PaletteCheckerCamera : MonoBehaviour
    {
        [SerializeField] RenderTexture m_RenderTexture;
        [SerializeField] Camera m_Camera;

        [ExecuteInEditMode]
        [ContextMenu("WriteToRenderTexture")]
        public void WriteToRenderTexture()
        {
            m_Camera.forceIntoRenderTexture = true;
            m_Camera.targetTexture = m_RenderTexture;
            m_Camera.Render();
        }
    }
}
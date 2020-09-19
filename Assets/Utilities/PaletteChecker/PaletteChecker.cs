using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Future.Utilities
{
    public class PaletteChecker : MonoBehaviour
    {
        [SerializeField] Texture2D m_Palette;
        [SerializeField] Color[] m_ColorsInPalette;

        [SerializeField] RenderTexture m_RenderTexture;
        [SerializeField] Color[] m_ColorsInTexture;

        [SerializeField] Texture2D m_TextureToConvert;

        [ExecuteInEditMode]
        [ContextMenu("GetPalette")]
        public void GetPalette()
        {
            m_ColorsInPalette = GetColorsFromTex(m_Palette);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [ExecuteInEditMode]
        [ContextMenu("GetRenderTextureColors")]
        public void GetRenderTextureColors()
        {
            Texture2D tex = new Texture2D(256, 256);
            RenderTexture.active = m_RenderTexture;
            tex.ReadPixels(new Rect(0, 0, m_RenderTexture.width, m_RenderTexture.height), 0, 0);
            tex.Apply();

            m_ColorsInTexture = GetColorsFromTex(tex);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [ExecuteInEditMode]
        [ContextMenu("ConvertToTexturePalette")]
        public void ConvertToTexturePalette()
        {
            Texture2D newTex = new Texture2D(m_TextureToConvert.width, m_TextureToConvert.height);
            Color[] oldColors = m_TextureToConvert.GetPixels();

            HashSet<Color> paletteSet = new HashSet<Color>();
            for (int i = 0; i < m_ColorsInPalette.Length; i++)
            {
                if (!paletteSet.Contains(m_ColorsInPalette[i]))
                    paletteSet.Add(m_ColorsInPalette[i]);
            }

            for (int i = 0; i < oldColors.Length; i++)
            {
                if (!paletteSet.Contains(oldColors[i]))
                    oldColors[i] = FindNearestColorInPalette(oldColors[i]);
            }

            newTex.SetPixels(oldColors);
#if UNITY_EDITOR
            byte[] bytes = newTex.EncodeToPNG();
            FileStream stream = new FileStream(Application.dataPath + "newTex.png", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);
            for (int i = 0; i < bytes.Length; i++)
            {
                writer.Write(bytes[i]);
            }
            writer.Close();
            stream.Close();
            DestroyImmediate(newTex);
            //I can't figure out how to import the newly created .png file as a texture
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public Color FindNearestColorInPalette(Color src)
        {
            float minimumDistance = 255f * 255f + 255f * 255f + 255f * 255f + 1f;

            int closestColorID = 0;

            for (int i = 0; i < m_ColorsInPalette.Length; i++)
            {
                float distR = Mathf.Abs(m_ColorsInPalette[i].r - src.r);
                float distG = Mathf.Abs(m_ColorsInPalette[i].g - src.g);
                float distB = Mathf.Abs(m_ColorsInPalette[i].b - src.b);

                float distance = distR * distR + distG * distG + distB * distB;

                if (distance < minimumDistance)
                    closestColorID = i;
            }

            return m_ColorsInPalette[closestColorID];
        }

        Color[] GetColorsFromTex(Texture2D tex)
        {
            var texPixels = tex.GetPixels();

            HashSet<Color> foundColors = new HashSet<Color>();
            Color[] colorArray;

            {
                for (int i = 0; i < texPixels.Length; i++)
                {
                    if (!foundColors.Contains(texPixels[i]))
                        foundColors.Add(texPixels[i]);
                }
            }

            {
                colorArray = new Color[foundColors.Count];
                int i = 0;
                foreach (var item in foundColors)
                {
                    colorArray[i] = item;
                    i++;
                }
            }

            return colorArray;
        }
    }
}
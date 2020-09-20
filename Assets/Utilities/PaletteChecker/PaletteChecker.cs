using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Future.Utilities
{
    public class PaletteChecker : MonoBehaviour
    {
        enum ConverterColorSettings { RGB, HSV }

        [SerializeField] Texture2D m_Palette;
        [SerializeField] Color[] m_ColorsInPalette;

        [SerializeField] RenderTexture m_RenderTexture;
        [SerializeField] Color[] m_ColorsInTexture;

        [SerializeField] Texture2D m_TextureToConvert;

        [Header("Closest color algorithm settings")]
        [Range(0f, 1f)]
        [SerializeField] float m_HueWeight = .47f;
        [Range(0f, 1f)]
        [SerializeField] float m_SaturationWeight = .2875f;
        [Range(0f, 1f)]
        [SerializeField] float m_ValueWeight = .2375f;
        [SerializeField]
        ConverterColorSettings m_ConverterColorSettings;

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
                {
                    switch (m_ConverterColorSettings)
                    {
                        case ConverterColorSettings.RGB:
                            oldColors[i] = FindNearestColorInPaletteRGB(oldColors[i]);
                            break;
                        case ConverterColorSettings.HSV:
                            oldColors[i] = FindNearestColorInPaletteHSV(oldColors[i]);
                            break;
                        default:
                            break;
                    }
                }
            }

            newTex.SetPixels(oldColors);
#if UNITY_EDITOR
            byte[] bytes = newTex.EncodeToPNG();
            FileStream stream = new FileStream(Application.dataPath + "newTex_" + m_ConverterColorSettings.ToString() + ".png", FileMode.OpenOrCreate, FileAccess.Write);
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
        public Color FindNearestColorInPaletteRGB(Color src)
        {
            if (src.a < .1f)
                return new Color(1f, 1f, 1f, 0f);
            float minimumDistance = 255f * 255f + 255f * 255f + 255f * 255f + 1f;

            int closestColorID = 0;

            for (int i = 0; i < m_ColorsInPalette.Length; i++)
            {
                float distH = Mathf.Abs(m_ColorsInPalette[i].r - src.r);
                float distS = Mathf.Abs(m_ColorsInPalette[i].g - src.g);
                float distV = Mathf.Abs(m_ColorsInPalette[i].b - src.b);

                float distance = (distH * distH) + (distS * distS) + (distV * distV);

                if (distance < minimumDistance)
                    closestColorID = i;
            }

            return m_ColorsInPalette[closestColorID];
        }

        public Color FindNearestColorInPaletteHSV(Color src)
        {
            if (src.a < .1f)
                return new Color(1f, 1f, 1f, 0f);
            float minimumDistance = 255f * 255f + 255f * 255f + 255f * 255f + 1f;

            float H = 0f;
            float S = 0f;
            float V = 0f;
            Color.RGBToHSV(src, out H, out S, out V);

            int closestColorID = 0;

            for (int i = 0; i < m_ColorsInPalette.Length; i++)
            {
                float paletteH = 0f;
                float paletteS = 0f;
                float paletteV = 0f;

                Color.RGBToHSV(m_ColorsInPalette[i], out paletteH, out paletteS, out paletteV);

                float distH = Mathf.Abs(paletteH - H);
                float distS = Mathf.Abs(paletteS - S);
                float distV = Mathf.Abs(paletteV - V);

                float distance = (distH * distH) * m_HueWeight + (distS * distS) * m_SaturationWeight + (distV * distV) * m_ValueWeight;

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
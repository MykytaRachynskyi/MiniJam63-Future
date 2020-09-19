using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Future
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] SpriteRenderer m_SpriteRenderer;

        public delegate void OnTappedCallback(int coordX, int coordY, int id);
        OnTappedCallback m_OnTappedCallback;

        int m_CoordX;
        int m_CoordY;
        int m_ID;

        public void Init(Sprite sprite, int ID, int coordX, int coordY, OnTappedCallback onTappedCallback)
        {
            m_CoordX = coordX;
            m_CoordY = coordY;
            m_ID = ID;

            m_SpriteRenderer.sprite = sprite;
            m_OnTappedCallback = onTappedCallback;
        }

        public Vector2 GetCoordinates()
        {
            return new Vector2(m_CoordX, m_CoordY);
        }

        public int GetID()
        {
            return m_ID;
        }

        public bool IsVisible()
        {
            return m_SpriteRenderer.sprite != null;
        }

        public void PlayMoveDownAnimation(int spaces)
        {
            Debug.Log(string.Format("I {0} WAS TOLD TO MOVE {1} SPACES DOWN", GetCoordinates().ToString(), spaces));
        }

        public void SetInvisible()
        {
            m_SpriteRenderer.sprite = null;
        }

        public void OnTapped()
        {
            m_OnTappedCallback?.Invoke(m_CoordX, m_CoordY, m_ID);
        }

        private void OnMouseUpAsButton()
        {
            OnTapped();
        }
    }
}
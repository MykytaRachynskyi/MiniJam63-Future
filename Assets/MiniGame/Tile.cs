using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Future
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] SpriteRenderer m_SpriteRenderer;
        [SerializeField] float m_FallTimeInSeconds;

        public delegate void OnTappedCallback(int coordX, int coordY, int id);
        OnTappedCallback m_OnTappedCallback;

        public delegate void OnAnimationFinishedCallback(Tile tile);

        Coroutine m_FallAnimation;

        Coroutine m_InitFallAnimation;

        int m_CoordX;
        int m_CoordY;
        int m_ID;

        Vector3 m_InitialPosition;

        bool m_Interactable = false;

        public void Init(Sprite sprite, int ID, int coordX, int coordY, OnTappedCallback onTappedCallback)
        {
            m_CoordX = coordX;
            m_CoordY = coordY;
            m_ID = ID;

            m_SpriteRenderer.sprite = sprite;
            m_OnTappedCallback = onTappedCallback;

            m_InitialPosition = this.transform.position;

            m_Interactable = true;
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

        public void SetInteractable(bool interactable)
        {
            m_Interactable = interactable;
        }

        public void PlayMoveDownAnimation(int spaces, float distance, Tile coveredTile, OnAnimationFinishedCallback callback)
        {
            if (spaces == 0)
                return;

            if (m_FallAnimation != null)
                StopCoroutine(m_FallAnimation);

            m_FallAnimation = StartCoroutine(MoveAnimation(spaces, distance, coveredTile, callback));
        }

        public void InitPlayMoveDownAnimation(int spaces, float distance, OnAnimationFinishedCallback callback)
        {
            if (spaces == 0)
                return;

            if (m_InitFallAnimation != null)
                StopCoroutine(m_InitFallAnimation);

            m_InitFallAnimation = StartCoroutine(InitMoveAnimation(spaces, distance, callback));
        }

        IEnumerator MoveAnimation(int spaces, float downwardDistance, Tile coveredTile, OnAnimationFinishedCallback callback)
        {
            float progress = 0f;

            Vector3 currentPos = this.transform.position;
            Vector3 targetPos = currentPos - new Vector3(0f, spaces * downwardDistance, 0f);

            while (progress < 1f)
            {
                progress += Time.deltaTime / m_FallTimeInSeconds;
                this.transform.position = Vector3.Lerp(currentPos, targetPos, progress);
                yield return null;
            }

            if (coveredTile != null)
                coveredTile.SetSprite(this.m_SpriteRenderer.sprite, m_ID);

            SetInvisible();

            this.transform.position = m_InitialPosition;
            yield return null;

            callback?.Invoke(this);
        }

        IEnumerator InitMoveAnimation(int spaces, float downwardDistance, OnAnimationFinishedCallback callback)
        {
            SetInteractable(false);
            float progress = 0f;

            Vector3 currentPos = this.transform.position;
            Vector3 targetPos = currentPos - new Vector3(0f, spaces * downwardDistance, 0f);

            while (progress < 1f)
            {
                progress += Time.deltaTime / m_FallTimeInSeconds;
                this.transform.position = Vector3.Lerp(currentPos, targetPos, progress);
                yield return null;
            }
            yield return null;

            m_InitialPosition = this.transform.position;

            callback?.Invoke(this);
        }

        public void SetSprite(Sprite sprite, int ID)
        {
            m_SpriteRenderer.sprite = sprite;
            m_ID = ID;
        }

        public void SetInvisible()
        {
            m_SpriteRenderer.sprite = null;
        }

        public void OnTapped()
        {
            if (!m_Interactable)
                return;

            m_OnTappedCallback?.Invoke(m_CoordX, m_CoordY, m_ID);
        }

        private void OnMouseUpAsButton()
        {
            OnTapped();
        }
    }
}
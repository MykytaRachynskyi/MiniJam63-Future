﻿using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Future
{
    public class Notification : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] RectTransform m_ThisRectTransform;
        [SerializeField] CanvasGroup m_CanvasGroup;
        [SerializeField] TMPro.TextMeshProUGUI m_Text;

        [SerializeField] AnimationCurve m_AnimationCurve;

        [SerializeField] Vector3 m_HiddenPos;
        [SerializeField] Vector3 m_ShownPos;

        [SerializeField] Vector3 m_SwipeAwayPositionLeft;
        [SerializeField] Vector3 m_SwipeAwayPositionRight;

        [SerializeField] float m_MoveTimeInSeconds;
        [SerializeField] float m_MoveTimeInSecondsAfterEndDrag;

        [SerializeField] float m_SwipeAwayMargin;
        [SerializeField] float m_SwipeAwayTimeInSeconds;

        [SerializeField] IntUnityEvent m_OnSwipeAway;

        delegate void MoveCallback();

        bool m_IsSwipeable = false;

        Coroutine m_MovementAnimation;
        Coroutine m_SwipeAwayAnimation;

        Vector2 m_MousePosAtLastDrag = Vector2.zero;

        int m_NotificationID = 0;

        public void SetNotificationText(string text)
        {

        }

        [ContextMenu("Show notification")]
        public void Show()
        {
            StopMovementAnimation();

            m_MovementAnimation = StartCoroutine(MoveAnimation(m_HiddenPos, m_ShownPos, m_MoveTimeInSeconds, null, () =>
            {
                m_IsSwipeable = true;
            }));
        }

        [ContextMenu("Hide notification")]
        public void Hide()
        {
            StopMovementAnimation();

            m_MovementAnimation = StartCoroutine(MoveAnimation(m_ShownPos, m_HiddenPos, m_MoveTimeInSeconds, () =>
            {
                m_IsSwipeable = false;
            }));
        }

        void StopMovementAnimation()
        {
            if (m_MovementAnimation != null)
                StopCoroutine(m_MovementAnimation);
        }

        IEnumerator MoveAnimation(Vector3 startPos, Vector3 endPos, float moveTimeInSeconds,
                                  MoveCallback onMoveStart = null, MoveCallback onMoveEnd = null)
        {
            onMoveStart?.Invoke();

            float progress = 0f;
            while (progress < 1f)
            {
                progress += Time.deltaTime / moveTimeInSeconds;
                m_ThisRectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, m_AnimationCurve.Evaluate(progress));

                yield return null;
            }

            m_ThisRectTransform.anchoredPosition = endPos;

            onMoveEnd?.Invoke();
        }

        public void SwipeAway(bool goingLeft)
        {
            if (m_SwipeAwayAnimation != null)
                StopCoroutine(m_SwipeAwayAnimation);

            m_SwipeAwayAnimation = StartCoroutine(SwipeAwayAnimation(goingLeft));
        }

        IEnumerator SwipeAwayAnimation(bool goingLeft)
        {
            float progress = 0f;

            Vector3 startPos = m_ThisRectTransform.anchoredPosition;
            Vector3 targetPos = goingLeft ? m_SwipeAwayPositionLeft : m_SwipeAwayPositionRight;

            while (progress < 1f)
            {
                progress += Time.deltaTime / m_SwipeAwayTimeInSeconds;
                m_ThisRectTransform.anchoredPosition = Vector3.Lerp(startPos, targetPos, progress);
                m_CanvasGroup.alpha = 1f - progress;

                yield return null;
            }

            m_CanvasGroup.alpha = 0f;
            m_OnSwipeAway?.Invoke(goingLeft ? 0 : 1);

            yield return null;

            m_ThisRectTransform.anchoredPosition = m_HiddenPos;
            m_CanvasGroup.alpha = 1f;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!m_IsSwipeable)
                return;

            m_MousePosAtLastDrag = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!m_IsSwipeable)
                return;

            m_ThisRectTransform.anchoredPosition += new Vector2((eventData.position - m_MousePosAtLastDrag).x, 0f) / 80f;

            m_MousePosAtLastDrag = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            StopMovementAnimation();

            if (Mathf.Abs(m_ThisRectTransform.anchoredPosition.x - m_ShownPos.x) > m_SwipeAwayMargin)
            {
                bool goingLeft = Mathf.Sign(m_ThisRectTransform.anchoredPosition.x - m_ShownPos.x) < 0f;
                Debug.Log(string.Format("Swiping {0}!",
                    goingLeft ? "Left" : "Right"));

                m_IsSwipeable = false;
                SwipeAway(goingLeft);
            }
            else
            {
                m_MovementAnimation = StartCoroutine(
                    MoveAnimation(m_ThisRectTransform.anchoredPosition, m_ShownPos, m_MoveTimeInSecondsAfterEndDrag));
            }
        }
    }
}
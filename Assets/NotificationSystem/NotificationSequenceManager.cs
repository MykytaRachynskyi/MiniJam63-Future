using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Future
{
    public class NotificationSequenceManager : MonoBehaviour
    {
        [SerializeField] NotificationSystemItem[] m_ItemSequence;
        [SerializeField] Notification m_Notification;
        [SerializeField] float m_StandardNotificationDelay;

        int m_CurrentSequenceID;

        WaitForSeconds m_StandardDelay;
        Dictionary<int, bool> m_ChoiceMap = new Dictionary<int, bool>();

        Coroutine m_DelayedShowNotification;

        public void StartSequence()
        {
            m_CurrentSequenceID = 0;
            SetNextNotification();
        }

        public void OnNotificationSwipe(bool swipedLeft)
        {
            var sequenceItem = m_ItemSequence[m_CurrentSequenceID];
            if (sequenceItem.RequiresChoice)
            {
                sequenceItem.ChoiceMade = swipedLeft;
            }

            m_CurrentSequenceID++;
            SetNextNotification();
        }

        void SetNextNotification()
        {
            if (m_DelayedShowNotification != null)
                StopCoroutine(m_DelayedShowNotification);

            m_Notification.SetNotification(m_ItemSequence[m_CurrentSequenceID]);
            m_DelayedShowNotification = StartCoroutine(DelayedShowNotification());
        }

        IEnumerator DelayedShowNotification()
        {
            if (m_StandardDelay == null)
                m_StandardDelay = new WaitForSeconds(m_StandardNotificationDelay);

            yield return m_StandardDelay;

            m_Notification.Show();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Future
{
    [CreateAssetMenu(menuName = "Notifications/NotificationItem_")]
    public class NotificationSystemItem : ScriptableObject
    {
        [SerializeField] int m_ID;
        public int ID { get { return m_ID; } }

        [SerializeField] string m_NotificationText;
        public string NotificationText { get { return m_NotificationText; } }

        [SerializeField] string m_NotificationTextIfPrecedentLeft;
        public string NotificationTextIfPrecedentLeft { get { return m_NotificationTextIfPrecedentLeft; } }

        [SerializeField] string m_NotificationTextIfPrecedentRight;
        public string NotificationTextIfPrecedentRight { get { return m_NotificationTextIfPrecedentRight; } }

        [SerializeField] bool m_RequiresChoice;
        public bool RequiresChoice { get { return m_RequiresChoice; } }

        [SerializeField] string m_LeftOptionText;
        public string LeftOptionText { get { return m_LeftOptionText; } }
        [SerializeField] string m_RightOptionText;
        public string RightOptionText { get { return m_RightOptionText; } }

        [SerializeField] NotificationSystemItem m_PrecedentItem;
        public NotificationSystemItem PrecedentItem { get { return m_PrecedentItem; } }

        [SerializeField] bool m_ChoiceMade;
        public bool ChoiceMade
        {
            get
            {
                return m_ChoiceMade;
            }
            set
            {
                m_ChoiceMade = value;
                m_ChoiceMadeString = value ? m_LeftOptionText : m_RightOptionText;
            }
        }

        [SerializeField] string m_ChoiceMadeString;

        [SerializeField] UnityEvent m_OnNotificationProcessed;
        [SerializeField] UnityEvent m_OnNotificationProcessedIfPrecedentLeft;
        [SerializeField] UnityEvent m_OnNotificationProcessedIfPrecedentRight;

        [SerializeField] UnityEvent m_OnChosenLeft;
        [SerializeField] UnityEvent m_OnChosenRight;

        public void ProcessItem()
        {
            if (PrecedentItem != null)
            {
                if (PrecedentItem.ChoiceMade)
                {
                    m_OnNotificationProcessedIfPrecedentLeft?.Invoke();
                }
                else
                {
                    m_OnNotificationProcessedIfPrecedentRight?.Invoke();
                }
            }
            else
            {
                m_OnNotificationProcessed?.Invoke();
            }
        }
    }
}
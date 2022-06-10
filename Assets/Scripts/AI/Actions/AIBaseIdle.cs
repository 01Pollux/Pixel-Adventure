using System.Collections;
using UnityEngine;

namespace AI.Actions
{
    public class AIBaseIdle : BaseAIAction
    {
        [SerializeField] private float m_IdleDuration = 1.5f;

        [Header("States")]
        [SerializeField] private AIBasePatrol m_PatrolState;

        private float m_IdleTime;


        private void Awake()
        {
            this.AllowUpdates = !m_StartupDisabled;
        }

        private void OnEnable()
        {
            m_IdleTime = m_IdleDuration;
        }


        public override bool Transition()
        {
            if (m_IdleTime >= 0f)
            {
                m_IdleTime -= Time.deltaTime;
                return false;
            }

            m_PatrolState.AllowUpdates = true;
            return true;
        }
    }
}
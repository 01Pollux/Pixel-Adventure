using System.Collections.Generic;
using UnityEngine;

namespace AI.Actions
{
    public class AIBasePatrol : BaseAIAction
    {
        [SerializeField] private List<Vector2> m_PatrolPositions;
        [SerializeField] private int m_PatrolIndex;

        [SerializeField] private Vector2 m_PatrolSpeed = new(6.4f, 7f);
        [SerializeField] private float m_ArriveDistance = 0.4f;

        [Header("States")]
        [SerializeField] private AIBaseIdle m_IdleState;


        private bool m_ReachedDestination;
        private bool m_PostFirstTransition;

        private EnemyAIController m_Controller;


        private void Awake()
        {
            this.AllowUpdates = !m_StartupDisabled;
            m_Controller = StateHolder;
        }


        private void OnEnable()
        {
            var animator = m_Controller.AnimManager.Animator;
            animator.SetBool("Is Idling", true);
            // TODO remove
            animator.SetBool("Is Awake", true);

            if (m_PostFirstTransition)
                m_Controller.FlipSprite();
        }

        private void OnDisable()
        {
            
        }


        private void FixedUpdate()
        {
            if (m_ReachedDestination)
                return;

            Vector2 patrol_dir = (m_PatrolPositions[m_PatrolIndex] - m_Controller.Position).normalized;
            m_Controller.Velocity = patrol_dir * m_PatrolSpeed;
        }


        private void Update()
        {
            if (m_ReachedDestination)
                return;

            if (Vector2.Distance(m_Controller.Position, m_PatrolPositions[m_PatrolIndex]) < m_ArriveDistance)
            {
                m_Controller.Velocity = Vector2.zero;
                m_ReachedDestination = true;
            }
        }

        public override bool Transition()
        {
            // if we reached the destination, just idle for few moments
            if (m_ReachedDestination)
            {
                m_ReachedDestination = false;
                m_PatrolIndex = (m_PatrolIndex + 1) % m_PatrolPositions.Count;

                m_IdleState.AllowUpdates = true;
                m_PostFirstTransition = true;

                return true;
            }

            return false;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < m_PatrolPositions?.Count - 1; i++)
            {
                Gizmos.DrawLine(
                    m_PatrolPositions[i],
                    m_PatrolPositions[i + 1]
                );
            }
        }
    }
}
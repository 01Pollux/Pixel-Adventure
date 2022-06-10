using UnityEngine;

namespace AI.Actions
{
    public class BaseAIAction : MonoBehaviour
    {
        [SerializeField] protected bool m_StartupDisabled;
        [SerializeField] protected int m_Priority;

        public int Priority => m_Priority;

        private bool m_AllowUpdates;

        public bool OnlyDisableUpdates
        {
            get;
            private set;
        }

        public bool AllowUpdates
        {
            get
            {
                return OnlyDisableUpdates ? m_AllowUpdates : this.enabled;
            }
            set
            {
                if (OnlyDisableUpdates)
                    m_AllowUpdates = value;
                else
                    this.enabled = value;
            }
        }

        public EnemyAIController StateHolder => GetComponentInParent<EnemyAIController>();


        public BaseAIAction(bool only_disable_updates = false) => OnlyDisableUpdates = only_disable_updates;

        public virtual bool Transition() => throw new System.NotImplementedException();
    }
}
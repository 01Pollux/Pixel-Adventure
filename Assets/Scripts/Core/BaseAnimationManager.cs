using UnityEngine;

namespace Core
{
    public class BaseAnimationManager
    {
        private readonly Animator m_Animator;

        public BaseAnimationManager(Animator animator) => m_Animator = animator;


        public Animator @Animator => m_Animator;

        public Vector2 WalkXY
        {
            set { WalkX = value.x; WalkY = value.y; }
        }
        public float WalkX
        {
            set => @Animator.SetFloat("WalkX", value);
        }
        public float WalkY
        {
            set => @Animator.SetFloat("WalkY", value);
        }

        public bool WasHit
        {
            set { if (value) @Animator.SetTrigger("WasHit"); }
        }


        public void TriggerHurt() => m_Animator.SetTrigger("Hurt");
        public void TriggerDeath() => m_Animator.SetTrigger("Die");
    }
}

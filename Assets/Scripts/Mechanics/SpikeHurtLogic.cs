using UnityEngine;
using Interfaces;

namespace Mechanics
{
    public class SpikeHurtLogic :
        MonoBehaviour,
        IDamageInfo
    {
        [SerializeField] private LayerMask m_TargetLayer;
        [SerializeField] private int m_Damage = 5;
        [SerializeField] private float m_KnockbackMod = 3;


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if ((m_TargetLayer & 1 << collision.collider.gameObject.layer) == 0)
                return;

            if (collision.collider.TryGetComponent(out IHealth health))
                health.TakeDamage(this);
        }


        public GameObject parentObj => gameObject;

        public int Damage => m_Damage;

        public float KnockbackMod => m_KnockbackMod;
    }
}
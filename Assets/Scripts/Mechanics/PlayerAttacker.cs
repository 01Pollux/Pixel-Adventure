using Interfaces;
using System.Collections;
using UnityEngine;

namespace Mechanics
{
    public class PlayerAttacker :
        MonoBehaviour,
        IDamageInfo,
        IHealth
    {
        [Header("Attack field")]
        [SerializeField] private int m_StompDamage = 10;
        [SerializeField] private float m_KnockbackMod = 5f;
        [SerializeField] private LayerMask m_AttackerLayer;

        [Header("Health field")]
        [SerializeField] private int m_MaxHealth;
        [SerializeField] private float m_DamageImmunityCooldown;

        [Header("Spawn field")]
        [SerializeField] private float m_RespawnTime = 2f;
        [SerializeField] private Vector2 m_SpawnPosition;


        private Animator m_Animator;
        private PlayerController m_Controller;
        private Rigidbody2D m_Rigidbody;


        private void Awake()
        {
            m_Animator = GetComponentInChildren<Animator>();
            m_Controller = GetComponent<PlayerController>();
            m_Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start() => Respawn();


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if ((m_AttackerLayer & (1 << collision.gameObject.layer)) != 0)
            {
                // If we are stomping the enemy
                if (collision.contacts[0].normal == (Vector2)transform.up)
                {
                    if (collision.gameObject.TryGetComponent<IHealth>(out var health))
                        health.TakeDamage(this);
                }
                else if (collision.gameObject.TryGetComponent<IDamageInfo>(out var damage))
                    this.TakeDamage(damage);
            }
        }


        private IEnumerator CoOnPlayerDeath()
        {
            m_Animator.SetTrigger("Die");
            m_Controller.enabled = false;
            m_Rigidbody.simulated = false;

            EventSystem.GameMessenger.RaiseAsync(EventSystem.EGameEvent.PlayerDeath, this);
            yield return new WaitForSeconds(m_RespawnTime);
            EventSystem.GameMessenger.RaiseAsync(EventSystem.EGameEvent.PlayerRespawn, this);

            m_Rigidbody.simulated = true;
            m_Animator.SetTrigger("Respawn");
            Respawn();
        }


        private void Respawn()
        {
            m_Controller.enabled = true;
            m_Rigidbody.velocity = Vector2.zero;

            Health = m_MaxHealth;
            gameObject.transform.position = m_SpawnPosition;
        }


        // IDamageInfo
        GameObject IDamageInfo.parentObj => gameObject;
        int IDamageInfo.Damage => m_StompDamage;
        float IDamageInfo.KnockbackMod => m_KnockbackMod;


        // IHealth
        public int Health { get; set; }
        public int MaxHealth => m_MaxHealth;
        public float CurrentDamageCooldown { get; set; }
        public float DamageCooldown => m_DamageImmunityCooldown;

        void IHealth.OnTakeDamage(IDamageInfo damage_info)
        {
        }

        void IHealth.OnTakeDamageFatal(IDamageInfo damage_info)
        {
            StartCoroutine(CoOnPlayerDeath());
        }
    }
}
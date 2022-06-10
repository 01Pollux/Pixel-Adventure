using System.Linq;
using System.Collections.Generic;
using Interfaces;
using AI.Actions;
using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class EnemyAIController : 
        MonoBehaviour,
        IDamageInfo,
        IHealth
    {
        #region Variables
        [Header("Stats")]
        [SerializeField] private int m_Damage = 15;
        [SerializeField] private float m_DamageCooldown = 0.7f;
        [SerializeField] private float m_KnockbackMod;

        [SerializeField] private int m_MaxHealth = 40;

        private List<BaseAIAction> m_Actions;


        public Core.BaseMoveable Moveable
        {
            get;
            private set;
        }

        public Core.BaseAnimationManager AnimManager
        {
            get;
            private set;
        }

        public SpriteRenderer @SpriteRenderer
        {
            get;
            private set;
        }

        public Vector2 Velocity
        {
            get => Moveable.Rigidbody.velocity;
            set => Moveable.Rigidbody.velocity = value;
        }

        public Vector2 Position
        {
            get => Moveable.Rigidbody.position;
            set => Moveable.Rigidbody.position = value;
        }
        #endregion


        private void Awake()
        {
            Moveable = new(GetComponent<Rigidbody2D>());
            AnimManager = new(GetComponent<Animator>());
            @SpriteRenderer = GetComponent<SpriteRenderer>();
            
            m_Health = m_MaxHealth;

            m_Actions = GetComponentsInChildren<BaseAIAction>().ToList();
            m_Actions.Sort(
                (a, b) =>
                {
                    return a.Priority - b.Priority switch
                    {
                        0 => 0,
                        >0 => 1,
                        _ => -1
                    };
                }
            );
        }

        private void Update()
        {
            foreach (var action in m_Actions)
            {
                if (action.AllowUpdates && action.Transition())
                    action.AllowUpdates = false;
            }
        }


        public void FlipSprite()
        {
            @SpriteRenderer.flipX = !@SpriteRenderer.flipX;
        }


        #region Interfaces Implementation
        private int m_Health;
        public int Health
        {
            get => m_Health;
            set => m_Health = value;
        }
        
        public int MaxHealth => m_MaxHealth;


        public GameObject parentObj => gameObject;

        public int Damage => m_Damage;

        public float KnockbackMod => m_KnockbackMod;

        private float m_CurrentDamageCooldow;
        public float CurrentDamageCooldown { get => m_CurrentDamageCooldow; set => m_CurrentDamageCooldow = value; }

        public float DamageCooldown => m_DamageCooldown;


        public void OnTakeDamage(IDamageInfo damage_info)
        {
        }

        public void OnTakeDamageFatal(IDamageInfo damage_info)
        {
            gameObject.SetActive(false);
        }
        #endregion
    }
}
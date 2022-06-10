using Core;
using Helpers;
using Interfaces;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace Mechanics
{
    public class LocalPlayerController :
        MonoBehaviour,
        IDamageInfo,
        IHealth
    {
        #region Variables
        [Header("Movements")]
        [SerializeField] private float m_Speed = 400f;
        [SerializeField] private LayerMask m_GroundLayer;

        [SerializeField] private float m_JumpForce = 15f;
        [SerializeField] private int m_ExtraJumps = 1;


        [Header("Stats")]
        [SerializeField] private int m_Damage = 30;
        [SerializeField] private float m_DamageCooldown = 0.8f;
        [SerializeField] private float m_KnockbackMod = 0f;

        [SerializeField] private int m_MaxHealth = 50;
        [SerializeField] private float m_EnemyStompBounce = 15f;


        [Header("UI")]
        [SerializeField] private TextMeshProUGUI m_FruitText;


        private SpriteRenderer m_SpriteRenderer;
        private BaseMoveable m_Moveable;
        private BaseAnimationManager m_AnimationManager;


        private int m_CurJumps;
        private bool m_IsClimbing;


        private BoxDirCollision m_BoxDirCollision;
        private int m_EnemyLayer;

        private int m_Fruits;

        #endregion


        #region Initialisation
        private void Awake()
        {
            m_Moveable = new(GetComponent<Rigidbody2D>());
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_AnimationManager = new(GetComponent<Animator>());
            m_BoxDirCollision = new(GetComponent<BoxCollider2D>(), transform, m_GroundLayer);

            m_Health = m_MaxHealth;
            m_EnemyLayer = LayerMask.GetMask("Enemy");
        }

        private void OnEnable()
        {
            var gameplay = GameManager.Instance.InputSystem.Gameplay;
            gameplay.Jump.started += OnJumpBegin;
            gameplay.Jump.canceled += OnJumpEnd;
        }

        private void OnDisable()
        {
            var gameplay = GameManager.Instance.InputSystem.Gameplay;
            gameplay.Jump.started -= OnJumpBegin;
            gameplay.Jump.canceled -= OnJumpEnd;
        }
        #endregion


        private void FixedUpdate()
        {
            if (!m_IsClimbing)
                m_Moveable.XVelocity = GetMovementDirection() * m_Speed * Time.fixedDeltaTime;
        }

        private void Update()
        {
            m_IsClimbing = false;
            m_BoxDirCollision.Update();

            var jump = GameManager.Instance.InputSystem.Gameplay.Jump;

            if ((m_BoxDirCollision.touchType & (Helpers.TouchType.Bottom)) != 0)
            {
                if (!jump.inProgress)
                    m_CurJumps = 0;
            }
            else if ((m_BoxDirCollision.touchType & (Helpers.TouchType.LeftRight)) != 0)
            {
                HandlePlayerClimb();
                if (m_IsClimbing && !jump.inProgress)
                    m_Moveable.YVelocity = 0f;
                return;
            }

            float direction = GetMovementDirection();
            if (direction != 0f)
                m_SpriteRenderer.flipX = direction < 0f;
        }

        private void LateUpdate()
        {
            m_AnimationManager.WalkXY = m_Moveable.Velocity.normalized;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Fruits"))
            {
                IFruit fruit = collision.gameObject.GetComponent<IFruit>();
                fruit.OnTouch(this);
                m_FruitText.text = $"Fruits: {++m_Fruits}";
            }
        }


        #region Jump and collisions
        private void OnJumpBegin(InputAction.CallbackContext action)
        {
            if (m_CurJumps > m_ExtraJumps)
                return;

            if (!m_IsClimbing)
            {
                ++m_CurJumps;
                m_Moveable.YVelocity = m_JumpForce;
            }
        }

        private void OnJumpEnd(InputAction.CallbackContext action)
        {
            if (m_Moveable.YVelocity >= 0f)
                m_Moveable.YVelocity = 0f;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            HandlePlayerToEnemyCollision(collision);
        }
        #endregion


        #region Helpers
        private void HandlePlayerToEnemyCollision(Collision2D collision)
        {
            if ((m_EnemyLayer & (1 << collision.collider.gameObject.layer)) == 0)
                return;

            // if we did stomp the enemy
            if (Vector2.Dot(transform.up, collision.contacts[0].normal) == 1)
            {
                if (collision.collider.gameObject.TryGetComponent<IHealth>(out var health))
                    health.TakeDamage(this);
                m_Moveable.YVelocity += m_EnemyStompBounce;
            }
        }


        private float GetMovementDirection()
        {
            var gameplay = GameManager.Instance.InputSystem.Gameplay;
            return gameplay.Movement.ReadValue<float>();
        }


        private void HandlePlayerClimb()
        {
            m_IsClimbing = GameManager.Instance.InputSystem.Gameplay.WallClimb.inProgress;

            m_AnimationManager.Animator.SetBool("WallSlide", m_IsClimbing);
            m_Moveable.Rigidbody.gravityScale = m_IsClimbing ? 0f : 3.5f;

            if (m_IsClimbing)
            {
                if ((m_BoxDirCollision.touchType & Helpers.TouchType.Left) != 0)
                    m_SpriteRenderer.flipX = true;
                else
                    m_SpriteRenderer.flipX = false;
            }
        }
        #endregion


        #region Interfaces Implementation
        public GameObject parentObj => gameObject;

        public int Damage => m_Damage;

        public float KnockbackMod => m_KnockbackMod;

        private int m_Health;
        public int Health
        {
            get => m_Health;
            set => m_Health = value;
        }

        public int MaxHealth => m_MaxHealth;

        private float m_CurrentDamageCooldow;
        public float CurrentDamageCooldown { get => m_CurrentDamageCooldow; set => m_CurrentDamageCooldow = value; }

        public float DamageCooldown => m_DamageCooldown;


        public void OnTakeDamage(IDamageInfo damage_info)
        {
            m_AnimationManager.TriggerHurt();
        }

        public void OnTakeDamageFatal(IDamageInfo damage_info)
        {
            m_AnimationManager.TriggerDeath();
        }
        #endregion
    }
}
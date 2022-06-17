using Helpers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchType = Helpers.TouchType;

namespace Mechanics
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movements")]
        [SerializeField] private LayerMask m_GroundLayer;

        [SerializeField] private float m_JumpPower = 15f;
        [SerializeField] private float m_MoveSpeed = 400f;
        [SerializeField] private float m_DashSpeed = 600f;
        [SerializeField] private float m_WallJumpLerp = 20f;

        [SerializeField] private float m_ClimbSpeed = 150f, m_SlideSpeed = 275f;

        [SerializeField] private int m_NumberOfJumps = 2;
        [SerializeField] private int m_NumberOfDahes = 2;
        [SerializeField] private float m_Radius = 0.04f;

        [Header("Effects")]
        [SerializeField] private ParticleSystem m_DashEffect;
        [SerializeField] private ParticleSystem m_JumpEffect;
        [SerializeField] private ParticleSystem m_WallJumpEffect;
        [SerializeField] private ParticleSystem m_SlideEffect;


        private MoveState m_MoveState;
        private int m_CurJumps;
        private int m_CurDashes;


        private Rigidbody2D m_Rigidbody;
        private Animator m_Animator;
        private SpriteRenderer m_SpriteRenderer;
        private CircleDirCollision m_DirCollision;


        private static Core.LocalInputActions.GameplayActions GameplayInput => Core.GameManager.GetInputManager().Gameplay;

        private bool IsTouchingWall => m_DirCollision.touchType.HasFlag(TouchType.Left) | m_DirCollision.touchType.HasFlag(TouchType.Right);
        private bool IsTouchingFloor => m_DirCollision.touchType.HasFlag(TouchType.Bottom);


        private void Awake()
        {
            m_Rigidbody         = GetComponent<Rigidbody2D>();
            m_Animator          = GetComponentInChildren<Animator>();
            m_SpriteRenderer    = GetComponentInChildren<SpriteRenderer>();
            m_DirCollision      = new(GetComponent<BoxCollider2D>(), m_Radius, m_GroundLayer);
        }


        private void OnEnable()
        {
            m_CurDashes = m_CurJumps = 0;
            m_MoveState = 0;

            var gp = GameplayInput;
            gp.Jump.started += OnPlayerBeginJump;
            gp.Jump.canceled += OnPlayerEndJump;

            gp.Dash.started += OnPlayerTryToDash;

            gp.WallClimb.started += OnPlayerBeginWallGrab;
            gp.WallClimb.canceled += OnPlayerEndWallGrab; ;
        }

        private void OnDisable()
        {
            var gp = GameplayInput;
            gp.Jump.started -= OnPlayerBeginJump;
            gp.Jump.canceled -= OnPlayerEndJump;

            gp.Dash.started -= OnPlayerTryToDash;

            gp.WallClimb.started -= OnPlayerBeginWallGrab;
            gp.WallClimb.canceled -= OnPlayerEndWallGrab;
        }


        private void OnPlayerBeginJump(InputAction.CallbackContext ctx)
        {
            if (!enabled)
                return;

            m_MoveState |= MoveState.Jumping;
            PlayerTryJump();
        }

        private void OnPlayerEndJump(InputAction.CallbackContext ctx)
        {
            if (!enabled)
                return;

            m_MoveState &= ~MoveState.Jumping;
            if (!m_MoveState.HasFlag(MoveState.Dashing))
            {
                if (m_Rigidbody.velocity.y > 0f)
                    m_Rigidbody.velocity = new(m_Rigidbody.velocity.x, 0f);
            }
        }


        private void OnPlayerTryToDash(InputAction.CallbackContext ctx)
        {
            if (!enabled)
                return;

            if (m_MoveState.HasFlag(MoveState.Dashing) ||
                m_CurDashes >= m_NumberOfDahes ||
                GameplayInput.Movement.ReadValue<Vector2>() == Vector2.zero)
                return;

            StartCoroutine(CoPlayerTryToDash());
        }


        private void OnPlayerBeginWallGrab(InputAction.CallbackContext ctx)
        {
            if (!enabled)
                return;
            if (IsTouchingWall)
            {
                m_Rigidbody.gravityScale = 0f;
                m_MoveState |= MoveState.WallClimbing;

                float movex = GameplayInput.Movement.ReadValue<Vector2>().x;
                if (movex > 0f)
                    m_SpriteRenderer.flipX = false;
                else if (movex < 0f)
                    m_SpriteRenderer.flipX = true;
            }
        }

        private void OnPlayerEndWallGrab(InputAction.CallbackContext ctx)
        {
            if (!enabled)
                return;
            if (m_MoveState.HasFlag(MoveState.WallClimbing))
            {
                m_Rigidbody.gravityScale = 3.5f;
                m_MoveState &= ~MoveState.WallClimbing;
            }
        }


        private void FixedUpdate()
        {
            m_DirCollision.Update();

            var movexy = GetRawMoveInput();

            if (m_MoveState.HasFlag(MoveState.WallClimbing))
            {
                m_Rigidbody.velocity = Vector2.zero;
            }

            if (m_MoveState.HasFlag(MoveState.Dashing))
            {
                m_Rigidbody.velocity = m_DashSpeed * Time.fixedDeltaTime * movexy;
            }
            else if (!m_MoveState.HasFlag(MoveState.WallClimbing))
            {
                if (!m_MoveState.HasFlag(MoveState.NoMove))
                {
                    Vector2 new_speed = new(movexy.x * m_MoveSpeed * Time.fixedDeltaTime, m_Rigidbody.velocity.y);
                    if (m_MoveState.HasFlag(MoveState.WallJumping))
                        new_speed = Vector2.Lerp(m_Rigidbody.velocity, new_speed, m_WallJumpLerp * Time.fixedDeltaTime);
                    m_Rigidbody.velocity = new_speed;
                }
            }
            else
            {
                if (movexy.y != 0f)
                {
                    float climb_slide = movexy.y >= 0 ? m_ClimbSpeed : -m_SlideSpeed;
                    m_Rigidbody.velocity += new Vector2(m_Rigidbody.velocity.x, climb_slide * Time.fixedDeltaTime);

                    if (movexy.y < 0f)
                    {
                        m_SlideEffect.transform.localScale = new(m_DirCollision.touchType.HasFlag(TouchType.Left) ? 1f : -1f, 1f, 1f);
                        m_SlideEffect.transform.position = m_Rigidbody.position;
                        m_SlideEffect.Play();
                    }
                }
            }
        }



        private void Update()
        {
            if (IsTouchingFloor)
            {
                if (!m_MoveState.HasFlag(MoveState.Jumping))
                    m_CurJumps = 0;
                if (!m_MoveState.HasFlag(MoveState.Dashing))
                    m_CurDashes = 0;

                m_MoveState &= ~MoveState.WallJumping;
            }

            if (!m_MoveState.HasFlag(MoveState.WallClimbing))
            {
                var movexy = GameplayInput.Movement.ReadValue<Vector2>();
                if (movexy.x > 0f)
                    m_SpriteRenderer.flipX = false;
                else if (movexy.x < 0f)
                    m_SpriteRenderer.flipX = true;
            }

            m_Animator.SetBool("WallGrab", m_MoveState.HasFlag(MoveState.WallClimbing));
            m_Animator.SetFloat("MoveX", m_Rigidbody.velocity.x);
            m_Animator.SetFloat("MoveY", m_Rigidbody.velocity.y);
        }



        private IEnumerator CoPlayerTryToDash()
        {
            ++m_CurDashes;

            m_MoveState |= MoveState.Dashing;
            m_MoveState |= MoveState.WallJumping;
            m_MoveState &= ~MoveState.WallClimbing;

            m_Rigidbody.gravityScale = 0f;

            m_DashEffect.transform.position = m_Rigidbody.position;
            m_DashEffect.Play();


            yield return new WaitForSeconds(.15f);


            if (m_MoveState.HasFlag(MoveState.WallClimbing))
                m_Rigidbody.gravityScale = 0f;
            else
                m_Rigidbody.gravityScale = 3.5f;

            m_DashEffect.Stop();

            m_MoveState &= ~MoveState.Dashing;
            m_MoveState &= ~MoveState.WallJumping;
        }

        private IEnumerator CoPlayerDisableMovements()
        {
            m_MoveState |= MoveState.NoMove;
            yield return new WaitForSeconds(.15f);
            m_MoveState &= ~MoveState.NoMove;
        }


        private void PlayerTryJump()
        {
            if (m_MoveState.HasFlag(MoveState.WallClimbing))
            {
                StopCoroutine(CoPlayerDisableMovements());
                StartCoroutine(CoPlayerDisableMovements());

                m_MoveState |= MoveState.WallJumping;
                m_MoveState &= ~MoveState.WallClimbing;
                m_Rigidbody.gravityScale = 3.5f;

                Vector2 wall_dir = transform.right * (m_DirCollision.touchType.HasFlag(TouchType.Left) ? 1 : -1f);
                m_Rigidbody.velocity = new(m_Rigidbody.velocity.x, 0f);
                m_Rigidbody.velocity += m_JumpPower * (((Vector2)transform.up / 1.5f) + (wall_dir / 1.5f));


                m_WallJumpEffect.transform.localScale = new(m_DirCollision.touchType.HasFlag(TouchType.Left) ? 1f : -1f, 1f, 1f);
                m_WallJumpEffect.transform.position = m_Rigidbody.position;
                m_WallJumpEffect.Play();
            }
            else
            {
                if (!m_MoveState.HasFlag(MoveState.Dashing) && m_CurJumps < m_NumberOfJumps)
                {
                    m_CurJumps++;
                    m_Rigidbody.velocity = new(m_Rigidbody.velocity.x, m_JumpPower);

                    m_JumpEffect.transform.position = m_Rigidbody.position;
                    m_JumpEffect.Play();
                }
            }
        }


        private Vector2 GetRawMoveInput()
        {
            var movexy = GameplayInput.Movement.ReadValue<Vector2>();

            for (int i = 0; i < 2; i++)
            {
                if (movexy[i] > 0f)
                    movexy[i] = 1f;
                else if (movexy[i] < 0f)
                    movexy[i] = -1f;
            }

            return movexy;
        }


        [Flags]
        public enum MoveState
        {
            NoMove = 1 << 0,

            Jumping = 1 << 1,
            Dashing = 1 << 2,

            WallClimbing = 1 << 3,
            WallJumping = 1 << 4,
        }


#if false
        private void Reset() => m_DirCollision = new(GetComponent<BoxCollider2D>(), m_Radius, m_GroundLayer);

        private void OnDrawGizmos()
        {
            if (m_Rigidbody)
                m_DirCollision.DrawGizmos(new Color[]{ Color.red, Color.blue, Color.green, Color.yellow });
        }
#endif
    }
}
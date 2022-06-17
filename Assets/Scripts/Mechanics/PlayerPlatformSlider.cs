using Core;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

namespace Mechanics
{
    public class PlayerPlatformSlider : MonoBehaviour
    {
        [SerializeField] private LayerMask m_PlayerMask;
        [SerializeField] private float m_WaitCooldown = 0.3f;

        private PlatformEffector2D m_Effector;
        private static LayerMask s_DefaultLayer;
        private bool m_IsAbovePlatform;

        private void Awake()
        {
            m_Effector = GetComponent<PlatformEffector2D>();
            if (s_DefaultLayer == 0)
                s_DefaultLayer = LayerMask.NameToLayer("Default");
        }

        private void OnEnable()
        {
            var gp = GameManager.GetInputManager().Gameplay;
            gp.Movement.started += OnPlatformTryToSlide;
        }

        private void OnDisable()
        {
            var gp = GameManager.GetInputManager().Gameplay;
            gp.Movement.started -= OnPlatformTryToSlide;
        }

        private void OnPlatformTryToSlide(InputAction.CallbackContext ctx)
        {
            if (!m_IsAbovePlatform)
                return;

            float y = ctx.ReadValue<Vector2>().y;
            if (y >= 0f)
                return;

            StartCoroutine(CoAllowPlayerToSlide());
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if ((m_PlayerMask &  1 << collision.collider.gameObject.layer) != 0)
                m_IsAbovePlatform = true;
        }

        
        private void OnCollisionExit2D(Collision2D collision)
        {
            if ((m_PlayerMask &  1 << collision.collider.gameObject.layer) != 0)
                m_IsAbovePlatform = false;
        }


        private IEnumerator CoAllowPlayerToSlide()
        {
            int old_layer = gameObject.layer;

            m_Effector.colliderMask &= ~m_PlayerMask;
            gameObject.layer = s_DefaultLayer;

            yield return new WaitForSeconds(m_WaitCooldown);

            gameObject.layer = old_layer;
            m_Effector.colliderMask |= m_PlayerMask;
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public LocalInputActions InputSystem
        {
            get { return m_InputActions; }
        }


        private LocalInputActions m_InputActions;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            m_InputActions = new();

            m_InputActions.Gameplay.Enable();
            m_InputActions.Shared.Enable();
            m_InputActions.Shared.Menu.started += OnUIChange;
        }


        private void OnUIChange(InputAction.CallbackContext ctx)
        {
            if (m_InputActions.Gameplay.enabled)
            {
                m_InputActions.Gameplay.Disable();
                m_InputActions.UI.Enable();
            }
            else
            {
                m_InputActions.Gameplay.Enable();
                m_InputActions.UI.Disable();
            }
        }
    }
}
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager s_Instance;
        private LocalInputActions m_InputSystem;

        public enum InputState
        {
            Gameplay,
            UI
        };


        private void Awake()
        {
            if (s_Instance)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;
            DontDestroyOnLoad(gameObject);

            m_InputSystem = new();

            SetInputState(InputState.Gameplay);

            m_InputSystem.UI.ExitUI.started += OnUITryExit;
        }


        public static void SetInputState(InputState state) => s_Instance.SetInputStateImpl(state);
        private void SetInputStateImpl(InputState state)
        {
            var gp = m_InputSystem.Gameplay;
            var ui = m_InputSystem.UI;

            switch (state)
            {
            case InputState.Gameplay:
                gp.Movement.Enable();
                gp.Jump.Enable();
                gp.Dash.Enable();

                ui.Disable();
                Time.timeScale = 1f;

                break;
            case InputState.UI:
                gp.Movement.Disable();
                gp.Jump.Disable();
                gp.Dash.Disable();

                ui.Enable();
                Time.timeScale = 0f;

                break;
            }
        }


        public static LocalInputActions GetInputManager() =>
            s_Instance.m_InputSystem;


        private static void OnUITryExit(InputAction.CallbackContext ctx)
        {
            LevelsUI.UnloadUI();
            SetInputState(InputState.Gameplay);
        }
    }
}
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public LocalInputActions InputSystem
        {
            get;
            private set;
        }


        public enum InputState
        {
            Gameplay,
            UI
        };



        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InputSystem = new();

            SetInputState(InputState.Gameplay);

            InputSystem.UI.ExitUI.started += OnUITryExit;
        }


        public static void SetInputState(InputState state) => Instance.SetInputStateImpl(state);
        private void SetInputStateImpl(InputState state)
        {
            var gp = InputSystem.Gameplay;
            var ui = InputSystem.UI;

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


        private static void OnUITryExit(InputAction.CallbackContext ctx)
        {
            LevelsUI.UnloadUI();
            SetInputState(InputState.Gameplay);
        }
    }
}
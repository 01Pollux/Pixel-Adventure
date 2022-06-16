using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_PlayerController;

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
        }


        public void SetInputState(InputState state)
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

                break;
            case InputState.UI:
                gp.Movement.Disable();
                gp.Jump.Disable();
                gp.Dash.Disable();

                ui.Enable();
                break;
            }
        }
    }
}
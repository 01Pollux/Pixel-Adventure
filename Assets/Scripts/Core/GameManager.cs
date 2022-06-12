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
        }
    }
}
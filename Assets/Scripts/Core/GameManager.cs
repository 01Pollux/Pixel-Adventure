using Mechanics;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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

        public GameObject PlayerObject => m_PlayerController;
        public PlayerController PlayerController => PlayerObject.GetComponentInChildren<PlayerController>();

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(m_PlayerController);

            InputSystem = new();
        }
    }
}
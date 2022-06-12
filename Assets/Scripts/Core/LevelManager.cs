using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Core
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_LevelsUI;

        private int m_LevelId;
        private Button[] m_Levels;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            RegisterLevels();
        }


        private void OnSceneLoaded(Scene scene, LoadSceneMode scene_mode)
        {
            m_LevelId = scene.buildIndex;
            if (m_LevelId > 0)
                m_Levels[m_LevelId - 1].interactable = true;
        }

        private void RegisterLevels()
        {
            DontDestroyOnLoad(m_LevelsUI);

            m_Levels = m_LevelsUI.GetComponentsInChildren<Button>();
            int last_level = PlayerPrefs.HasKey("LastLevel") ? PlayerPrefs.GetInt("LastLevel") : 1;

            for (int i = 0; i < last_level; i++)
                m_Levels[i].interactable = true;

            for (int i = 0; i < m_Levels.Length; i++)
            {
                int index = i + 1;
                m_Levels[i].onClick.AddListener(() => { GoTo(index); });
            }
        }


        public void GoTo(int level)
        {
            if (level < SceneManager.sceneCountInBuildSettings)
            {
                m_LevelId = level;
                EnableGame();
                SceneManager.LoadScene(m_LevelId);
            }
        }

        public void GoToLast()
        {
            int last_level = PlayerPrefs.HasKey("LastLevel") ? PlayerPrefs.GetInt("LastLevel") : 1;
            GoTo(last_level);
        }

        public void GoToNext()
        {
            int last_level = PlayerPrefs.HasKey("LastLevel") ? PlayerPrefs.GetInt("LastLevel") : 1;
            GoTo(last_level);
        }



        public void ToggleLevelsPopup(bool state) => m_LevelsUI.SetActive(state);


        private void EnableGame()
        {
            var input_actions = GameManager.Instance.InputSystem;
            if (input_actions.Gameplay.enabled)
            {
                input_actions.Gameplay.Disable();
                input_actions.UI.Enable();
                m_LevelsUI.SetActive(false);
            }
            else
            {
                input_actions.Gameplay.Enable();
                input_actions.UI.Disable();
                m_LevelsUI.SetActive(false);
            }
        }
    }
}
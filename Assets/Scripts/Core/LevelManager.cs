using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EventSystem;

namespace Core
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_LevelsUI;
        [SerializeField] private GameObject m_IngameButtonUI;

        private int m_LevelId;

        private Button[] m_Levels;
        private Button[] m_IngameButton;


        public int CurrentLevel => m_LevelId;
        public int NextLevel => SceneManager.sceneCount <= (m_LevelId + 1) ? -1 : m_LevelId;


        private enum IngameButton
        {
            Previous,
            Levels,
            Restart,
            Next
        }


        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            RegisterLevels();
            RegisterIngameButtons();

            PlayerPrefs.SetInt("HighestLevel", 15);
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
            int last_level = PlayerPrefs.HasKey("HighestLevel") ? PlayerPrefs.GetInt("HighestLevel") : 1;

            for (int i = 0; i < last_level; i++)
                m_Levels[i].interactable = true;

            for (int i = 0; i < m_Levels.Length; i++)
            {
                int index = i + 1;
                m_Levels[i].onClick.AddListener(() => { GoTo(index); });
            }
        }

        private void RegisterIngameButtons()
        {
            DontDestroyOnLoad(m_IngameButtonUI);

            m_IngameButton = m_IngameButtonUI.GetComponentsInChildren<Button>();

            m_IngameButton[(int)IngameButton.Previous].interactable = false;
            m_IngameButton[(int)IngameButton.Next].interactable = false;

            m_IngameButton[(int)IngameButton.Previous].onClick.AddListener(
                () =>
                {
                    int level = CurrentLevel;
                    if (level > 1)
                        level--;
                    this.GoTo(level);
                }
            );

            m_IngameButton[(int)IngameButton.Next].onClick.AddListener(
                () =>
                {
                    int level = NextLevel;
                    if (level != -1)
                        this.GoTo(level);
                }
            );

            m_IngameButton[(int)IngameButton.Restart].onClick.AddListener(
                () =>
                {
                    SceneManager.LoadScene(this.CurrentLevel);
                    GameMessenger.Raise(EGameEvent.PlayerSpawn, this);
                }
                );
        }


        public void GoTo(int level)
        {
            if (level < SceneManager.sceneCountInBuildSettings)
            {
                m_LevelId = level;

                UpdateHighestLevel();
                ToggleGameUI();

                SceneManager.LoadScene(m_LevelId);

                if (level > 0)
                {
                    GameManager.Instance.PlayerObject.SetActive(true);
                    GameMessenger.Raise(EGameEvent.PlayerSpawn, this);
                }
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

        public void OpenLevels()
        {
            m_LevelsUI.SetActive(true);
        }




        public void ToggleLevelsPopup(bool state) => m_LevelsUI.SetActive(state);


        private void ToggleGameUI()
        {
            var input_actions = GameManager.Instance.InputSystem;
            if (input_actions.Gameplay.enabled)
            {
                input_actions.Gameplay.Disable();
                input_actions.UI.Enable();

                m_LevelsUI.SetActive(false);
                m_IngameButtonUI.SetActive(false);
            }
            else
            {
                input_actions.Gameplay.Enable();
                input_actions.UI.Disable();

                m_LevelsUI.SetActive(false);
                m_IngameButtonUI.SetActive(true);

                UpdateIngameUI();
            }
        }


        private void UpdateIngameUI()
        {
            int highest_level = PlayerPrefs.HasKey("HighestLevel") ? PlayerPrefs.GetInt("HighestLevel") : 1;

            m_IngameButton[(int)IngameButton.Previous].interactable = m_LevelId > 1;
            m_IngameButton[(int)IngameButton.Next].interactable = m_LevelId < highest_level;

        }


        private void UpdateHighestLevel()
        {
            int highest_level = PlayerPrefs.HasKey("HighestLevel") ? PlayerPrefs.GetInt("HighestLevel") : 1;
            if (m_LevelId > highest_level)
                PlayerPrefs.SetInt("HighestLevel", m_LevelId);
        }
    }
}
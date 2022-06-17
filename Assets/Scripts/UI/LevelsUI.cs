using Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LevelsUI : MonoBehaviour
    {
        [SerializeField] private GameObject m_Levels;
        [SerializeField] private Button m_HomeButton;

        private void Awake()
        {
            // Remove
            PlayerPrefs.SetInt("CompletedLevels", 2);
            PlayerPrefs.SetInt("CurrentLevel", 1);

            Button[] buttons = m_Levels.GetComponentsInChildren<Button>();

            for (int i = 0; i < PlayerPrefs.GetInt("CompletedLevels"); i++)
                buttons[i].interactable = true;

            for (int i = 0; i < buttons.Length; i++)
            {
                int idx = i + 1;
                buttons[i].onClick.AddListener(
                    () =>
                    {
                        LoadLevel(idx, false);
                        GameManager.SetInputState(GameManager.InputState.Gameplay);
                    }
                );
            }

            m_HomeButton.onClick.AddListener(
                () =>
                {
                    SceneManager.LoadScene("MainMenu");
                });
        }

        public static void LoadLevel(int index, bool check_bounds)
        {
            if (check_bounds && (index < 1 || index > PlayerPrefs.GetInt("CompletedLevels")))
                return;

            SceneManager.LoadScene($"Lvl{index}");
            PlayerPrefs.SetInt("CurrentLevel", index);
        }


        public static void RestartLevel() =>
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public static void LoadUI() =>
            SceneManager.LoadSceneAsync("UILevels", LoadSceneMode.Additive);
        
        public static void UnloadUI() =>
            SceneManager.UnloadSceneAsync("UILevels");


        public static int CompletedLevels() =>
            PlayerPrefs.GetInt("CompletedLevels");

        public static int CurrentLevel() =>
            PlayerPrefs.GetInt("CurrentLevel");
    }
}
using Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.PackageManager;

namespace UI
{
    public class LevelsUI : MonoBehaviour
    {
        private void Awake()
        {
            // Remove
            PlayerPrefs.SetInt("CompletedLevels", 50);

            Button[] buttons = GetComponentsInChildren<Button>();

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
        }

        public static void LoadLevel(int index, bool check_bounds)
        {
            if (check_bounds && (index < 1 || index > PlayerPrefs.GetInt("CompletedLevels")))
                return;

            SceneManager.LoadScene($"Lvl{index}");
            PlayerPrefs.SetInt("CurrentLevel", index);
        }

        public static int CompletedLevels() =>
            PlayerPrefs.GetInt("CompletedLevels");

        public static int CurrentLevel() =>
            PlayerPrefs.GetInt("CurrentLevel");
    }
}
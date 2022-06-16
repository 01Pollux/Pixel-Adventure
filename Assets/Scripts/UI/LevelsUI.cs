using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Core;

namespace UI
{
    public class LevelsUI : MonoBehaviour
    {
        void Awake()
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
                        SceneManager.LoadScene($"Lvl{idx}");
                        GameManager.SetInputState(GameManager.InputState.Gameplay);
                    }
                );
            }
        }
    }
}
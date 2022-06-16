using Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class IngameUI : MonoBehaviour
    {
        public void PreviousLevel()
        {
            int scene_idx = LevelsUI.CurrentLevel();
            if (scene_idx > 1)
                LevelsUI.LoadLevel(scene_idx - 1, false);
        }

        public void NextLevel()
        {
            LevelsUI.LoadLevel(LevelsUI.CurrentLevel() + 1, true);
        }

        public void OpenLevels()
        {
            SceneManager.LoadSceneAsync("UILevels", LoadSceneMode.Additive);
            GameManager.SetInputState(GameManager.InputState.UI);
        }

        public void RestartLevel() =>
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

using Core;
using UnityEngine;

namespace UI
{
    public class GameUI : MonoBehaviour
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
            LevelsUI.LoadUI();
            GameManager.SetInputState(GameManager.InputState.UI);
        }

        public void RestartLevel() =>
            LevelsUI.RestartLevel();


        public void ContinueLevel()
        {
            LevelsUI.LoadLevel(LevelsUI.CompletedLevels(), true);
            GameManager.SetInputState(GameManager.InputState.Gameplay);
        }
    }
}

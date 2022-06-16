using Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class IngameUI : MonoBehaviour
    {
        public void RestartLevel() =>
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void OpenLevels()
        {
            SceneManager.LoadSceneAsync("UILevels", LoadSceneMode.Additive);
            GameManager.SetInputState(GameManager.InputState.UI);
        }
    }
}

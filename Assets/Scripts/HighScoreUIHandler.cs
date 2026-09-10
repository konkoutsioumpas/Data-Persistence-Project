using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HighScoreUIHandler : MonoBehaviour
{
    public TextMeshProUGUI scoresText;

    void Start()
    {
        if (scoresText == null)
        {
            return;
        }

        if (DataManager.Instance == null)
        {
            scoresText.text = "No high scores yet.";
            return;
        }

        scoresText.text = DataManager.Instance.GetHighScoresDisplay();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}

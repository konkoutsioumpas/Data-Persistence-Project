using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuUIHandler : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public TextMeshProUGUI bestScoreText;

    void Start()
    {
        if (DataManager.Instance != null)
        {
            string lastName = DataManager.Instance.GetPlayerName();
            if (!string.IsNullOrEmpty(lastName))
            {
                playerNameInput.text = lastName;
            }
        }

        if (bestScoreText != null)
        {
            bestScoreText.text = MainManager.UpdateHighScore();
        }
    }

    public void StartNew()
    {
        DataManager.Instance.SetPlayerName(playerNameInput.text);
        SceneManager.LoadScene(1);
    }

    public void ShowHighScores()
    {
        SceneManager.LoadScene(2);
    }
}

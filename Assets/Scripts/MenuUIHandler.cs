using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuUIHandler : MonoBehaviour
{
    public TMP_InputField playerNameInput;

    void Start()
    {
        string lastName = DataManager.Instance.GetPlayerName();
        if (!string.IsNullOrEmpty(lastName))
        {
            playerNameInput.text = lastName;
        }
    }

    public void StartNew()
    {
        DataManager.Instance.SetPlayerName(playerNameInput.text);
        SceneManager.LoadScene(1);
    }
}

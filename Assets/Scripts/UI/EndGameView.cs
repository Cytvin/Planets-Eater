using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameView : MonoBehaviour
{
    [SerializeField]
    private Button _mainMenu;
    [SerializeField]
    private TextMeshProUGUI _text;

    public void Init()
    {
        _mainMenu.onClick.AddListener(OnMainMenuClick);
    }

    public void EnableOnVictory()
    {
        gameObject.SetActive(true);
        SetVictoryText();
    }

    public void EnableOnDefeat()
    {
        gameObject.SetActive(true);
        SetDefeatText();
    }

    private void SetVictoryText()
    {
        _text.SetText("Victory");
        _text.color = Color.green;
    }

    private void SetDefeatText()
    {
        _text.SetText("Defeat");
        _text.color = Color.red;
    }

    private void OnMainMenuClick()
    {
        SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
        MainMenu.Instance.MainMenuEnable();
    }
}

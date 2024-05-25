using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Button _start;
    [SerializeField]
    private Button _continue;
    [SerializeField]
    private Button _settings;
    [SerializeField]
    private Button _help;
    [SerializeField]
    private Button _exit;
    [SerializeField]
    private Button _mainMenu;
    [SerializeField]
    private SettingsView _settingsView;

    private static MainMenu _instance;

    public static MainMenu Instance => _instance;

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _start.onClick.AddListener(OnStartClick);
        _continue.onClick.AddListener(OnContinueGame);
        _settings.onClick.AddListener(OnSettingsClick);
        _exit.onClick.AddListener(OnExitClick);
        _mainMenu.onClick.AddListener(OnMainMenuClick);

        DontDestroyOnLoad(this);
    }

    public void MainMenuEnable()
    {
        gameObject.SetActive(true);
        _start.gameObject.SetActive(true);
        _continue.gameObject.SetActive(false);

        _exit.gameObject.SetActive(true);
        _mainMenu.gameObject.SetActive(false);
    }

    public void InGameMenuEnable()
    {
        gameObject.SetActive(true);
        _start.gameObject.SetActive(false);
        _continue.gameObject.SetActive(true);

        _exit.gameObject.SetActive(false);
        _mainMenu.gameObject.SetActive(true);
    }

    private void OnStartClick()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        gameObject.SetActive(false); 
    }

    private void OnContinueGame()
    {
        gameObject.SetActive(false);
    }

    private void OnSettingsClick()
    {
        _settingsView.Enable();
    }

    private void OnHelpClick()
    {

    }

    private void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void OnMainMenuClick()
    {
        SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
        MainMenuEnable();
    }
}

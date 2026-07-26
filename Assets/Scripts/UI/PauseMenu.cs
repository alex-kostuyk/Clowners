using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button returnToMenuButton;
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private string menuSceneName = "Main menu";

    private bool _isPaused;
    private CursorLockMode _previousLockState = CursorLockMode.None;

    private void Awake()
    {
        SetupListeners();
        if (pausePanel != null) pausePanel.SetActive(false);
        _isPaused = false;
        _previousLockState = Cursor.lockState;
    }

    private void OnEnable()
    {
        SetupListeners();
    }

    private void SetupListeners()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(ResumeGame);
            continueButton.onClick.AddListener(ResumeGame);
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(ResetGame);
            resetButton.onClick.AddListener(ResetGame);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(OnSettingsPressed);
            settingsButton.onClick.AddListener(OnSettingsPressed);
        }

        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.RemoveListener(OnReturnToMenuPressed);
            returnToMenuButton.onClick.AddListener(OnReturnToMenuPressed);
        }

    }

    private void Update()
    {
        SetupListeners();

        bool escapePressed = Input.GetKeyDown(KeyCode.Escape);
        bool pPressed = Input.GetKeyDown(KeyCode.P);
        bool cursorUnlockedInEditor = false;

#if UNITY_EDITOR
        if (!_isPaused && _previousLockState == CursorLockMode.Locked && Cursor.lockState != CursorLockMode.Locked)
        {
            cursorUnlockedInEditor = true;
        }
        _previousLockState = Cursor.lockState;
#endif

        if (pPressed)
        {
            HandlePauseToggle();
        }
        else if (escapePressed)
        {
            if (settingsMenu != null && settingsMenu.IsVisible)
            {
                settingsMenu.Hide();
            }
            else
            {
                HandlePauseToggle();
            }
        }
        else if (cursorUnlockedInEditor)
        {
            if (!_isPaused)
            {
                PauseGame();
            }
        }
    }

    private void LateUpdate()
    {
        if (_isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HandlePauseToggle()
    {
        if (settingsMenu != null && settingsMenu.IsVisible)
        {
            settingsMenu.Hide();
        }
        else if (_isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
#if UNITY_EDITOR
        _previousLockState = Cursor.lockState;
#endif
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#if UNITY_EDITOR
        _previousLockState = Cursor.lockState;
#endif
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void ResetGame()
    {
        Time.timeScale = 1f;
        _isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSettingsPressed()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsMenu != null)
        {
            settingsMenu.OnBack = () => { if (pausePanel != null) pausePanel.SetActive(true); };
            settingsMenu.Show();
        }
    }

    private void OnReturnToMenuPressed()
    {
        Time.timeScale = 1f;
        _isPaused = false;
        SceneManager.LoadScene(menuSceneName);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}

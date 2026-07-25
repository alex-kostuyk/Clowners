using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using KinematicCharacterController.Examples;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button returnToMenuButton;

    private bool _isPaused;
    private ExamplePlayer _player;
    private CameraLookAtEventTrigger _interactionTrigger;

    private void Awake()
    {
        if (continueButton != null) continueButton.onClick.AddListener(ResumeGame);
        if (resetButton != null) resetButton.onClick.AddListener(ResetGame);
        // Settings and Return to Menu are wired but do nothing yet
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsPressed);
        if (returnToMenuButton != null) returnToMenuButton.onClick.AddListener(OnReturnToMenuPressed);

        if (pausePanel != null) pausePanel.SetActive(false);
        _isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        SetPlayerInputEnabled(false);
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        SetPlayerInputEnabled(true);
    }

    private void ResetGame()
    {
        Time.timeScale = 1f;
        _isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSettingsPressed()
    {
        // TODO: implement settings menu
        Debug.Log("Settings button pressed - not implemented yet");
    }

    private void OnReturnToMenuPressed()
    {
        // TODO: implement return to main menu
        Debug.Log("Return to Game Menu pressed - not implemented yet");
    }

    private void OnDestroy()
    {
        // Ensure timeScale is restored if this object is destroyed
        Time.timeScale = 1f;
    }

    private void SetPlayerInputEnabled(bool enabled)
    {
        if (_player == null) _player = FindObjectOfType<ExamplePlayer>();
        if (_interactionTrigger == null) _interactionTrigger = FindObjectOfType<CameraLookAtEventTrigger>();

        if (_player != null) _player.enabled = enabled;
        if (_interactionTrigger != null) _interactionTrigger.enabled = enabled;
    }
}

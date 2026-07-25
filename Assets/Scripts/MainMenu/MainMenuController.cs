using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Settings Menu")]
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private GameObject mainButtonsPanel;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Test";

    [Header("Surveillance HUD")]
    [SerializeField] private TextMeshProUGUI timestampText;
    [SerializeField] private TextMeshProUGUI cameraIdText;
    [SerializeField] private GameObject recordingDot;

    private float _recordingDotTimer;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (newGameButton != null) newGameButton.onClick.AddListener(OnNewGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettings);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuit);

        if (cameraIdText != null)
            cameraIdText.text = "CAM_01 // FACILITY_MAIN";
    }

    private void Update()
    {
        // Update timestamp
        if (timestampText != null)
        {
            timestampText.text = System.DateTime.Now.ToString("yyyy.MM.dd  HH:mm:ss");
        }

        // Blink recording dot
        if (recordingDot != null)
        {
            _recordingDotTimer += Time.deltaTime;
            recordingDot.SetActive(Mathf.FloorToInt(_recordingDotTimer * 1.2f) % 2 == 0);
        }

        // Esc handling when settings is open
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenu != null && settingsMenu.IsVisible)
            {
                settingsMenu.Hide();
            }
        }
    }

    private void OnNewGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnSettings()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (settingsMenu != null)
        {
            settingsMenu.OnBack = () => { if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true); };
            settingsMenu.Show();
        }
    }

    private void OnQuit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}

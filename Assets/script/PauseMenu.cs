using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }
    public bool CanPause { get; private set; } = true;

    [Header("PanelPause")]
    [SerializeField] private GameObject pauseRoot;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject commandPanel;

    [Header("PanelPauseVerif")]
    
    [SerializeField] private Button firstSelectedButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button backCommand;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private PlayerInputHandler[] playerInputs;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        if (pauseRoot != null) pauseRoot.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
        if (confirmPanel != null) confirmPanel.SetActive(false);
        if (commandPanel != null) commandPanel.SetActive(false);
    }

    private void Update()
    {
        if (!PausePressedThisFrame()) return;
        if (!CanPause && !IsPaused) return; // secu ignore echap ou start
        if (!IsPaused)
        {
            PauseGame();
            return;
        }

        if (confirmPanel != null && confirmPanel.activeSelf)
        {
            ShowPauseMainPanel();
        }
        else
        {
            ResumeGame();
        }
    }

    public void SetCanPause(bool value)
    {
        CanPause = value;
        // secu si deja en pause (force le retour)
        if (!CanPause && IsPaused) ResumeGame();
    }

    private bool PausePressedThisFrame()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) return true;

        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad != null && gamepad.startButton.wasPressedThisFrame) return true;
        }

        return false;
    }

    private void CachePlayers()
    {
        playerInputs = FindObjectsByType<PlayerInputHandler>(FindObjectsSortMode.None);
    }

    private void SetSelected(Button button)
    {
        if (button == null || EventSystem.current == null)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }

    public void PauseGame()
    {
        if (IsPaused) return;
        CachePlayers();
        IsPaused = true;
        
        if (playerInputs != null)
        {
            foreach (var input in playerInputs)
            {
                if (input == null) continue;
                input.SetInputsEnabled(false);
                input.ClearInputs();
            }
        }

        Time.timeScale = 0f;
        
        if (pauseRoot != null) pauseRoot.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(true);
        if (confirmPanel != null) confirmPanel.SetActive(false);
        if (commandPanel != null) commandPanel.SetActive(false);

        SetSelected(firstSelectedButton);
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;

        Time.timeScale = 1f;
        IsPaused = false;

        if (pauseRoot != null) pauseRoot.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
        if (confirmPanel != null) confirmPanel.SetActive(false);

        CachePlayers();

        if (playerInputs != null)
        {
            foreach (var input in playerInputs)
            {
                if (input == null) continue;
                input.SetInputsEnabled(true);
                input.ClearInputs();
            }
        }
    }

    public void ShowCommandPanel()
    {
        if (!IsPaused) return;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (commandPanel != null) commandPanel.SetActive(true);

        SetSelected(backCommand);
    }

    public void ShowConfirmPanel()
    {
        if (!IsPaused) return;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (confirmPanel != null) confirmPanel.SetActive(true);

        SetSelected(backButton);
    }

    public void ShowPauseMainPanel()
    {
        if (!IsPaused) return;

        if (pausePanel != null) pausePanel.SetActive(true);
        if (confirmPanel != null) confirmPanel.SetActive(false);
        if (commandPanel != null) commandPanel.SetActive(false);

        SetSelected(firstSelectedButton);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

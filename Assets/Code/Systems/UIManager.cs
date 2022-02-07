using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject pauseMenu;


    private void Start()
    {
        resumeButton.onClick.AddListener(HandleResume);
        quitButton.onClick.AddListener(HandleQuit);
    }

    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void HidePauseMenu()
    {
        pauseMenu.SetActive(false);
    }
    
    private void HandleResume()
    {
        GameManager.Instance.TogglePause();
    }

    private void HandleQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();       
#endif
    }
}

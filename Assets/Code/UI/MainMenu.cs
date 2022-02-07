using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Animator crossfade;

    private int _optionSelected;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        startButton.onClick.AddListener(HandleStart);
        quitButton.onClick.AddListener(HandleQuit);
    }

    private void HandleStart()
    {
        StartCoroutine(LoadScene());
    }

    private void HandleQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();       
#endif
    }

    private IEnumerator LoadScene()
    {
        crossfade.SetTrigger("fade");
        var operation = SceneManager.LoadSceneAsync("Gameplay");
        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}

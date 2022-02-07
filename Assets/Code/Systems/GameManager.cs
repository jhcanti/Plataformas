using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameState { PREGAME, INITIALIZING, RUNNING, PAUSED, DEATH };

    [SerializeField] GameState _currentGameState;

    public GameState CurrentGameState { get { return _currentGameState; } private set { _currentGameState = value; } }

    public Events.EventChangeGameState OnGameStateChanged;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _currentGameState = GameState.PREGAME;
        GameData data = new GameData(transform.position.x, transform.position.y);
        SaveManager.Instance.SaveData(data);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause") || Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        UpdateState(_currentGameState == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);
    }

    public void UpdateState(GameState state)
    {
        GameState _previousGameState = _currentGameState;
        _currentGameState = state;

        if (_currentGameState == GameState.PAUSED)
        {
            Time.timeScale = 0;
            UIManager.Instance.ShowPauseMenu();
        }
        else
        {
            Time.timeScale = 1f;
            UIManager.Instance.HidePauseMenu();
        }
        
        OnGameStateChanged.Invoke(_currentGameState, _previousGameState);        
    }
}

using UnityEngine;

public class Music : MonoBehaviour
{
    public string[] songs;    
    int index = 0;

    public Events.EventChangeGameState OnGameStateChanged;


    void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleOnGameStateChanged);        
    }


    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING)
        {
            if (!AudioManager.Instance.isPlaying(songs[index]))
            {
                index++;
                if (index > 2)
                    index = 0;
                AudioManager.Instance.Play(songs[index]);
            }
        }        
    }


    void HandleOnGameStateChanged(GameManager.GameState state, GameManager.GameState previousState)
    {
        switch (state)
        {
            case GameManager.GameState.INITIALIZING:
                break;

            case GameManager.GameState.RUNNING:                
                AudioManager.Instance.Play(songs[index]);
                break;

            case GameManager.GameState.DEATH:
                AudioManager.Instance.Stop(songs[index]);
                break;

            default:
                break;
        }
    }
}

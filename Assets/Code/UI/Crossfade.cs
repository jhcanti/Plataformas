using UnityEngine;

public class Crossfade : MonoBehaviour
{
    Animation anim;

    public Events.EventChangeGameState OnGameStateChanged;


    private void Start()
    {
        anim = GetComponent<Animation>();
        GameManager.Instance.OnGameStateChanged.AddListener(HandleOnGameStateChanged);
    }


    public void FadeOutComplete()
    {
        GameManager.Instance.UpdateState(GameManager.GameState.INITIALIZING);
    }


    public void FadeInComplete()
    {
        GameManager.Instance.UpdateState(GameManager.GameState.RUNNING);
    }


    void HandleOnGameStateChanged(GameManager.GameState state, GameManager.GameState previousState)
    {
        if (state == GameManager.GameState.INITIALIZING)
        {
            anim.clip = anim.GetClip("FadeIn");
            anim.Play();
        }

        if (state == GameManager.GameState.DEATH && previousState == GameManager.GameState.RUNNING)
        {
            anim.clip = anim.GetClip("FadeOut");
            anim.Play();
        }
    }
}

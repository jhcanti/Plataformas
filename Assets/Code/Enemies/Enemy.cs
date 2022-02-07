using UnityEngine;

namespace Code.Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        protected bool isMoving;
        
        private void Start()
        {
            GameManager.Instance.OnGameStateChanged.AddListener(HandleOnGameStateChanged);
            DoStart();
        }

        protected abstract void DoStart();
        
        private void FixedUpdate()
        {
            CheckEnvironment();
            DoFixedUpdate();
        }

        protected abstract void DoFixedUpdate();
        protected abstract void CheckEnvironment();

        private void OnTriggerStay2D(Collider2D other)
        {
            DoOnTriggerStay(other);
        }

        protected abstract void DoOnTriggerStay(Collider2D other);
        
        
        private void HandleOnGameStateChanged(GameManager.GameState state, GameManager.GameState previousState)
        {
            isMoving = state != GameManager.GameState.PAUSED;
            
            switch (state)
            {
                case GameManager.GameState.INITIALIZING:
                    DoInitialize();
                    break;
                
                default:
                    break;
            }           
        }

        protected abstract void DoInitialize();
    }
}
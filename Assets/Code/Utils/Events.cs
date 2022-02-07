using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Events
{
    [System.Serializable] public class EventChangeGameState : UnityEvent<GameManager.GameState, GameManager.GameState> { }    
}

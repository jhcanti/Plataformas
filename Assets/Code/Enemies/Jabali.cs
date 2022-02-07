using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jabali : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator anim;
    public BoxCollider2D colliderEnemy;


    [Header("Movement")]
    public float speed = 2;
    public int dirFacing = -1;
    public bool isMoving = false;

    [Header("General")]
    public int health = 2;
    public float timeToRespawn = 15;
    public Events.EventChangeGameState OnGameStateChanged;




    void FixedUpdate()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.RUNNING)
            return;

        rb.velocity = new Vector2(speed * dirFacing, rb.velocity.y);
    }


}

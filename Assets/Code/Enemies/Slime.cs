using System.Collections;
using Code.Enemies;
using UnityEngine;

public class Slime : Enemy
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator anim;
    public Collider2D colliderEnemy;

    [Header("Environment")]
    public Transform originRay;
    public LayerMask whatIsPlayer;
    public Vector2 rayLeft;
    public Vector2 rayRight;    
    public float distanceRay = 6;
    public float detectRadius = 1.2f;
    bool isTouchingLeft = false;
    bool isTouchingRight = false;
    bool isTouchingDown = false;
    bool isDropping = false;
    Vector2 initPosition;
    
    public float timeToRespawn = 15;



    protected override void DoStart()
    {
        rayLeft.Normalize();
        rayRight.Normalize();
        initPosition = rb.position;
    }


    protected override void DoOnTriggerStay(Collider2D other)
    {
        
    }

    protected override void DoInitialize()
    {
        rb.position = initPosition;
        colliderEnemy.enabled = true;
        isDropping = false;
    }


    protected override void DoFixedUpdate()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.RUNNING)
            return;

        if (!isDropping)
        {
            CheckPlayer();
            if (isTouchingLeft || isTouchingRight || isTouchingDown)
            {
                isDropping = true;
                anim.SetTrigger("Drop");
            }
        }
    }

    protected override void CheckEnvironment()
    {
        
    }


    void CheckPlayer()
    {
        isTouchingLeft = Physics2D.Raycast(originRay.position, rayLeft, distanceRay, whatIsPlayer);
        isTouchingRight = Physics2D.Raycast(originRay.position, rayRight, distanceRay, whatIsPlayer);
        isTouchingDown = Physics2D.Raycast(originRay.position, Vector2.down, distanceRay, whatIsPlayer);
    }


    void Drop()
    {
        rb.gravityScale = 1;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            anim.SetTrigger("Splash");
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats.Instance.TakeDamage(1);
        }
    }


    void Death()
    {
        colliderEnemy.enabled = false;
        AudioManager.Instance.Play("Slime Death");
        if (Physics2D.OverlapCircle(originRay.position, detectRadius, whatIsPlayer))
        {
            PlayerStats.Instance.TakeDamage(1);
        }

        StartCoroutine(Respawn());
    }


    IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(timeToRespawn);
        DoInitialize();
        yield return new WaitForSecondsRealtime(.1f);
        anim.SetTrigger("Appears");
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(originRay.position, rayLeft * distanceRay);
        Gizmos.DrawRay(originRay.position, rayRight * distanceRay);
        Gizmos.DrawRay(originRay.position, Vector2.down * distanceRay);
        Gizmos.DrawWireSphere(originRay.position, detectRadius);
    }
}

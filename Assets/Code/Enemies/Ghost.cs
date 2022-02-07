using System.Collections;
using Code.Enemies;
using UnityEngine;

public class Ghost : Enemy
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator anim;
    public BoxCollider2D colliderEnemy;

    [Header("Environment")]
    public Transform frontGroundCheck;
    public Transform backGroundCheck;
    public Transform frontCheck;
    public LayerMask whatIsGround;
    bool isTouching = false;
    bool frontGrounded = false;
    bool backGrounded = false;
    bool shriek = false;
    public float distanceGroundCheck = .3f;
    public float distanceFrontCheck = .05f;
    public float detectRadius = 2;
    Vector2 initPosition;
    int initDirFacing;

    [Header("Movement")]
    public float speed = 2;
    public int dirFacing = -1;
    float initSpeed;

    [Header("General")]
    public GameObject soulEffectPrefab;    
    public bool isKnockBacking = false;
    public float timeKnockBacking = 0.5f;
    public int health = 2;
    public float timeToRespawn = 15;


    protected override void DoStart()
    {
        initDirFacing = dirFacing;
        initPosition = rb.position;
        initSpeed = speed;        
    }


    protected override void DoInitialize()
    {
        rb.position = initPosition;
        dirFacing = initDirFacing;
        speed = initSpeed;

        Vector3 scale = transform.localScale;
        scale.x = -dirFacing;
        transform.localScale = scale;
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = Vector2.zero;        
        colliderEnemy.enabled = true;
        health = 2;
        isKnockBacking = false;
    }


    void BeginMoving()
    {
        isMoving = true;
    }


    protected override void DoFixedUpdate()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.RUNNING)
            return;

        if (!isMoving)
            return;

        CheckEnvironment();
        
        if (isTouching)
            Flip();

        if (isKnockBacking)
        {
            if (rb.velocity.x > 0)
            {
                if ((dirFacing == 1 && !frontGrounded) || (dirFacing == -1 && !backGrounded))
                {
                    if (dirFacing == 1)
                        Flip();

                    rb.velocity = new Vector2(0, rb.velocity.y);
                    isKnockBacking = false;
                }                
            }
            else
            {
                if ((dirFacing == 1 && !backGrounded) || (dirFacing == -1 && !frontGrounded))
                {
                    if (dirFacing == -1)
                        Flip();

                    rb.velocity = new Vector2(0, rb.velocity.y);
                    isKnockBacking = false;
                }
            }
        }
        else
        {
            if (!frontGrounded)
                Flip();

            if (rb.bodyType != RigidbodyType2D.Static)
                rb.velocity = new Vector2(speed * dirFacing, rb.velocity.y);
        }
            
    }


    void LateUpdate()
    {
        anim.SetBool("Shriek", shriek);    
    }


    protected override void CheckEnvironment()
    {
        FrontCheck();
        GroundCheck();
        ShriekCheck();
    }


    protected override void DoOnTriggerStay(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStats.Instance.TakeDamage(1);
        }
    }


    void ShriekCheck()
    {
        if (shriek)
            return;

        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, detectRadius);

        if (col != null)
        {
            shriek = false;
            foreach (Collider2D item in col)
            {
                if (item.gameObject.CompareTag("Player"))
                {
                    int directionPlayer = (item.transform.position.x > transform.position.x) ? 1 : -1;
                    if (directionPlayer == dirFacing)
                    {                        
                        shriek = true;
                        AudioManager.Instance.Play("Ghost_scream");
                    }                        
                }
            }
        }        
    }


    void ShriekFinish()
    {
        shriek = false;
    }


    void FrontCheck()
    {
        isTouching = Physics2D.Raycast(frontCheck.position, Vector2.right * dirFacing, distanceFrontCheck, whatIsGround);
    }


    void GroundCheck()
    {
        frontGrounded = Physics2D.Raycast(frontGroundCheck.position, Vector2.down, distanceGroundCheck, whatIsGround);
        backGrounded = Physics2D.Raycast(backGroundCheck.position, Vector2.down, distanceGroundCheck, whatIsGround);
    }


    void Flip()
    {
        dirFacing *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    void Death()
    {
        isMoving = false;
        shriek = false;
        Instantiate(soulEffectPrefab, transform.position, Quaternion.identity);        
        StartCoroutine(Respawn());
    }


    public void TakeDamage(int direction)
    {
        health--;
        if (health <= 0)
        {
            anim.SetTrigger("Vanish");            
            rb.bodyType = RigidbodyType2D.Static;
            colliderEnemy.enabled = false;
        }
        else
        {
            KnockBackEnemy(direction, PlayerStats.Instance.swordForce);
        }
    }


    public void KnockBackEnemy(int direction, float force)
    {
        isKnockBacking = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.AddForce(Vector2.right * direction * force, ForceMode2D.Impulse);
        StartCoroutine(KnockBacking());
    }

    
    IEnumerator KnockBacking()
    {
        yield return new WaitForSecondsRealtime(timeKnockBacking);
        isKnockBacking = false;
    }


    IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(timeToRespawn);
        DoInitialize();
        anim.SetTrigger("Appears");
    }
}

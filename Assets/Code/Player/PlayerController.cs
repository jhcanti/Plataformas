using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]    
    [SerializeField] float speed = 5;
    [SerializeField] float airMovingForce = 30;
    [SerializeField] float coyoteDuration = .07f;
    [SerializeField] bool isCrouching = false;
    float input;
    bool downPressed = false;
    bool upPressed = false;
    bool isMoving = false;
    float coyoteTime = 0;    
    int dirFacing = 1;

    [Header("Dash")]
    [SerializeField] bool canDash = false;
    [SerializeField] bool isDashing = false;
    [SerializeField] float dashVelocity = 15;
    [SerializeField] float dashCoolDown = 2.5f;  // tiempo de espera entre Dash    
    [SerializeField] float distanceBetweenImages = .1f;
    [SerializeField] float dashTime = .2f;  // tiempo que dura el Dash
    float lastDash = -100f;  // tiempo en el que se realizo el ultimo Dash
    float dashTimeLeft;  // tiempo que falta hasta la siguiente AfterImage
    float lastImageXPos;
    bool dashPressed = false;

    [Header("Attack")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRadius = 0.5f;
    [SerializeField] LayerMask whatIsEnemy;
    public GameObject swordImpactPrefab;    
    bool attackPressed = false;
    bool isAttacking = false;

    [Header("Jump")]
    [SerializeField] bool jumpPressed = false;
    [SerializeField] bool isJumping = false;
    public bool canDoubleJump = false;
    [SerializeField] float jumpForce = 12;
    [SerializeField] int maxJumps = 2;
    int jumpsCounter = 0;    

    [Header("Wall Sliding")]
    [SerializeField] bool isWallSliding = false;
    [SerializeField] bool canSliding = false;
    [SerializeField] bool canWallSliding = true;
    [SerializeField] float maxSlidingSpeed = 1;
    float timeWallSliding = 0;
    [SerializeField] float maxStickyTime = .25f;    

    [Header("Wall Jump")]
    [SerializeField] float wallJumpForce = 13;
    [SerializeField] Vector2 wallJumpDir;

    [Header("Hanging")]
    [SerializeField] bool isClimbing = false;

    [Header("Check Environment")]
    [SerializeField] LayerMask groundLayer;        
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform frontCheck;
    [SerializeField] Transform ledgeCheck;
    [SerializeField] Transform eyesCheck;
    [SerializeField] bool isGrounded = false;
    [SerializeField] bool isTouching = false;
    [SerializeField] bool isHanging = false;

    SpriteRenderer rend;
    Rigidbody2D rb;
    Animator anim;
    [SerializeField] Tilemap platform;
    [SerializeField] BoxCollider2D boxCollider;
    const float smallAmount = 0.05f;
    const float cellSize = .5f;
    bool readyToClear;
    bool gamePaused;
    public Events.EventChangeGameState OnGameStateChanged;


    void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleOnGameStateChanged);

        rend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        // debemos normalizar el Vector de direccion del wallJump pues no queremos que influya
        // en la fuerza del salto
        wallJumpDir.Normalize();
    }


    void Initialize()
    {
        GameData data = SaveManager.Instance.LoadData();
        transform.position = new Vector2(data._pointX, data._pointY);
        dirFacing = 1;
        Vector3 scale = transform.localScale;
        scale.x = 1;
        transform.localScale = scale;
        anim.Rebind();
        ClearInput();
    }


    void Update()
    {
        ClearInput();
        GetInputs();        
    }


    void FixedUpdate()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.RUNNING)
            return;

        readyToClear = true;

        CheckEnvironment();
        CheckDash();
        GroundMovement();
        AirMovement();        
    }


    void LateUpdate()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isHanging", isHanging);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isClimbing", isClimbing);
    }


    void ClearInput()
    {
        if (!readyToClear)
            return;

        input = 0;
        jumpPressed = false;
        dashPressed = false;
        downPressed = false;
        upPressed = false;
        attackPressed = false;
        readyToClear = false;
    }


    void GetInputs()
    {
        if (gamePaused) return;
        
        // obtenemos el movimiento horizontal del Input
        input = Input.GetAxisRaw("Horizontal");

        // para no perder ninguna pulsacion
        jumpPressed = jumpPressed || Input.GetButtonDown("Jump");
        downPressed = downPressed || Input.GetAxisRaw("Vertical") == -1f;
        upPressed = upPressed || Input.GetAxisRaw("Vertical") == 1f;
        dashPressed = dashPressed || Input.GetButtonDown("Dash");        
        attackPressed = attackPressed || Input.GetButtonDown("Attack");

        if (dashPressed && isMoving && canDash)
        {
            if (Time.time >= lastDash + dashCoolDown)
                AttemptToDash();
        }
    }


    void AttemptToDash()
    {
        isDashing = true;
        rb.gravityScale = 0f;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        ObjectPooler.Instance.SpawnFromPool("AfterImage");
        lastImageXPos = transform.position.x;        
    }


    void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft <= 0 || isTouching)
            {
                isDashing = false;
                rb.gravityScale = 4f;
                rb.velocity = Vector2.zero;
            }

            if (dashTimeLeft > 0)
            {
                rb.velocity = new Vector2(dirFacing * dashVelocity, 0f);
                dashTimeLeft -= Time.fixedDeltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXPos) > distanceBetweenImages)
                {
                    ObjectPooler.Instance.SpawnFromPool("AfterImage");
                    lastImageXPos = transform.position.x;
                }
            }            
        }
    }


    void CheckEnvironment()
    {
        // comprobamos si el player esta tocando el suelo
        Vector2 origin = new Vector2(groundCheck.position.x, groundCheck.position.y - smallAmount / 2);
        Vector2 size = new Vector2(boxCollider.bounds.size.x - smallAmount, smallAmount);
        isGrounded = Physics2D.OverlapBox(origin, size, 0f, groundLayer);

        // comprobamos si el player esta tocando un muro en la direccion en que esta mirando
        isTouching = Physics2D.Raycast(frontCheck.position, Vector2.right * dirFacing, boxCollider.bounds.extents.x + smallAmount, groundLayer);

        // si el player esta tocando el suelo o algun muro no esta saltando
        if (isGrounded || isTouching)
        {
            isJumping = false;
            jumpsCounter = 0;
        }            

        // comprobamos si el player puede agarrarse a un ledge        
        bool ledgeFound = Physics2D.Raycast(ledgeCheck.position, Vector2.right * dirFacing, boxCollider.bounds.extents.x + smallAmount * 2, groundLayer);
        bool eyesFound = Physics2D.Raycast(eyesCheck.position, Vector2.right * dirFacing, boxCollider.bounds.extents.x + smallAmount * 2, groundLayer);        

        if (eyesFound && !ledgeFound && !isGrounded && rb.velocity.y < 0 && !isHanging)
        {
            isHanging = true;
            rb.bodyType = RigidbodyType2D.Static;
        }

    }


    void GroundMovement()
    {
        if (isHanging)
            return;

        float xVelocity = input * speed;

        if (input * dirFacing < 0)
            Flip();

        if (isGrounded && !isCrouching && attackPressed && !isAttacking)
        {
            Attack();
        }

        // no queremos que cuando estemos en el aire se modifique la velocidad horizontal del Rigidbody
        // ni cuando estemos agachados
        if (isGrounded && !isDashing && !isCrouching && !isAttacking && rb.bodyType != RigidbodyType2D.Static)
        {            
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);
            coyoteTime = Time.time + coyoteDuration;
            isMoving = rb.velocity.x != 0;
        }                 
        
        if (isGrounded && !isMoving && downPressed)
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }

        // si pulsamos el boton de salto mientras estamos agachados y la plataforma solo tiene
        // un Tile de grosor la atravesamos hacia abajo
        if (isCrouching && jumpPressed)
        {
            // comprobamos el grosor del Tile que tenemos debajo
            if (CheckThickness())
            {
                // caemos, desactivando momentaneamente el collider del Player
                StartCoroutine(DeactivateCollider());
            }
        }

    }


    void AirMovement()
    {        
        if (isHanging)
        {
            // si pulsamos arriba subimos la plataforma
            if (upPressed)
            {
                isHanging = false;
                isClimbing = true;
            }
            else
            {                
                if (downPressed)
                {                    
                    isHanging = false;
                    canWallSliding = true;
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - smallAmount);
                }
            }

            if (jumpPressed)
            {
                isHanging = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
                AudioManager.Instance.Play("Jump");
                rb.AddForce(new Vector2(wallJumpDir.x * -dirFacing * wallJumpForce, wallJumpDir.y * wallJumpForce), ForceMode2D.Impulse);                
            }            
        }

        if (jumpPressed)
        {
            // doble salto: si ya estamos saltando
            if (canDoubleJump && isJumping && jumpsCounter < maxJumps)
            {
                isGrounded = false;
                isJumping = true;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpsCounter++;
            }
            else
            {
                // salto normal mientras estamos en el suelo o dura la ventana coyote
                if (!isCrouching && (isGrounded || coyoteTime > Time.time))
                {
                    isGrounded = false;
                    isJumping = true;
                    AudioManager.Instance.Play("Jump");
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    jumpsCounter++;
                }
            }
        }

        // si estamos tocando un muro, en el aire y pulsando hacia el muro
        // volvemos a permitir que se pueda hacer wallSliding
        if (isTouching && !isGrounded && rb.velocity.y < 0 && input == dirFacing)
            canWallSliding = true;

        // para poder hacer wallSliding tenemos que estar tocando un muro,
        // no tocar el suelo y estar descendiendo
        if (canSliding)
            isWallSliding = isTouching && !isGrounded && rb.velocity.y < 0 && canWallSliding;
        
        if (isWallSliding)
        {
            // reducimos la velocidad vertical
            if (rb.velocity.y < -maxSlidingSpeed)
                rb.velocity = new Vector2(rb.velocity.x, -maxSlidingSpeed);

            // si pulsamos el input hacia el muro mantenemos el wallSliding
            if (input == dirFacing)
            {
                canWallSliding = true;
                timeWallSliding = 0;
            }
            else
            // sino empezamos a contabilizar el tiempo que estamos deslizandonos,
            // si supera el tiempo maximo dejamos de deslizarnos
            {
                timeWallSliding += Time.fixedDeltaTime;
                if (timeWallSliding >= maxStickyTime)
                {
                    canWallSliding = false;
                    timeWallSliding = 0;
                }
            }
        }

        // si estamos en el aire no modificamos directamente la velocidad horizontal del Rigidbody
        // sino que aplicamos una fuerza constante, pero si ya tenia una velocidad, al sumarle la
        // fuerza constante se movera demasiado deprisa, debemos controlar que la velocidad no sea
        // superior a la velocidad establecida por la variable speed
        if (!isGrounded && !isWallSliding && input != 0 && !isDashing)
        {
            rb.AddForce(new Vector2(airMovingForce * input, 0f));
            if (Mathf.Abs(rb.velocity.x) > speed)
                rb.velocity = new Vector2(speed * input, rb.velocity.y);
        }

        // si estamos haciendo wallSliding y pulsamos Jump haremos un wallJump
        // debemos normalizar el Vector de direccion del wallJump pues no queremos que influya
        // en la fuerza del salto
        if (isWallSliding && jumpPressed)
        {
            AudioManager.Instance.Play("Jump");
            rb.AddForce(new Vector2(wallJumpDir.x * -dirFacing * wallJumpForce, wallJumpDir.y * wallJumpForce), ForceMode2D.Impulse);            
        }                 

        // si atacamos estando en el aire
        if (!isGrounded && !isWallSliding && !isDashing && !isAttacking && !isHanging && !isClimbing && attackPressed)
        {
            Attack();
        }
    }

    
    bool CheckThickness()
    {
        bool check;

        // comprobamos si el suelo tiene solo un Tile de grosor
        Vector2 origin = new Vector2(groundCheck.position.x, groundCheck.position.y - 0.75f);
        Vector2 size = new Vector2(boxCollider.bounds.size.x - smallAmount, smallAmount);
        check = Physics2D.OverlapBox(origin, size, 0f, groundLayer);

        // comprobamos que dos Tiles por debajo del Player haya un Tile
        Vector3Int tilePos = platform.WorldToCell(origin);
        Tile tile = platform.GetTile<Tile>(tilePos);

        // devolvemos el resultado del AND de las dos operaciones
        return (!check && tile == null);
    }


    IEnumerator DeactivateCollider()
    {
        boxCollider.enabled = false;
        yield return new WaitForSecondsRealtime(0.3f);
        boxCollider.enabled = true;
    }


    public IEnumerator PlayerBlink()
    {
        Color color;
        float alpha;

        for (int i = 0; i < 4; i++)
        {
            for (float j = 1; j > 0; j -= 0.1f)
            {
                alpha = j;
                color = new Color(1f, 1f, 1f, alpha);
                rend.color = color;
            }

            yield return new WaitForSecondsRealtime(0.1f);

            for (float j = 0; j <= 1; j += 0.1f)
            {
                alpha = j;
                color = new Color(1f, 1f, 1f, alpha);
                rend.color = color;
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }

        PlayerStats.Instance.canTakeDamage = true;
    }



    void Flip()
    {
        if (!isWallSliding && !isClimbing)
        {
            dirFacing *= -1;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }        
    }


    void Attack()
    {
        AudioManager.Instance.Play("Attack");
        isAttacking = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetTrigger("Attack");

        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsEnemy);
        foreach (Collider2D col in colliders)
        {            
            GameObject impact = Instantiate(swordImpactPrefab, col.transform.position, Quaternion.identity);
            Destroy(impact, 2);

            int direction = rb.position.x < col.transform.position.x ? 1 : -1;            
            col.gameObject.SendMessage("TakeDamage", direction);
        }
    }


    public void Step1()
    {
        AudioManager.Instance.Play("Step1");
    }


    public void Step2()
    {
        AudioManager.Instance.Play("Step2");
    }


    public void FinishClimb()
    {
        // colocamos al player justo encima del Tile que acaba de escalar
        Vector3Int cellPosition = platform.WorldToCell(new Vector3(frontCheck.position.x + (boxCollider.bounds.extents.x + smallAmount) * dirFacing, frontCheck.position.y, 0f));
        Vector3 pos = platform.GetCellCenterWorld(cellPosition);
        transform.position = new Vector3(pos.x, pos.y + cellSize / 2 + smallAmount, 0f);
        isClimbing = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }


    public void FinishAttack()
    {
        isAttacking = false;
    }


    void HandleOnGameStateChanged(GameManager.GameState state, GameManager.GameState previousState)
    {
        switch (state)
        {
            case GameManager.GameState.INITIALIZING:
                if (previousState == GameManager.GameState.DEATH)
                    Initialize();
                break;

            case GameManager.GameState.RUNNING:
                gamePaused = false;
                break;

            case GameManager.GameState.PAUSED:
                gamePaused = true;
                break;
            
            case GameManager.GameState.DEATH:
                PlayerDeath();
                break;

            default:
                break;
        }
    }


    void PlayerDeath()
    {        
        rb.velocity = Vector2.zero;        
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(groundCheck.position.x, groundCheck.position.y - 0.75f, 0), new Vector3(0.4f, smallAmount, 0));
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}

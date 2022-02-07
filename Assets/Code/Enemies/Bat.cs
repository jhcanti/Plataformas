using System.Collections;
using UnityEngine;

namespace Code.Enemies
{
    public class Bat : Enemy
    {
        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator anim;
        [SerializeField] private CircleCollider2D colliderEnemy;

        [Header("Environment")]
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private Transform frontCheck;
        [SerializeField] private Transform upCheck;
        [SerializeField] private Transform downCheck;
        [SerializeField] private float distanceRadius = .1f;

        private bool _isTouchingFront;
        private bool _isTouchingUp;
        private bool _isTouchingDown;
        private bool _isDead = false;
        private Vector2 _initPosition;
        private int _initDirFacing;

        [Header("Movement")]
        [SerializeField] private float speed = 2;
        [SerializeField] private int dirFacing = -1;
        [SerializeField] private Vector2 direction;
        
        private float _initSpeed;
        private Vector2 _initDirection;

        [Header("General")]
        [SerializeField] private GameObject soulEffectPrefab;    
        [SerializeField] private int health = 2;    
        [SerializeField] private float timeToRespawn = 15;

        
        protected override void DoStart()
        {
            _initDirFacing = dirFacing;
            _initPosition = rb.position;
            _initSpeed = speed;
            direction.Normalize();
            _initDirection = direction;
        }


        protected override void DoInitialize()
        {
            rb.position = _initPosition;
            dirFacing = _initDirFacing;
            speed = _initSpeed;
            direction = _initDirection;

            Vector3 scale = transform.localScale;
            scale.x = dirFacing * -1;
            transform.localScale = scale;

            rb.bodyType = RigidbodyType2D.Dynamic;
            colliderEnemy.enabled = true;
            health = 2;        
            isMoving = true;
            _isDead = false;
        }
        

        protected override void DoFixedUpdate()
        {
            if (GameManager.Instance.CurrentGameState != GameManager.GameState.RUNNING)
                return;
            
            if (!isMoving)
            {
                if (!_isTouchingDown || _isDead) return;
                
                _isDead = true;
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;
                anim.SetTrigger("Death");
                StartCoroutine(Respawn());

                return;
            }

            if (_isTouchingFront)
            {
                Flip();
                direction.x *= -1;
            }
        
            if (_isTouchingDown || _isTouchingUp)
            {
                direction.y *= -1;
            }

            if (rb.bodyType != RigidbodyType2D.Static)
                rb.velocity = speed * direction;
        }


        protected override void CheckEnvironment()
        {
            _isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, distanceRadius, whatIsGround);
            _isTouchingDown = Physics2D.OverlapCircle(downCheck.position, distanceRadius, whatIsGround);
            _isTouchingUp = Physics2D.OverlapCircle(upCheck.position, distanceRadius, whatIsGround);
        }


        private void Flip()
        {
            dirFacing *= -1;
            var scale = transform.localScale;
            scale.x = -dirFacing;
            transform.localScale = scale;
        }

        private void Death()
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1;
            colliderEnemy.enabled = false;
            isMoving = false;
            Instantiate(soulEffectPrefab, transform.position, Quaternion.identity);        
        }

        protected override void DoOnTriggerStay(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerStats.Instance.TakeDamage(1);
            }
        }
        
        public void TakeDamage(int direction)
        {
            health--;
            if (health > 0) return;
            
            AudioManager.Instance.Play("Bat_squeak");
            anim.SetTrigger("Vanish");            
            rb.bodyType = RigidbodyType2D.Static;
            colliderEnemy.enabled = false;
        }


        private IEnumerator Respawn()
        {
            yield return new WaitForSecondsRealtime(timeToRespawn);
            DoInitialize();
            yield return new WaitForSecondsRealtime(.1f);
            anim.SetTrigger("Appears");
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, .4f);
            Gizmos.DrawWireSphere(frontCheck.position, distanceRadius);
            Gizmos.DrawWireSphere(upCheck.position, distanceRadius);
            Gizmos.DrawWireSphere(downCheck.position, distanceRadius);
        }
    }
}


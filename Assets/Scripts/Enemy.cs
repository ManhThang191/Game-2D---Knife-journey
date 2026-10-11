using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Enemy : MonoBehaviour
{
    [SerializeField] private Image HpFill;
    
    private float currentHP;
    public Rigidbody2D rb;
    public Animator animator;
    
    public bool facingLeft = true;
    public bool inRange = false;
    private bool isDead = false;
    
    public float maxHealth = 10;
    public float moveSpeed = 2f;
    public float distance = 1f;
    public float distanceLeftRight = 1f;
    public float attackRange = 10f;
    public float retrieveDistance = 2.5f;
    public float chaseSpeed = 4f;
    public float attackRadius;
    public float damage = 3f;


    public Transform checkPoint;
    public Transform checkPointLeftRight;
    public Transform player;
    public Transform attackPoint;
    public Transform pointSpawnCoin;

    public LayerMask attackLayer;
    public LayerMask layerMask;

    public GameObject coinPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDead = false;
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        currentHP = maxHealth;
        UpdateHPBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }
        Move(); 
        // UpdateHPBar();
    }

    private void Move()
    {
        if(Vector2.Distance(transform.position, player.position) <= attackRange )
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }

        if (inRange)
        {
            
            if(Vector2.Distance(transform.position, player.position) > retrieveDistance)
            {
                if(player.position.x > transform.position.x)
                {
                    FlipToRight();
                }
                else
                {   
                    FlipToLeft();
                }
                animator.SetBool("Attack1", false);

                transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("Attack1", true);
            }
        }
        else
        {
            animator.SetBool("Attack1", false);
            transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);

            RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance, layerMask);
            RaycastHit2D hitRight = Physics2D.Raycast(checkPointLeftRight.position, Vector2.right, distanceLeftRight, layerMask);
            RaycastHit2D hitLeft = Physics2D.Raycast(checkPointLeftRight.position, Vector2.left, distanceLeftRight, layerMask);

            if(hit)
            {
                animator.SetBool("isWalk",true);    
            }
            if ( facingLeft && (!hit || hitLeft)  )
            {
                FlipToRight();  
            }
            else if (!facingLeft && (!hit || hitRight ) )
            {
                FlipToLeft();
            }
        }
        
    }

    public void Attack()
    {
        
        Collider2D collInfor = Physics2D.OverlapCircle(attackPoint.position,attackRadius, attackLayer);

        if (collInfor)
        {
            Player player = collInfor.gameObject.GetComponent<Player>();
            // Debug.Log(collInfor.transform.name);
            if(collInfor.gameObject.GetComponent<Player>() != null)
            {
                
                player.TakeDamage(damage);
                Animator playerAnimator = player.GetComponent<Animator>();
                playerAnimator.SetTrigger("PlayerHurt");
            }
        }
        
    }
    public void TakeDamage(float damage)
    {
         if (isDead)
        {
            return;
        }

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHealth);

        UpdateHPBar();

        if (currentHP <= 0f)
        {
            Died();
        }
    }

    
    private void UpdateHPBar()
    {
        if (HpFill == null)
        {
            return;
        }

        HpFill.fillAmount = maxHealth > 0f
            ? currentHP / maxHealth
            : 0f;
    }


    public void SpawnCoinAfterDied()
    {
        Debug.Log("spawn coin");
        Instantiate(coinPrefab, pointSpawnCoin.position, Quaternion.identity);
    }
    public void Died()
    {
        isDead = true;

        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        animator.SetBool("Died", true);
        // SpawnCoinAfterDied();
    }
    public void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }
    private void FlipToRight()
    {
        transform.eulerAngles = new Vector3(0,-180,0);
        facingLeft =false;
    }
    private void FlipToLeft()
    {
        transform.eulerAngles = new Vector3(0,0,0);
        facingLeft = true;
    }
    private void OnDrawGizmosSelected()
    {
        if(checkPoint == null)
        {
            return;
        }
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(checkPointLeftRight.position, Vector2.right * distanceLeftRight);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(checkPointLeftRight.position, Vector2.left * distanceLeftRight);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange );

        if(attackPoint == null) return;
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
      if(collision.gameObject.tag == "Player")
        {
            animator.SetTrigger("isAttack1");
            
        }  
    }

}

using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public Text healthText;
    
    public float maxHealth = 10;
    private float movement;
    public float jumpHeight = 1f;
    public float damage;
    
    [SerializeField] public float moveSpeed = 5f;
    public Rigidbody2D rb;

    public Animator animator;
    
    private bool isFacingRight =true;
    private bool isGround = true;

    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(maxHealth <= 0)
        {
            Died();
        }
        healthText.text = maxHealth.ToString();
        Move();
    }

    public void Attack()
    {

        Collider2D colliInfor =  Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (colliInfor)
        {
            Enemy enemy = colliInfor.gameObject.GetComponent<Enemy>();

            // Debug.Log(collInfor.transform.name);
            if(colliInfor.gameObject.GetComponent<Enemy>() != null)
            {
                enemy.TakeDamage(damage);
                Animator enemyAnimator = enemy.GetComponent<Animator>();
                enemyAnimator.SetTrigger("EnemyHurt");
            }
        }
    }
    private void Move()
    {
        movement = Input.GetAxisRaw("Horizontal");

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("isAttack");
        }

        if(movement != 0)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }

        if (movement < 0f && isFacingRight )
        {
            transform.eulerAngles = new Vector3(0f,180f,0f);
            isFacingRight = false;
            
        }else if(movement > 0f && !isFacingRight)
        {
            transform.eulerAngles = new Vector3(0f,0f,0f);
            isFacingRight = true;
        }

        
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
            animator.SetBool("isJump", true);

            isGround = false;
            
        }
    }
    public void TakeDamage(float damage)
    {
        if(maxHealth <= 0)
        {
            return;
        }
        maxHealth -= damage;
    }

    public void Died()
    {
        Debug.Log("Player Died!!");
    }
    private void FixedUpdate()
    {
        transform.position += new Vector3(movement,0f,0f) * Time.fixedDeltaTime * moveSpeed;
    }

    private void Jump()
    {
    
        rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            isGround = true;
            animator.SetBool("isJump", false);

        }
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position,attackRadius);

    }
}

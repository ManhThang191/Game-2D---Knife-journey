using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public Text currentHPText;
    public Text coinText;
    public Text maxHPText;
    [SerializeField] private Image hpFill;

    private float coin;
    public float maxHealth = 10;
    public float currentHP;
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
        maxHPText.text = maxHealth.ToString();
        currentHP = maxHealth;
        UpdateHPBar();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHP <= 0)
        {
            Died();
        }
        // healthText.text = maxHealth.ToString();
        currentHPText.text = currentHP.ToString();

        Move();
        UpdateHPBar();
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
        if(currentHP <= 0)
        {
            return;
        }
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHealth);
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        hpFill.fillAmount = currentHP / maxHealth;
    }
    public void Died()
    {
        Debug.Log("Player Died!!");
        FindAnyObjectByType<GameManager>().isGameActive = false;
        Destroy(this.gameObject);
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

        if(collision.gameObject.tag == "VictoryPoint")
        {
            
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            coin += 10;
            coinText.text = coin.ToString();
            collision.transform.GetChild(0).GetComponent<Animator>().SetTrigger("CollectCoin");

            // Debug.Log("Coin!");
            Destroy(collision.gameObject,2f);
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position,attackRadius);

    }
}

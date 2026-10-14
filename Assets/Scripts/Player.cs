using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    [Header("UI - Health")]
    [SerializeField] private Text currentHPText;
    [SerializeField] private Text maxHPText;
    [SerializeField] private Image hpFill;
    [SerializeField] private Image delayedHpFill;

    [Header("UI - Coins")]
    [SerializeField] private Text coinText;

    [Header("Health")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float currentHP = 10f;
    [SerializeField] private float damage;
    [SerializeField] private float damageDelay = 0.5f;
    [SerializeField] private float drainSpeed = 0.5f;

    private float delayedHP;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 1f;

    private float movement;
    private bool isFacingRight = true;
    private bool isGround = true;

    [Header("Physics & Animation")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private LayerMask attackLayer;

    [Header("Damage Popup")]
    [SerializeField] private DamePopup GameManagerDamePopup;
    [SerializeField] private Color damagePopupColor = Color.white;

    [Header("Currency")]
    [SerializeField] private float coin;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        maxHPText.text = maxHealth.ToString();

        currentHP = maxHealth;
        delayedHP = maxHealth;

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
        UpdateDelayedHPBar();
    }

    public void Attack()
    {

        Collider2D colliInfor =  Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (colliInfor)
        {
            Enemy enemy = colliInfor.gameObject.GetComponent<Enemy>();
            if (enemy == null) return;

            // Vector3 popupPosition = enemy.transform.position + new Vector3(0f, 1f, 0f);
            
            if(colliInfor.gameObject.GetComponent<Enemy>() != null)
            {
                GameManagerDamePopup.ShowDamage(damage, enemy.transform, damagePopupColor);
                
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
        UpdateDelayedHPBar();

        if (currentHP <= 0)
        {
            Died();
        }
    }

    private void UpdateHPBar()
    {
        hpFill.fillAmount = currentHP / maxHealth;
        if(currentHP <= (maxHealth / 100 * 30))
        {
            hpFill.color = Color.red;
        }
    }

    private void UpdateDelayedHPBar()
    {
        if (delayedHpFill == null || maxHealth <= 0) return;

        float targetHP = currentHP / maxHealth;

        delayedHpFill.fillAmount = Mathf.MoveTowards(
            delayedHpFill.fillAmount,
            targetHP,
            drainSpeed * Time.deltaTime
        );
    }

    public void Died()
    {
        Debug.Log("Player Died!!");
        GetComponent<Collider2D>().enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
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
            FindAnyObjectByType<ScenceManagement>().LoadLevel();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Trigger với: " + collision.name);
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

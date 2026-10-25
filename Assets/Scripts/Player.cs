using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    [Header("UI - Health")]
    [SerializeField] private Text currentHPText;
    [SerializeField] private Text maxHPText;
    [SerializeField] private Image hpFill;
    [SerializeField] private Image delayedHpFill;
    private Color originalHPColor; //59D937

    [Header("UI - Coins")]
    [SerializeField] private Text coinText;

    [Header("Health")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float currentHP = 10f;
    [SerializeField] private float damage;
    // [SerializeField] private float damageDelay = 0.5f;
    [SerializeField] private float drainSpeed = 0.5f;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float currentStamina;
    // [SerializeField] private float staminaDrainDelay = 0.5f;
    // [SerializeField] private float staminaDrainSpeed = 0.5f;

    [Header("Stamina For Each Activity")]
    [SerializeField] private float staminaForAttack;
    [SerializeField] private float staminaForMove;
    [SerializeField] private float staminaForJump;
    [SerializeField] private float staminaForUseSkill;
    [SerializeField] private float moveStaminaInterval = 0.2f;
    private float moveStaminaTimer;
    // [SerializeField] private float staminaForAttack;

    [Header("Stamina Recovery")]
    [SerializeField] private float staminaRecoveryRate = 2f;
    [SerializeField] private float staminaRecoveryDelay = 2f;
    private float staminaIdleTimer;


    [Header("UI Stamina")]
    [SerializeField] private Image staminaFill;
    [SerializeField] private Image delayedStaminaFill;
    private Color originalStaminaColor; //D7BB3C

    // private float delayedHP;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 1f;
    private float movement;
    private bool isFacingRight = true;
    private bool isGround = false;

    [Header("Physics & Animation")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Attack")]
    private bool isAttacking = false;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private LayerMask attackLayer;

    [Header("Damage Popup")]
    // [SerializeField] private InforNumberPopup inforNumberPopup;

    [SerializeField] private Color damagePopupColor = Color.white;

    [Header("Currency")]
    [SerializeField] private float coin;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        maxHPText.text = maxHealth.ToString();

        if(hpFill != null)
        {
            originalHPColor = hpFill.color;
        }

        if(staminaFill != null)
        {
            originalStaminaColor = staminaFill.color;
        }


        currentHP = maxHealth;
        // delayedHP = maxHealth;

        currentStamina = maxStamina;
        if (delayedStaminaFill != null)
        {
            delayedStaminaFill.fillAmount = 1f;
        }
        
        UpdateHPBar();
        UpdateStamina();
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
        HandleMoveStamina();
        animator.SetBool("isRun", movement != 0f);
        RecoverStamina();



        UpdateHPBar();
        UpdateDelayedHPBar();

        UpdateStamina();
        UpdateDelayedStaminaBar();
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
                // if (Input.GetMouseButtonDown(0))
                // {
                //     animator.SetTrigger("isAttack");
                //     // Attack();
                // }
                // inforNumberPopup.ShowDamage(damage, enemy.transform, damagePopupColor);
                PopupManagement.Instance.ShowDamage(damage, enemy.transform, damagePopupColor);
                
                enemy.TakeDamage(damage);

                Animator enemyAnimator = enemy.GetComponent<Animator>();

                enemyAnimator.SetTrigger("EnemyHurt");
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }
    private void Move()
    {
        movement = Input.GetAxisRaw("Horizontal");

        if (Input.GetMouseButtonDown(0) )
        {
            if (TryUseStamina(staminaForAttack))
            {
                isAttacking = true;
                animator.SetTrigger("isAttack");
            }
        }

        // if(movement != 0)
        // {
        //     animator.SetBool("isRun", true);
        // }
        // else
        // {
        //     animator.SetBool("isRun", false);
        // }

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
            if (!TryUseStamina(staminaForJump))
            {
                return;
            }


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
        if (hpFill == null || maxHealth <= 0f) return;

        float hpPercent = currentHP / maxHealth;

        hpFill.fillAmount = hpPercent;

        if (hpPercent <= 0.1f)
        {
            hpFill.color = Color.red;
        }
        else if (hpPercent <= 0.4f)
        {
            hpFill.color = new Color(1f, 0.5f, 0f); // Cam
        }
        else
        {
            hpFill.color = originalHPColor;
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

    private void UpdateStamina()
    {
        if(staminaFill == null) return;

        float staminaPercent = currentStamina / maxStamina;

        staminaFill.fillAmount = staminaPercent;

        if (staminaPercent <= 0.1f)
        {
            staminaFill.color = Color.red;
        }
        else if (staminaPercent <= 0.4f)
        {
            staminaFill.color = new Color(1f, 0.5f, 0f); // Cam
        }
        else
        {
            staminaFill.color = originalStaminaColor;
        }

    }

    

    private void UpdateDelayedStaminaBar()
    {
        if(delayedStaminaFill == null) return;
        if (delayedStaminaFill == null || maxStamina <= 0) return;

        float targetStamina = currentStamina / maxStamina;

        delayedStaminaFill.fillAmount = Mathf.MoveTowards(
            delayedStaminaFill.fillAmount,
            targetStamina,
            drainSpeed * Time.deltaTime
        );
    }

    private bool TryUseStamina(float staminaTaken)
    {
        if (currentStamina < staminaTaken)
        {
            return false;
        }

        currentStamina -= staminaTaken;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        UpdateStamina();
        UpdateDelayedStaminaBar();

        return true;
    }

    private void HandleMoveStamina()
    {
        // Không di chuyển thì không tiêu hao stamina
        if (movement == 0f)
        {
            moveStaminaTimer = 0f;
            return;
        }

        // Không đủ stamina thì dừng di chuyển
        if (currentStamina <= 0f)
        {
            movement = 0f;
            return;
        }

        moveStaminaTimer += Time.deltaTime;

        if (moveStaminaTimer >= moveStaminaInterval)
        {
            // Kiểm tra stamina trước khi trừ
            if (currentStamina >= staminaForMove)
            {
                TryUseStamina(staminaForMove);
            }
            else
            {
                // Không đủ stamina cho lần di chuyển tiếp theo
                currentStamina = 0f;
                movement = 0f;
            }

            moveStaminaTimer = 0f;
        }
    }

    private void RecoverStamina()
    {
        // Nếu Player đang di chuyển hoac dang tan cong
        if (movement != 0f || isAttacking || !isGround )
        {
            staminaIdleTimer = 0f;
            return;
        }

        // Đã đầy stamina thì không cần hồi
        if (currentStamina >= maxStamina)
        {
            staminaIdleTimer = 0f;
            return;
        }

        // Đếm thời gian đứng yên
        staminaIdleTimer += Time.deltaTime;

        // Chưa đứng yên đủ thời gian chờ
        if (staminaIdleTimer < staminaRecoveryDelay)
        {
            return;
        }

        // Bắt đầu hồi stamina
        currentStamina += staminaRecoveryRate * Time.deltaTime;

        currentStamina = Mathf.Clamp(
            currentStamina,
            0f,
            maxStamina
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
            collision.GetComponent<Collider2D>().enabled = false;
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


using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator animator;
    [SerializeField] private Image hpFill;

    [Header("Health")]
    [SerializeField] protected float maxHealth = 10f;
    private float currentHP;
    private bool isDead;

    public bool IsDead => isDead;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float chaseSpeed = 4f;
    [SerializeField] protected float attackRange = 10f;
    [SerializeField] protected float retrieveDistance = 2.5f;

    [SerializeField] protected float distance = 1f;
    [SerializeField] protected float distanceLeftRight = 1f;

    protected bool facingLeft = true;
    protected bool inRange;

    [Header("Attack")]
    [SerializeField] protected float damage = 3f;
    [SerializeField] protected float attackRadius = 0.5f;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask attackLayer;

    [Header("Detection")]
    [SerializeField] protected Transform checkPoint;
    [SerializeField] protected Transform checkPointLeftRight;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected Transform player;

    [Header("Damage Popup")]
    [SerializeField] private InforNumberPopup inforNumberPopup;
    [SerializeField] private Color damagePopupColor = Color.white;

    [Header("Drop Item")]
    [SerializeField] protected GameObject coinPrefab;
    [SerializeField] protected Transform pointSpawnCoin;

    [Header("Audio")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip attackMissClip;

    protected virtual void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        currentHP = maxHealth;
        isDead = false;

        UpdateHPBar();
    }

    protected virtual void Update()
    {
        if (isDead)
            return;

        Move();
    }

    // Hanh vi di chuyen mac dinh.
    // Lop con co the override neu co AI rieng.
    protected virtual void Move()
    {
        if (player == null)
            return;

        inRange = Vector2.Distance(
            transform.position,
            player.position
        ) <= attackRange;

        if (inRange)
        {
            ChaseOrAttack();
        }
        else
        {
            Patrol();
        }
    }

    protected virtual void ChaseOrAttack()
    {
        float distanceToPlayer = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distanceToPlayer > retrieveDistance)
        {
            if (player.position.x > transform.position.x)
                FlipToRight();
            else
                FlipToLeft();

            animator.SetBool("Attack1", false);

            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                chaseSpeed * Time.deltaTime
            );
        }
        else
        {
            animator.SetBool("Attack1", true);
        }
    }

    protected virtual void Patrol()
    {
        animator.SetBool("Attack1", false);

        transform.Translate(
            Vector2.left * moveSpeed * Time.deltaTime
        );

        if (checkPoint == null || checkPointLeftRight == null)
            return;

        RaycastHit2D hit = Physics2D.Raycast(
            checkPoint.position,
            Vector2.down,
            distance,
            layerMask
        );

        RaycastHit2D hitRight = Physics2D.Raycast(
            checkPointLeftRight.position,
            Vector2.right,
            distanceLeftRight,
            layerMask
        );

        RaycastHit2D hitLeft = Physics2D.Raycast(
            checkPointLeftRight.position,
            Vector2.left,
            distanceLeftRight,
            layerMask
        );

        // animator.SetBool("isWalk", hit);

        if (facingLeft && (!hit || hitLeft))
            FlipToRight();
        else if (!facingLeft && (!hit || hitRight))
            FlipToLeft();
    }

    // Duoc goi tu Animation Event cua animation tan cong.
    public virtual void Attack()
    {
        if (isDead || attackPoint == null)
            return;

        Collider2D hit = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            attackLayer
        );

        if (hit == null)
        {
            AudioManager.Instance?.PlaySFX(attackMissClip);
            return;
        }

        if(hit.gameObject.GetComponent<Player>() != null)
        {
            AudioManager.Instance?.PlaySFX(attackClip);
        };
        

        if (hit == null)
            return;

        Player target = hit.GetComponent<Player>();

        if (target == null)
            return;

        target.TakeDamage(damage);

        if (inforNumberPopup != null)
        {
            inforNumberPopup.ShowDamage(
                damage,
                target.transform,
                damagePopupColor
            );
        }

        Animator targetAnimator = target.GetComponent<Animator>();

        if (targetAnimator != null)
            targetAnimator.SetTrigger("PlayerHurt");
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead || amount <= 0f)
            return;

        currentHP = Mathf.Clamp(
            currentHP - amount,
            0f,
            maxHealth
        );

        UpdateHPBar();

        if (currentHP <= 0f)
            Died();
    }

    protected virtual void UpdateHPBar()
    {
        if (hpFill == null)
            return;

        hpFill.fillAmount = maxHealth > 0f
            ? currentHP / maxHealth
            : 0f;
    }

    public virtual void Died()
    {
        if (isDead)
            return;

        isDead = true;

        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (animator != null)
            animator.SetBool("Died", true);
    }

    // Goi tu Animation Event khi animation chet ket thuc.
    public virtual void SpawnCoinAfterDied()
    {
        if (coinPrefab == null || pointSpawnCoin == null)
            return;

        Instantiate(
            coinPrefab,
            pointSpawnCoin.position,
            Quaternion.identity
        );
    }

    // Goi tu Animation Event khi animation chet ket thuc.
    public virtual void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    protected void FlipToRight()
    {
        transform.eulerAngles = new Vector3(0f, -180f, 0f);
        facingLeft = false;
    }

    protected void FlipToLeft()
    {
        transform.eulerAngles = Vector3.zero;
        facingLeft = true;
    }

    protected virtual void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (isDead || !collision.gameObject.CompareTag("Player"))
            return;

        if (animator != null)
            animator.SetTrigger("isAttack1");
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (checkPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(
                checkPoint.position,
                Vector2.down * distance
            );
        }

        if (checkPointLeftRight != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(
                checkPointLeftRight.position,
                Vector2.right * distanceLeftRight
            );

            Gizmos.color = Color.green;
            Gizmos.DrawRay(
                checkPointLeftRight.position,
                Vector2.left * distanceLeftRight
            );
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(
                attackPoint.position,
                attackRadius
            );
        }
    }
}

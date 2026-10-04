using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxHealth = 10;
    private float movement;
    public Rigidbody2D rb;
    [SerializeField] public float moveSpeed = 5f;
    public float jumpHeight = 1f;
    private bool isFacingRight =true;
    private bool isGround = true;
    public Animator animator;

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
        Move();
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
}

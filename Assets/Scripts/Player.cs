using UnityEngine;

public class Player : MonoBehaviour
{
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
        
    }

    // Update is called once per frame
    void Update()
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

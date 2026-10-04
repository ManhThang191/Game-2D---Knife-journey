using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool facingLeft = true;
    public float moveSpeed = 2f;
    public Transform checkPoint;
    public Transform checkPointLeftRight;
    public float distance = 1f;
    public float distanceLeftRight = 1f;
    public LayerMask layerMask;
    public Animator animator;
    public bool inRange = false;
    public Transform player;
    public float attackRange = 10f;
    public float retrieveDistance = 2.5f;
    public float chaseSpeed = 4f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move();
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
            Move();
        }
    }

    private void Move()
    {
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
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
      if(collision.gameObject.tag == "Player")
        {
            animator.SetTrigger("isAttack1");
            animator.SetTrigger("isHurt");

        }  
    }
}

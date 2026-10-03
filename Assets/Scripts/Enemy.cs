using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool facingLeft = true;
    public float moveSpeed = 2f;
    public Transform checkPoint;
    public float distance = 1f;
    public LayerMask layerMask;
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);

        RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance, layerMask);

        if(hit)
        {
        animator.SetBool("isWalk",true);
            
        }
        if (!hit && facingLeft )
        {
            transform.eulerAngles = new Vector3(0,-180,0);
            facingLeft =false;
        }
        else if (!hit && !facingLeft)
        {
            transform.eulerAngles = new Vector3(0,0,0);
            facingLeft = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(checkPoint == null)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);
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

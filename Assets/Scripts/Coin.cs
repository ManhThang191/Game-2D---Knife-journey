using UnityEngine;

public class Coin : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;


    [Header("Logic Collect")]

    private bool isCollected;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public bool Collect()
    {
        if (isCollected) return false;

        isCollected = true;

        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetTrigger("CollectCoin");
        }

        return true;
    }
    public void Destroy()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        Destroy(transform.parent != null
        ? transform.parent.gameObject
        : gameObject);
    }
}

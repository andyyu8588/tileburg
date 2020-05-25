using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Player configuration
    [SerializeField] float runSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float climbSpeed;
    [SerializeField] Vector2 deathKick;

    // Player state
    bool isAlive = true;

    // Player components
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myCapsuleCollider;
    BoxCollider2D myBoxCollider;
    float gravityScale;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        gravityScale = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Die();
        if (isAlive)
        {
        Movement();
        }
    }

    private void Movement() {
        Run();
        Climb();
        Jump();
        FlipSprite();
    }

    // Player run function
    private void Run()
    {
        // Set horizontal velocity
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // value between -1 and 1
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        // Change animation
        bool hasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Walk", hasHorizontalSpeed);
    }

    // Jumping
    private void Jump()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump") && myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            Vector2 jumpVelocity = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocity;            
        }
    }

    // Flips player left and right
    private void FlipSprite()
    {
        // horizontal movement
        bool hasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        float knightScale = 0.3870969f;
        
        if (hasHorizontalSpeed) 
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x) * knightScale, transform.localScale.y);
        }
    }

    // Climbing ladders
    private void Climb()
    {
        if (myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            // vertical movement
            float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
            Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
            myRigidBody.velocity = climbVelocity;

            // set animation
            bool hasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
            myAnimator.SetBool("Climb", true);
            myRigidBody.gravityScale = 0f;
        } 
        else 
        {
            myAnimator.SetBool("Climb", false);
            myRigidBody.gravityScale = gravityScale;
        }
    }

    private void Die()
    {
        if (myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Death");
            GetComponent<Rigidbody2D>().velocity = deathKick;
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;


public class SkeletonNormal : Enemy
{
    

    private Rigidbody2D myRigidBody;
    public Transform target;
    public float chaseRadius;
    public float attackRadius;
    public Transform homePosition;
    public Animator anim;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (currentState != EnemyState.dead)
        {
            currentState = EnemyState.idle;
        }
        myRigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (currentState == EnemyState.stagger && health > 0)
        {
            anim.SetBool("hit", true);
        }
        else
        {
            anim.SetBool("hit", false);
        }
    }



    void FixedUpdate()
    {
        if (currentState == EnemyState.dead)
        {
            myRigidBody.linearVelocity = Vector2.zero;
            myRigidBody.bodyType = RigidbodyType2D.Kinematic;
            return;
        
        }
        CheckDistance();
    }
    void CheckDistance()
    {
        if (anim.GetBool("isDead") == false && health > 0)
        {
            if (Vector3.Distance(target.position, transform.position) <= chaseRadius && Vector3.Distance(target.position, transform.position) > attackRadius)
            {
                if (currentState == EnemyState.idle || currentState == EnemyState.walk && currentState != EnemyState.stagger)
                {
                    Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                    changeAnim(temp - transform.position);
                    myRigidBody.MovePosition(temp);
                    ChangeState(EnemyState.walk);
                    anim.SetBool("moving", true);
                }
            }
            else
            {
                anim.SetBool("moving", false);
            }
        }
    }

    private void setAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY", setVector.y);
    }
    private void changeAnim(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                setAnimFloat(Vector2.right);
            }
            else if (direction.x < 0)
            {
                setAnimFloat(Vector2.left);
            }
        }
        else if (Mathf.Abs(direction.x) <= Mathf.Abs(direction.y))
        {
            if (direction.y > 0)
            {
                setAnimFloat(Vector2.up);
            }
            else if (direction.y < 0)
            {
                setAnimFloat(Vector2.down);
            }
        }
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == EnemyState.dead)
        {
            return; // Prevent changing state if already dead
        }
        if (currentState != newState)
        {
            currentState = newState;
        }
    }

    // Call this method when the skeleton takes damage and dies
    protected override void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && currentState != EnemyState.dead) // Check if already dead to prevent multiple death triggers)
        {
            currentState = EnemyState.dead;
            anim.SetBool("moving", false);
            anim.SetTrigger("dead");
            anim.SetBool("isDead", true);
            DieSound(); // Call parent function to play the death sound

            // Disable all colliders
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = false;
            }

            StartCoroutine(PlayDeathAnimationAndDisable()); // Play death animation and then disable the enemy
        }
        else
        {
            HitSound(); // Call parent function to play the hit sound
        }
    }

    // Coroutine to wait for the death animation to finish and then disable the object
    private IEnumerator PlayDeathAnimationAndDisable()
    {
        // Wait for the death animation to finish before disabling the object
        float deathAnimationDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        float extendedDuration = deathAnimationDuration + 3.0f;
        yield return new WaitForSeconds(extendedDuration);

        // Stop the animator so it stays on the last frame
        anim.enabled = false;

        // Disable the game object after the animation completes
        this.gameObject.SetActive(false);
    }
}

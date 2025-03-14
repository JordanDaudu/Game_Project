using System.Collections;
using UnityEngine;

public class LogNormal : Enemy
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

    // Update is called once per frame
    void Update()
    {
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
        if (anim.GetBool("isDead") == false)
        {
            if (Vector3.Distance(target.position, transform.position) <= chaseRadius && Vector3.Distance(target.position, transform.position) > attackRadius)
            {
                if (currentState == EnemyState.idle || currentState == EnemyState.walk && currentState != EnemyState.stagger)
                {
                    Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                    changeAnim(temp - transform.position);
                    myRigidBody.MovePosition(temp);
                    ChangeState(EnemyState.walk);
                    anim.SetBool("wakeUp", true);
                }
            }
            else
            {
                anim.SetBool("wakeUp", false);
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

    protected override void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && currentState != EnemyState.dead) // Check if already dead to prevent multiple death triggers)
        {
            currentState = EnemyState.dead;
            anim.SetBool("wakeUp", false);
            anim.SetTrigger("dead");
            anim.SetBool("isDead", true);

            // Disable all colliders
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = false;
            }

            StartCoroutine(PlayDeathAnimationAndDisable()); // Play death animation and then disable the enemy
        }
    }

    // Coroutine to wait for the death animation to finish and then disable the object
    private IEnumerator PlayDeathAnimationAndDisable()
    {
        // Wait for the death animation to finish before disabling the object
        float deathAnimationDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        float extendedDuration = deathAnimationDuration;
        yield return new WaitForSeconds(extendedDuration);

        // Stop the animator so it stays on the last frame
        anim.enabled = false;

        // Disable the game object after the animation completes
        this.gameObject.SetActive(false);
    }
}

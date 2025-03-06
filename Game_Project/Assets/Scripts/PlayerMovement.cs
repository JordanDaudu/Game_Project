using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    idle,
    walk,
    attack,
    interact,
    stagger,
    lowHealth,
    dead
}
public class PlayerMovement : MonoBehaviour
{
    public PlayerState currentState;
    public float speed = 5f;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public FloatValue currentHealth;
    public Signal playerHealthSignal;

    void Start()
    {
        currentState = PlayerState.walk;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);

    }

    void Update()
    {
        change = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        if (change != Vector3.zero)
        {
            change.Normalize(); // Prevents diagonal speed boost
        }

        if (Input.GetButtonDown("Attack") && currentState != PlayerState.attack
            && currentState != PlayerState.stagger)
        {
            StartCoroutine(AttackCo());
        }
        else if(currentState == PlayerState.idle || currentState == PlayerState.walk || currentState == PlayerState.lowHealth)
        {
            UpdateAnimation();
        }
        else if(currentState == PlayerState.stagger)
        {
            animator.SetTrigger("hit");
        }
    }

    private IEnumerator AttackCo()
    {
        animator.SetBool("attacking", true);
        currentState = PlayerState.attack;
        yield return null;
        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.33f);
        currentState = PlayerState.walk;
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    void UpdateAnimation()
    {
        if (currentState == PlayerState.attack)
        {
            return;
        }
        if (currentState == PlayerState.stagger)
        {

        }
        if (currentHealth.RuntimeValue <= 2)
        {
            animator.SetBool("lowHealth", true);
        }
        else
        {
            animator.SetBool("lowHealth", false);
        }
        if (change != Vector3.zero)
        {
            spriteRenderer.flipX = (change.x < 0);
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    void MoveCharacter()
    {
        if(currentState == PlayerState.idle || currentState == PlayerState.walk)
        {
            myRigidbody.MovePosition(myRigidbody.position + (Vector2)change * speed * Time.fixedDeltaTime);
        }
    }

    public void Knock(float knockTime, float damage)
    {
        currentHealth.RuntimeValue -= damage;
        playerHealthSignal.Raise();
        if (currentHealth.RuntimeValue > 0)
        {
            StartCoroutine(KnockCo(knockTime));
        }
        else
        {
            currentState = PlayerState.dead;
            animator.SetBool("moving", false);
            animator.SetTrigger("dead");
            animator.SetBool("isDead", true);

            // Disable all colliders
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = false;
            }

            // Freeze Rigidbody's movement
            myRigidbody.linearVelocity = Vector2.zero;  // Stop the movement immediately
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll; // Freeze all physics movement

            StartCoroutine(PlayDeathAnimationAndDisable());
        }
    }

    private IEnumerator KnockCo(float knockTime)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.linearVelocity = Vector2.zero;
            currentState = PlayerState.idle;
            myRigidbody.linearVelocity = Vector2.zero;
        }
    }

    // Coroutine to wait for the death animation to finish and then disable the object
    private IEnumerator PlayDeathAnimationAndDisable()
    {
        // Wait for the death animation to finish before disabling the object
        float deathAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float extendedDuration = deathAnimationDuration + 5.0f;
        yield return new WaitForSeconds(extendedDuration);

        // Stop the animator so it stays on the last frame
        animator.enabled = false;

        // Disable the game object after the animation completes
        this.gameObject.SetActive(false);
    }
}

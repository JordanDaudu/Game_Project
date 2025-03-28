using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum PlayerState
{
    idle,
    walk,
    attack,
    interact,
    stagger,
    lowHealth,
    roll,
    dead
}
public class PlayerMovement : MonoBehaviour
{
    public InputSystem_Actions playerControls;
    private InputAction move;
    private InputAction attack;
    private InputAction interact;
    private InputAction roll;
    private InputAction openMenu;
    Vector2 moveDirection = Vector2.zero;
    public bool CanBeHit = true; // Prevents multiple hits  
    public float hitCooldown = 0.5f;    // Time before another hit is registered

    private AudioSource audioSource;
    public AudioClip[] attackSounds;


    public PlayerState currentState;
    private Vector2 rollDirection;
    public float rollSpeed = 10f;  // Speed of the roll
    public float speed = 5f;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public FloatValue currentHealth;
    public Signal playerHealthSignal;
    public VectorValue startingPosition;
    public MainMenu mainMenu; // Menu UI reference

    // Weapons variables
    public Animator weaponAnimator;
    private SpriteRenderer weaponSprite;

    private void Awake()
    {
        playerControls = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        attack = playerControls.Player.Attack;
        attack.Enable();
        attack.performed += Attack;

        interact = playerControls.Player.Interact;
        interact.Enable();

        roll = playerControls.Player.Roll;
        roll.Enable();
        roll.performed += Roll;

        openMenu = playerControls.Player.OpenMenu;
        openMenu.Enable();
        openMenu.performed += OpenMenu;
    }
    private void OnDisable()
    {
        move.Disable();
        attack.Disable();
        interact.Disable();
        roll.Disable();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentState = PlayerState.walk;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
        transform.position = startingPosition.initialValue;
        if(startingPosition.isFromTransition)
        {
            if (startingPosition.facingDirectionX != 0)
            {
                animator.SetFloat("moveX", startingPosition.facingDirectionX);
            }
            if (startingPosition.facingDirectionY != 0)
            {
                animator.SetFloat("moveY", startingPosition.facingDirectionY);
            }
        }
        // This if statement is to get the weapon game object sprite
        if(weaponAnimator !=  null)
        {
            weaponSprite = weaponAnimator.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        if(currentState == PlayerState.idle || currentState == PlayerState.walk || currentState == PlayerState.lowHealth)
        {
            UpdateAnimation();
        }
        else if(currentState == PlayerState.stagger)
        {
            animator.SetTrigger("hit");
        }

        // testing purposes REMOVE LATER
        if(Input.GetKeyDown("q"))
        {
            currentHealth.RuntimeValue += 1;
            playerHealthSignal.Raise();
        }
    }

    private IEnumerator AttackCo()
    {
        animator.SetBool("attacking", true);
        currentState = PlayerState.attack;

        if (audioSource != null && attackSounds.Length > 0)
        {
            // Pick a random index from the attackSounds array
            int randomIndex = Random.Range(0, attackSounds.Length);

            // Play the randomly selected attack sound
            audioSource.PlayOneShot(attackSounds[randomIndex]);
        }

        moveDirection = Vector2.zero;
        myRigidbody.linearVelocity = Vector2.zero;

        yield return null;
        animator.SetBool("attacking", false);

        // This is code added for weapon game object animation
        if(weaponAnimator != null)
        {
            weaponAnimator.SetTrigger("Attack");
            weaponSprite.enabled = true;
        }
        // Note: THE YIELD IS NOT PART OF THE WEAPON ANIMATION
        yield return new WaitForSeconds(.33f);
        if(weaponAnimator != null) // deactivation part of the weapon game obect
        {
            weaponSprite.enabled = false; // De-Activate the weapon animator sprite
        }

        currentState = PlayerState.walk;
    }

    private IEnumerator RollCo()
    {
        if (animator.GetBool("isRolling") == true) yield break;  // Prevent multiple rolls at once

        animator.SetBool("isRolling", true);
        currentState = PlayerState.roll;


        // Store the current movement direction
        if (moveDirection != Vector2.zero)
        {
            rollDirection = moveDirection.normalized;  // Ensure it's always a unit vector
        }
        else
        {
            rollDirection = new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY")).normalized;
        }

        // Apply roll velocity
        myRigidbody.linearVelocity = rollDirection * rollSpeed;

        // Wait until the animation actually starts before getting duration
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Rolling"));

        float rollDuration = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(rollDuration);  // Wait for the roll to finish

        // Reset movement
        animator.SetBool("isRolling", false);
        myRigidbody.linearVelocity = Vector2.zero;
        currentState = PlayerState.walk;
    }
    void FixedUpdate()
    {
        MoveCharacter();
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (currentState != PlayerState.attack && currentState != PlayerState.stagger && currentState != PlayerState.roll)
        {
            StartCoroutine(AttackCo());
        }
    }

    private void Roll(InputAction.CallbackContext context)
    {
        if (currentState != PlayerState.attack && currentState != PlayerState.stagger && currentState != PlayerState.roll)
        {
            StartCoroutine(RollCo());
        }
    }

    private void OpenMenu(InputAction.CallbackContext context)
    {
        if(mainMenu != null)
        {
            mainMenu.Menu();
        }
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
        if (moveDirection != Vector2.zero)
        {
            animator.SetFloat("moveX", moveDirection.x);
            animator.SetFloat("moveY", moveDirection.y);
            animator.SetBool("moving", true);
            
            // This if statement is for the Weapon direction animations
            if(weaponAnimator != null)
            {
                weaponAnimator.SetFloat("moveX", moveDirection.x);
                weaponAnimator.SetFloat("moveY", moveDirection.y);
            }
        }
        else
        {
            animator.SetBool("moving", false);
            currentState = PlayerState.idle;
        }
    }

    void MoveCharacter()
    {
        if(currentState == PlayerState.idle || currentState == PlayerState.walk)
        {
            currentState = PlayerState.walk;
            myRigidbody.linearVelocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
        }
    }

    public void Knock(float knockTime, float damage)
    {
        currentHealth.RuntimeValue -= damage;
        playerHealthSignal.Raise();
        if (currentHealth.RuntimeValue > 0 && CanBeHit)
        {
            CanBeHit = false;  
        currentState = PlayerState.stagger; 
        StartCoroutine(KnockCo(knockTime));
        StartCoroutine(HitCooldown());
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
    private IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(hitCooldown);
        CanBeHit = true;
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

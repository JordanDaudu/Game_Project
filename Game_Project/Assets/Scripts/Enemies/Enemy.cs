using System.Collections;
using UnityEngine;

public enum EnemyState
{
    idle,
    walk,
    attack,
    stagger,
    dead
}

public class Enemy : MonoBehaviour
{
    public EnemyState currentState;
    public FloatValue maxHealth;
    public float health;
    public string enemyName;
    public int baseAttack;
    public float moveSpeed;

    public bool CanBeHit = true;  // Prevents multiple hits
    public float hitCooldown = 0.3f; // Time before another hit is registered

    public AudioSource audioSource; // Handles sound playback
    public AudioClip deathSound; // Assign in Inspector
    public AudioClip[] hitSound; // Assign in Inspector
    public AudioClip[] walkingSound; // Assign in Inspector
    public AudioClip[] idleSound; // Assign in Inspector

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        health = maxHealth.initialValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    protected virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && currentState != EnemyState.dead) // Check if already dead to prevent multiple death triggers
        {
            currentState = EnemyState.dead;
            this.gameObject.SetActive(false);
        }
    }

   

    public void Knock(Rigidbody2D myRigidbody, float knockTime, float damage)
{
    if (CanBeHit) // Ensure only one hit is registered per attack
    {
        CanBeHit = false;  // Start cooldown
        currentState = EnemyState.stagger; // Ensure the enemy is staggered
        TakeDamage(damage);
        StartCoroutine(KnockCo(myRigidbody, knockTime));
        StartCoroutine(HitCooldown());
    }
}

private IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime)
{
    if (myRigidbody != null)
    {
        yield return new WaitForSeconds(knockTime);
        myRigidbody.linearVelocity = Vector2.zero; // Reset movement
        currentState = EnemyState.idle; // Reset state
    }
}

private IEnumerator HitCooldown()
{
    yield return new WaitForSeconds(hitCooldown);
    CanBeHit = true; // Allow future hits
}

    protected virtual void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    protected virtual void DieSound()
    {
        PlaySound(deathSound);
    }
    protected virtual void HitSound()
    {
        // Pick a random index from the attackSounds array
            int randomIndex = Random.Range(0, hitSound.Length);

            // Play the randomly selected attack sound
            PlaySound(hitSound[randomIndex]);
    }
}

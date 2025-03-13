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
            //currentState = EnemyState.dead;
        }
    }

   

    public void Knock(Rigidbody2D myRigidbody, float knockTime, float damage)
    {
        StartCoroutine(KnockCo(myRigidbody, knockTime));
        TakeDamage(damage);
    }

    private IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.linearVelocity = Vector2.zero;
            currentState = EnemyState.idle;
            myRigidbody.linearVelocity = Vector2.zero;
        }
    }
}

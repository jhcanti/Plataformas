using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyData data;
    public Animator anim;
    Rigidbody2D rbEnemy;
    Collider2D colliderEnemy;
    public bool isKnockBacking = false;
    public float timeKnockBacking = 0.5f;
    public int health;


    private void Start()
    {
        rbEnemy = GetComponent<Rigidbody2D>();
        colliderEnemy = GetComponent<Collider2D>();
        health = data.lives;
    }

    public void TakeDamage(int direction, float force)
    {
        health--;
        if (health <= 0)
        {
            anim.SetTrigger("Vanish");
            rbEnemy.bodyType = RigidbodyType2D.Static;
            colliderEnemy.enabled = false;
        }
        else
        {
            KnockBackEnemy(direction, force);
        }
    }


    public void KnockBackEnemy(int direction, float force)
    {
        isKnockBacking = true;
        rbEnemy.velocity = new Vector2(0, rbEnemy.velocity.y);
        rbEnemy.AddForce(Vector2.right * direction * force, ForceMode2D.Impulse);
        StartCoroutine(KnockBacking());
    }


    IEnumerator KnockBacking()
    {
        yield return new WaitForSecondsRealtime(timeKnockBacking);
        isKnockBacking = false;
    }
}

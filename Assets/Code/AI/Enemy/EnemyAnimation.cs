using SoulHunter.Enemy;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour // Mort
{
    EnemyBase enemyBase;
    EnemyCombat enemyCombat;
    EnemyScript movement;

    SpriteRenderer spriteRenderer;

    Animator anim;

    private void Awake()
    {
        enemyBase = GetComponentInParent<EnemyBase>();
        enemyCombat = GetComponentInParent<EnemyCombat>();
        movement = GetComponentInParent<EnemyScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        UpdateValues();
        FlipTexture();
    }

    /// <summary>
    /// Updates animator values
    /// </summary>
    void UpdateValues()
    {
        // Syncs animator booleans with static booleans
        anim.SetBool("isAttacking", enemyCombat.attacking);
        anim.SetBool("isDead", enemyBase.isDead);
        anim.SetBool("isHurt", false);

        // Syncs animator speed with enemy speed
        anim.SetFloat("Speed", Mathf.Abs(movement.currentSpeed));
    }

    /// <summary>
    /// Flips enemy sprite according to its movement direction
    /// </summary>
    void FlipTexture()
    {
        if (movement.Moving)
        {
            if (movement.MoveRight)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    /// <summary>
    /// Plays hurt animation once
    /// </summary>
    public void HurtAnimation()
    {
        anim.SetBool("isHurt", true);
    }
}

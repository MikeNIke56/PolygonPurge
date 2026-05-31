using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* base class for all entities
*/
public class EntityBaseClass : MonoBehaviour, IDamageable
{
    [Header("Base Variables")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float moveSpeed;

    protected float curHealth;
    protected Rigidbody2D rb;

    public Color baseColor;
    public Color damageColor;
    protected bool changeSpriteColor;

    protected void LookAt(Vector3 target, float offset)
    {
        //find angle between the player and target
        float lookAngle = AngleBetweenTwoPoints(transform.position, target) + offset;

        //apply target rotation on the z axis
        transform.eulerAngles = new Vector3(0, 0, lookAngle);
    }

    private float AngleBetweenTwoPoints(Vector3 point1,  Vector3 point2)
    {
        return Mathf.Atan2(point1.y - point2.y, point1.x - point2.x) * Mathf.Rad2Deg;
    }

    public virtual void TakeDamage(float damage)
    {

    }

    public virtual void Die()
    {

    }

    /**
    * blends between damage and base color of enemy sprite
    */
    protected void ResetSpriteColor()
    {
        Color spriteColor = GetComponent<SpriteRenderer>().color;

        spriteColor = Color.Lerp(spriteColor, baseColor,
            Time.deltaTime * 6.5f);

        GetComponent<SpriteRenderer>().color = spriteColor;
    }


    public float GetCurHealth()
    {
        return curHealth;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
    }
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
}

/**
 * the interface for entities to take damage
 */
public interface IDamageable
{
    void TakeDamage(float damage);
}

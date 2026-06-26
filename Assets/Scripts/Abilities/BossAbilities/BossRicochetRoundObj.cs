using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BossRicochetRoundObj: MonoBehaviour
{
    public float damage;
    public float speed;
    public float hitboxCooldown;

    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    private Rigidbody2D rb;
    private Collider2D col;
    private Camera cam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        HandleWallBounces();

        //keep the direction, force the speed
        rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        //if the collided object is on a layer we should interact with...
        if (LayerMaskChecker.i.IsInLayerMask(collider.gameObject, targetLayers))
        {
            GameObject entity = collider.gameObject;

            //and if it implements the IDamageable interface
            if (entity.TryGetComponent(out IDamageable myInterface))
            {
                EnemyBaseClass enemy = entity.GetComponent<EnemyBaseClass>();

                //cause damage
                enemy.TakeDamage(damage);
                StartCoroutine(StartHitboxCooldown());
            }
        }
    }

    /**
     * causes the round to "bounce" off the screen borders
     */
    private void HandleWallBounces()
    {
        //convert screen resolution to in-game world positions
        Vector2 screenMin = cam.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 screenMax = cam.ViewportToWorldPoint(new Vector2(1, 1));

        //get the halfsize of the object so we "collide" with the edge of the 
        //object
        float halfWidth = transform.localScale.x / 2;
        float halfHeight = transform.localScale.y / 2;

        Vector2 pos = transform.position;
        Vector2 velocity = rb.linearVelocity;

        //left wall
        if (pos.x - halfWidth <= screenMin.x)
        {
            pos.x = screenMin.x + halfWidth;

            //force rightward
            velocity.x = Mathf.Abs(velocity.x);
        }
        //right wall
        else if (pos.x + halfWidth >= screenMax.x)
        {
            pos.x = screenMax.x - halfWidth;

            //force leftward
            velocity.x = -Mathf.Abs(velocity.x);
        }

        //bottom wall
        if (pos.y - halfHeight <= screenMin.y)
        {
            pos.y = screenMin.y + halfHeight;

            //force upward
            velocity.y = Mathf.Abs(velocity.y);
        }
        //top wall
        else if (pos.y + halfHeight >= screenMax.y)
        {
            pos.y = screenMax.y - halfHeight;

            //force downward
            velocity.y = -Mathf.Abs(velocity.y);
        }

        rb.linearVelocity = velocity;
        transform.position = pos;
    }

    private IEnumerator StartHitboxCooldown()
    {
        col.enabled = false;
        yield return new WaitForSecondsRealtime(hitboxCooldown);
        col.enabled = true;
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }
}

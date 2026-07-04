using System.Collections;
using UnityEngine;

/**
 * the arc shockwave that boss emits with the "ArcShockwave" ability
 */
public class BossArcShockwaveObj : MonoBehaviour
{
    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    //how big the circle is
    public float radius = 1f;

    //how fast the ring expands out
    public float expansionSpeed;

    public float lifeTime;
    public float hitboxThickness;

    private float damage;

    //determines how smooth the circle is
    private int segments = 100;

    private LineRenderer lr;
    private CircleCollider2D circleCollider;

    private void OnEnable()
    {
        radius = 1f;

        if(lr)
        {
            circleCollider.radius = radius;
            DrawCircle(radius);
        }

        StartCoroutine(StartLifetimeCountdown());
    }

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();

        DrawCircle(radius);
    }

    void Update()
    {
        //continuously expand the ring
        radius += expansionSpeed * Time.deltaTime;
        DrawCircle(radius);
        circleCollider.radius = radius;
    }

    private void DrawCircle(float radius)
    {
        //sets up the line renderer to be in the shape of a circle
        lr.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0);

            lr.SetPosition(i, pos);
        }
    }

    protected virtual IEnumerator StartLifetimeCountdown()
    {
        yield return new WaitForSecondsRealtime(lifeTime);
        ObjectPoolingManager.ReturnObjectToPool(gameObject,
            ObjectPoolingManager.PoolType.Ability);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //if the collided object is on a layer we should interact with...
        if (LayerMaskChecker.i.IsInLayerMask(collider.gameObject, targetLayers))
        {
            GameObject entity = collider.gameObject;

            float distance = Vector2.Distance(entity.transform.position, 
                transform.position);

            //checks if the player is in contact with the edge of the ring
            if (Mathf.Abs(distance - radius) <= hitboxThickness * 0.5f)
            {
                //and if it implements the IDamageable interface
                if (entity.TryGetComponent(out IDamageable myInterface))
                {
                    EntityBaseClass enemy = entity.GetComponent<EntityBaseClass>();

                    //cause damage
                    enemy.TakeDamage(damage);
                }
            }
        }
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}

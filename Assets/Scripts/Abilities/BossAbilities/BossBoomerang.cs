using System.Collections;
using UnityEngine;

public class BossBoomerang: MonoBehaviour
{
    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    [Header("Boomerang Variables")]
    public float throwDistance = 10f;
    public float curveWidth = 5f;
    public float returnCurveWidth = -3f;
    public float duration = 1.5f;
    public float spinSpeed = 720f;

    public float damage;
    public float lifeTime;
    public bool isSpectreRound = false;

    private BossEnemy boss;
    private BossBoomerangBonanza parent;
    private Rigidbody2D rb;
    private Vector2 p0, p1, p2, p3;
    private float elapsed;
    private bool active;

    private void Awake()
    {
        boss = FindAnyObjectByType<BossEnemy>();
    }

    private void OnEnable()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();

        Launch(Random.insideUnitCircle.normalized);
        StartCoroutine(StartLifetimeCountdown());
    }

    private void Launch(Vector2 direction)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        Vector2 dir = direction.normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x);

        // start
        p0 = rb.position;  

        //outward curve
        p1 = rb.position + dir * throwDistance * 0.5f + perp * curveWidth; 

        //far point curves back
        p2 = rb.position + dir * throwDistance + perp * returnCurveWidth;

        //return to origin
        p3 = boss.transform.position;                                      

        elapsed = 0f;
        active = true;
    }

    void FixedUpdate()
    {
        if (!active) return;

        //constantly update the return position
        p3 = boss.transform.position;

        elapsed += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        if (t >= 1f)
        {
            active = false;
            rb.linearVelocity = Vector2.zero;
            rb.position = p3;

            ObjectPoolingManager.ReturnObjectToPool(gameObject,
                ObjectPoolingManager.PoolType.Bullet);

            if(isSpectreRound == false)
                parent.curBoomerangActive--;

            return;
        }

        //position on cubic bezier
        Vector2 pos = CubicBezier(p0, p1, p2, p3, t);

        //move via velocity for physics interactions
        Vector2 moveDir = (pos - rb.position);
        rb.linearVelocity = moveDir / Time.fixedDeltaTime;

        //rotate the boomerang
        rb.rotation += spinSpeed * Time.fixedDeltaTime;
    }

    /**
     * bezier curve for the boomerang to follow
     */
    Vector2 CubicBezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        float u = 1f - t;
        return u * u * u * a +
               3f * u * u * t * b +
               3f * u * t * t * c +
               t * t * t * d;
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        //if the collided object is on a layer we should interact with...
        if (LayerMaskChecker.i.IsInLayerMask(collision.gameObject, targetLayers))
        {
            GameObject entity = collision.gameObject;

            //and if it implements the IDamageable interface
            if (entity.TryGetComponent(out IDamageable myInterface))
            {
                //cause damage
                entity.GetComponent<EntityBaseClass>().TakeDamage(damage);
            }
        }
    }

    public void SetBoomerangParent(BossBoomerangBonanza parent)
    {
        this.parent = parent;
    }

    private IEnumerator StartLifetimeCountdown()
    {
        yield return new WaitForSeconds(lifeTime);
        ObjectPoolingManager.ReturnObjectToPool(gameObject,
            ObjectPoolingManager.PoolType.Bullet);
    }
}

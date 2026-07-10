using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossColonyBug: MonoBehaviour
{
    [Header("Bug Variables")]
    public float damage;
    public float speed;
    public float lifetime;
    public float lookOffset;
    public float damageTriggerRange;

    [Header("Bob Up/Down Variables")]
    private Vector3 tempRot;
    public float frequency;
    public float amplitude;

    public GameObject bugBody;

    private Quaternion lastFaceDirection;
    private PlayerController target;
    private BossColony parent;
    private Rigidbody2D rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        target = FindTarget();
        StartCoroutine(StartLifetimeCountdown());
    }

    private void Update()
    {
        if (!target || !target.gameObject.activeSelf)
            target = FindTarget();

        //when bug is within range of the target enemy
        if(Vector2.Distance(transform.position, 
            target.transform.position) <= damageTriggerRange)
        {
            target.TakeDamage(damage);
            ObjectPoolingManager.ReturnObjectToPool(gameObject,
            ObjectPoolingManager.PoolType.Bullet);
            parent.curBugsActive--;
        }
        RotateOnZ();
    }

    private void FixedUpdate()
    {
        if (target && target.gameObject.activeSelf)
        {
            LookAt(target.transform.position, lookOffset);
            rb.linearVelocity = -speed * Time.fixedDeltaTime * transform.right;
        }
    }

    private PlayerController FindTarget()
    {
        return PlayerController.i;
    }

    protected virtual IEnumerator StartLifetimeCountdown()
    {
        yield return new WaitForSeconds(lifetime);
        parent.curBugsActive--;
        ObjectPoolingManager.ReturnObjectToPool(gameObject,
            ObjectPoolingManager.PoolType.Bullet);
    }

    private void RotateOnZ()
    {
        //rotate back and forth with a Sin()
        tempRot = transform.eulerAngles;
        tempRot.z = 90f + Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * 
            amplitude;

        bugBody.transform.localEulerAngles = tempRot;
    }

    private void LookAt(Vector3 target, float offset)
    {
        //find angle between the player and target
        float lookAngle = AngleBetweenTwoPoints(transform.position, target) + 
            offset;

        //apply target rotation on the z axis
        transform.eulerAngles = new Vector3(0, 0, lookAngle);
    }

    private float AngleBetweenTwoPoints(Vector3 point1, Vector3 point2)
    {
        return Mathf.Atan2(point1.y - point2.y, point1.x - point2.x) * Mathf.Rad2Deg;
    }

    public void SetColonyParent(BossColony parent)
    {
        this.parent = parent;
    }
}

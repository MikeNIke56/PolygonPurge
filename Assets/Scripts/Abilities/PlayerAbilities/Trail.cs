using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Trail : MonoBehaviour
{
    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    private TrailRenderer trailRend;
    private EdgeCollider2D edgeCollider;
    private List<Vector2> points = new List<Vector2>();

    public float trailDamage;

    public void Setup(float startingTrailTime, float startingTrailDamage)
    {
        trailRend = GetComponent<TrailRenderer>();
        trailRend.time = startingTrailTime;

        edgeCollider = GetComponent<EdgeCollider2D>();

        trailDamage = startingTrailDamage;
    }

    private void FixedUpdate()
    {
        UpdateColliderPoints();
    }

    /**
     * updates the edge collider's vertices to match the trail's
     */
    private void UpdateColliderPoints()
    {
        //grab all of the trail's vertices
        Vector3[] trailPositions = new Vector3[trailRend.positionCount];
        trailRend.GetPositions(trailPositions);

        points.Clear();

        //convert 3D world positions to 2D local positions for the Edge Collider
        for (int i = 0; i < trailPositions.Length; i++)
            points.Add(transform.InverseTransformPoint(trailPositions[i]));

        //apply the updated vertices to the Edge Collider
        if (points.Count > 1)
            edgeCollider.points = points.ToArray();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        //if the collided object is on a layer we should interact with...
        if (LayerMaskChecker.i.IsInLayerMask(collider.gameObject, targetLayers))
        {
            GameObject entity = collider.gameObject;

            //and if it implements the IDamageable interface
            if (entity.TryGetComponent(out IDamageable myInterface))
            {
                EnemyBaseClass enemy = entity.GetComponent<EnemyBaseClass>();
                enemy.TakeDamage(Time.deltaTime * trailDamage);
            }
        }
    }
}

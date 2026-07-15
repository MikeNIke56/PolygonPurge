using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

/**
 * the chain lightning object that goes between enemies
 */
public class ChainLightning : MonoBehaviour
{
    [SerializeField] List<LineRenderer> lines = new List<LineRenderer>();
    public float lifeTime;

    private void OnEnable()
    {
        StartCoroutine(StartLifetimeCountdown());
    }

    private void Start()
    {
        StartCoroutine(StartLifetimeCountdown());
    }

    /**
     * sets the end points of the line renderer
     */
    public void SetPosition(Transform startPos, Transform endPos)
    {
        if(lines.Count > 0)
        {
            for(int i = 0; i < lines.Count; i++)
            {
                if (lines[i].positionCount >= 2)
                {
                    lines[i].SetPosition(0, startPos.position);
                    lines[i].SetPosition(1, endPos.position);
                }
            }
        }
    }

    public virtual IEnumerator StartLifetimeCountdown()
    {
        yield return new WaitForSeconds(lifeTime);
        ObjectPoolingManager.ReturnObjectToPool(gameObject,
            ObjectPoolingManager.PoolType.Ability);
    }
}

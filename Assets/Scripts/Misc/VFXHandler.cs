using System.Collections;
using UnityEngine;

/**
 * once a vfx object is created or re-enabled- wait a bit before returning to pool
 */
public class VFXHandler : MonoBehaviour
{
    private ParticleSystem partSystem;
    public float despawnBuffer;

    private void OnEnable()
    {
        partSystem = GetComponent<ParticleSystem>();
        StartCoroutine(StartLifetimeCountdown());
    }

    void Start()
    {
        partSystem = GetComponent<ParticleSystem>();
        StartCoroutine(StartLifetimeCountdown());
    }

    public IEnumerator StartLifetimeCountdown()
    {
        yield return new WaitForSeconds(partSystem.main.duration + despawnBuffer);

        ObjectPoolingManager.ReturnObjectToPool(gameObject,
            ObjectPoolingManager.PoolType.VFX);
    }
}

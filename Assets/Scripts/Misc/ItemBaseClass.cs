using System.Security.Cryptography;
using UnityEngine;

/**
* base class for all pickupable items
*/
public class ItemBaseClass : MonoBehaviour
{
    [Header("Item Base Variables")]
    private Vector3 startLocalPos;
    public float frequency;
    public float amplitude;
    [SerializeField] protected GameObject childObj;

    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    private void Start()
    {
        startLocalPos = childObj.transform.localPosition;
    }

    protected void BobUpDown()
    {
        //float up/down with a Sin()
        Vector3 pos = startLocalPos;
        pos.y += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;

        childObj.transform.localPosition = pos;
    }
}

using UnityEngine;

public class ArenaArea : MonoBehaviour
{
    private BoxCollider2D spawnArea;
    public static ArenaArea i { get; private set; }

    private void Awake()
    {
        if (i != null)
            Destroy(gameObject);
        else
            i = this;
    }

    private void Start()
    {
        spawnArea = GetComponent<BoxCollider2D>();
    }

    public BoxCollider2D GetSpawnArea()
    {
        return spawnArea;
    }
}

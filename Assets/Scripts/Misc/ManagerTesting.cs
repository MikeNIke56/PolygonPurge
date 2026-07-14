using System.Collections;
using UnityEngine;

public class ManagerTesting : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        yield return new WaitForSecondsRealtime(2);
        Debug.Log(Resources.FindObjectsOfTypeAll<GameManager>().Length);
        Debug.Log(Resources.FindObjectsOfTypeAll<ObjectPoolingManager>().Length);
        Debug.Log(Resources.FindObjectsOfTypeAll<EnemyManager>().Length);
        Debug.Log(Resources.FindObjectsOfTypeAll<PlayerController>().Length);
    }
}

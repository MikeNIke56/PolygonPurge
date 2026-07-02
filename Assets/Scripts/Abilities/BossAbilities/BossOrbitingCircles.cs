using System.Collections.Generic;
using UnityEngine;

public class BossOrbitingCircles: AbilityBaseClass
{
    public float orbitSpeed;
    public float orbitSpeedIncreaseAmnt;

    [SerializeField] GameObject circleObj;
    public List<BossOrbitCircleObj> circles;
    private int numOfCircles = 3;

    protected override void Update()
    {
        ContinuouslyRotate();
    }

    public override void SetUp()
    {
        base.SetUp();
        circles = new List<BossOrbitCircleObj>();
        SpawnCircles();
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        orbitSpeed *= orbitSpeedIncreaseAmnt;
        //numOfCircles++;
        //SpawnCircles();
    }

    private void SpawnCircles()
    {
        //spawns in circle object
        for (int i = 0; i < numOfCircles; i++)
        {
            GameObject circleObjCopy = Instantiate(circleObj, transform);
            circles.Add(circleObjCopy.GetComponent<BossOrbitCircleObj>());
        }

        //sets locations of circles
        circles[0].gameObject.transform.localPosition =
            new Vector3(0, 4f, 0);
        circles[1].gameObject.transform.localPosition =
            new Vector3(-3.3f, -2.2f, 0);
        circles[2].gameObject.transform.localPosition =
            new Vector3(3.3f, -2.2f, 0);
    }

    private void ContinuouslyRotate()
    {
        Vector3 newRotation = transform.eulerAngles;
        newRotation.z += Time.deltaTime * orbitSpeed;
        transform.eulerAngles = newRotation;
    }
}

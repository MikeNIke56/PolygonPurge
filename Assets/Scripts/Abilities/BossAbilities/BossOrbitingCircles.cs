using System.Collections.Generic;
using UnityEngine;

public class BossOrbitingCircles: AbilityBaseClass
{
    public float orbitSpeed;
    public float orbitSpeedIncreaseAmnt;

    [SerializeField] GameObject circleObj;
    public List<OrbitCircleObj> circles;
    private int numOfCircles = 1;

    protected override void Update()
    {
        ContinuouslyRotate();
    }

    public override void SetUp()
    {
        base.SetUp();
        circles = new List<OrbitCircleObj>();
        SpawnNewCircle();
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        orbitSpeed *= orbitSpeedIncreaseAmnt;
        numOfCircles++;
        SpawnNewCircle();
    }

    private void SpawnNewCircle()
    {
        //spawns in circle object
        GameObject circleObjCopy = Instantiate(circleObj, transform);

        circles.Add(circleObjCopy.GetComponent<OrbitCircleObj>());

        //adjusts locations of circles
        switch (numOfCircles)
        {
            case 1:
                circles[0].gameObject.transform.localPosition = 
                    new Vector3(2f, 0, 0);
                break;
            case 2:
                circles[0].gameObject.transform.localPosition =
                    new Vector3(2f, 0, 0);
                circles[1].gameObject.transform.localPosition =
                    new Vector3(-2f, 0, 0);
                break;
            case 3:
                circles[0].gameObject.transform.localPosition =
                    new Vector3(0, 2f, 0);
                circles[1].gameObject.transform.localPosition =
                    new Vector3(-1.75f, -1.2f, 0);
                circles[2].gameObject.transform.localPosition =
                    new Vector3(1.75f, -1.2f, 0);
                break;
            case 4:
                circles[0].gameObject.transform.localPosition =
                    new Vector3(2f, 0, 0);
                circles[1].gameObject.transform.localPosition =
                    new Vector3(-2f, 0, 0);
                circles[2].gameObject.transform.localPosition =
                    new Vector3(0, 2f, 0);
                circles[3].gameObject.transform.localPosition =
                    new Vector3(0, -2f, 0);
                break;
            case 5:
                circles[0].gameObject.transform.localPosition =
                    new Vector3(2f, .45f, 0);
                circles[1].gameObject.transform.localPosition =
                    new Vector3(-2f, .45f, 0);
                circles[2].gameObject.transform.localPosition =
                    new Vector3(0, 2f, 0);
                circles[3].gameObject.transform.localPosition =
                    new Vector3(1f, -1.75f, 0);
                circles[4].gameObject.transform.localPosition =
                    new Vector3(-1f, -1.75f, 0);
                break;
            default:
                Debug.Log("invalid amount of circles");
                break;
        }
    }

    private void ContinuouslyRotate()
    {
        Vector3 newRotation = transform.eulerAngles;
        newRotation.z += Time.deltaTime * orbitSpeed;
        transform.eulerAngles = newRotation;
    }
}

using Unity.VisualScripting;
using UnityEngine;

/**
* the weapon pivot point that the primary weapon rotates around
*/
public class WeaponPivotPoint : MonoBehaviour
{
    private float gunFlipSpeed = 12f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
    * makes the weapon pivot point rotate to look at the mouse's position
    */
    public void LookAtMouse(Vector3 target)
    {
        //find angle between the player and target
        float lookAngle = AngleBetweenTwoPoints(transform.position, target) + 180;

        //apply target rotation on the z axis
        transform.eulerAngles = new Vector3(0, 0, lookAngle);


        //flip the gun sprite when aiming left
        Vector3 gunScale = transform.localScale;

        //smoothly flip on y axis
        if (transform.eulerAngles.z < 90f || transform.eulerAngles.z > 270f)
            gunScale.y = Mathf.Lerp(gunScale.y, 1f, gunFlipSpeed * Time.deltaTime);
        else
            gunScale.y = Mathf.Lerp(gunScale.y, -1f, gunFlipSpeed * Time.deltaTime); 

        transform.localScale = gunScale;
    }

    /**
     * finds angle between 2 points in space and returns it
     */
    private float AngleBetweenTwoPoints(Vector3 point1, Vector3 point2)
    {
        return Mathf.Atan2(point1.y - point2.y, point1.x - point2.x) * Mathf.Rad2Deg;
    }
}

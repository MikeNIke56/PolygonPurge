using System.Collections;
using System.Collections.Generic;
using Terresquall;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/**
* player driver script
*/
public class PlayerController : EntityBaseClass
{
    public enum PlayerStates
    {
        Walking,
        Shooting,
        Death,
        Dodging
    }
    [SerializeField] protected List<PlayerStates> currentPlayerStates;


    [SerializeField] protected CircleCollider2D hitbox;
    [SerializeField] protected CircleCollider2D wallHitbox;

    //input Action Variables
    public InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction shootAction;
    private InputAction dodgeAction;
    private InputAction lookAction;

    //input vector
    private Vector2 moveVal;

    [Header("Dodge Variables")]
    public float dodgePower;
    public float dodgeDecaySpeed;

    private float curHealthRegenTime;
    [SerializeField] protected float maxHealthRegenTime;
    [SerializeField] protected float healthRegenSpeed;

    //player's current primary weapon
    private WeaponBaseClass primaryWeapon;
    public GameObject primaryWeaponGameObject;

    //primary weapon pivot
    public WeaponPivotPoint weaponPivotPoint;

    //ability pivot
    public GameObject abilityPivotPoint;

    //little buddy pivot
    public GameObject littleBuddyPivotPoint;

    //arc pylon pivot
    public GameObject arcPylonPivotPoint;

    //public GameObject w;

    private SpectreRounds spectreRounds;

    public static PlayerController i { get; private set; }


    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        if (i != null)
        {
            Destroy(gameObject);
        }
        else
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }

        moveAction = InputSystem.actions.FindAction("Move");
        shootAction = InputSystem.actions.FindAction("Shoot");
        dodgeAction = InputSystem.actions.FindAction("Dodge");
        lookAction = InputSystem.actions.FindAction("Look");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        curHealth = maxHealth;
        curHealthRegenTime = maxHealthRegenTime;
        GetComponent<SpriteRenderer>().color = baseColor;

        //load in and equip our starting weapon
        GameObject primaryWeaponGameObjectCopy = Instantiate<GameObject>(
            primaryWeaponGameObject, weaponPivotPoint.transform);

        primaryWeapon = primaryWeaponGameObjectCopy.GetComponent<WeaponBaseClass>();
    }

    // Update is called once per frame
    void Update()
    {
        moveVal = moveAction.ReadValue<Vector2>();

        if (moveVal.sqrMagnitude > 0 && !currentPlayerStates.Contains(PlayerStates.Walking))
            currentPlayerStates.Add(PlayerStates.Walking);
        else if(moveVal.sqrMagnitude < .001f && currentPlayerStates.Contains(PlayerStates.Walking))
            currentPlayerStates.Remove(PlayerStates.Walking);

        //auto fire if AR
        if (shootAction.IsPressed() && primaryWeapon && primaryWeapon is AR)
            Shoot();
        //tap fire if anything else
        else if (shootAction.WasPressedThisFrame() && primaryWeapon)
            Shoot();

        if (dodgeAction.WasPressedThisFrame() && !currentPlayerStates.Contains(PlayerStates.Dodging) &&
            moveVal.sqrMagnitude > 0)
            Dodge();

        Look();

        if(curHealth < maxHealth)
            HandleHealthRegen();

        if (changeSpriteColor == true)
        {
            ResetSpriteColor();
            if (GetComponent<SpriteRenderer>().color == baseColor)
                changeSpriteColor = false;
        }
    }

    void FixedUpdate()
    {
        //inputVelocity is our current move direction * our speed
        Vector2 inputVelocity = moveVal * moveSpeed;

        if (currentPlayerStates.Contains(PlayerStates.Dodging))
        {
            //we take the magnitude of our current velocity squared, not squaring is more expensive
            float sqrMagnitude = rb.linearVelocity.sqrMagnitude;

            //as long as the current velocity's magnitude is higher than movement's magnitude
            //and we are still moving ( > 1 )
            if (sqrMagnitude > inputVelocity.sqrMagnitude && sqrMagnitude > 1f)
            {
                //gradually slow down our velocity
                rb.linearVelocity -= dodgeDecaySpeed * Time.fixedDeltaTime * rb.linearVelocity.normalized;
                return;
            }
            currentPlayerStates.Remove(PlayerStates.Dodging);
            //hitbox.radius = baseHitboxSize;
            hitbox.enabled = true;
        }
        Move(inputVelocity);
    }

    public void Move(Vector2 inputVelocity)
    {
        rb.linearVelocity = inputVelocity;
    }

    public void Look()
    {
        if (GameManager.i.curState != GameManager.GameState.InGame) return;

        //get the current mouse position in screen pixels
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (weaponPivotPoint)
            weaponPivotPoint.LookAtMouse(mousePosition);
    }

    public void Dodge()
    {
        if (!currentPlayerStates.Contains(PlayerStates.Dodging))
        {
            currentPlayerStates.Add(PlayerStates.Dodging);
            //hitbox.radius = dodgeHitboxSize;
            hitbox.enabled = false;
            rb.linearVelocity = moveVal * dodgePower;
        }
    }

    public void Shoot()
    {
        //fire multiple times per press if Shotgun
        if (primaryWeapon is Shotgun && 
            primaryWeapon.canFire == true)
        {
            for (int i = 0; i < primaryWeapon.
                gameObject.GetComponent<Shotgun>().rounds; i++)
            {
                primaryWeapon.Fire();
            }
            StartCoroutine(StartFireCooldown());

            if (spectreRounds)
                StartCoroutine(ShootSpectreRounds(primaryWeapon));
        }
        else if(primaryWeapon is Shotgun == false &&
            primaryWeapon.canFire == true)
        {
            primaryWeapon.Fire();
            StartCoroutine(StartFireCooldown());

            if (spectreRounds)
                StartCoroutine(ShootSpectreRounds(primaryWeapon));
        }
    }

    /**
     * fires the current weapon again after a delay (tied to the spectre rounds
     * ability)
     */
    private IEnumerator ShootSpectreRounds(WeaponBaseClass weapon)
    {
        yield return new WaitForSecondsRealtime(spectreRounds.
            delayAfterFirstShot);

        switch (weapon)
        {
            case Pistol:
                if(spectreRounds.GetCurrentLevel() >= 1)
                    weapon.Fire();
                break;
            case Shotgun:
                if (spectreRounds.GetCurrentLevel() >= 2)
                    for (int i = 0; i < weapon.gameObject.
                        GetComponent<Shotgun>().rounds; i++)
                    {
                        weapon.Fire();
                    }
                break;
            case Sniper:
                if (spectreRounds.GetCurrentLevel() >= 3)
                    weapon.Fire();
                break;
            case AR:
                if (spectreRounds.GetCurrentLevel() >= 4)
                    weapon.Fire();
                break;
            case RPG:
                if (spectreRounds.GetCurrentLevel() >= 5)
                    weapon.Fire();
                break;
            default:
                Debug.Log("not valid weapon");
                break;
        }

    }

    private IEnumerator StartFireCooldown()
    {
        currentPlayerStates.Add(PlayerStates.Shooting);
        primaryWeapon.canFire = false;

        yield return new WaitForSecondsRealtime(primaryWeapon.fireCooldown);

        primaryWeapon.canFire = true;
        currentPlayerStates.Remove(PlayerStates.Shooting);
    }

    /**
     * will start health regening after we stop taking damage 
     * for a specified amount of time
     */
    private void HandleHealthRegen()
    {
        curHealthRegenTime -= Time.deltaTime;

        if(curHealthRegenTime <= 0)
        {
            curHealth += Time.deltaTime * healthRegenSpeed;

            if (curHealth >= maxHealth)
                curHealthRegenTime = maxHealthRegenTime;

            if (curHealthRegenTime <= -10)
                curHealthRegenTime = -.01f;
        }
    }

    public override void TakeDamage(float damage)
    {
        //float calculatedDamage = Mathf.Clamp(damage - defense, 1, damage);
        curHealth -= damage;
        curHealthRegenTime = maxHealthRegenTime;
        GetComponent<SpriteRenderer>().color = damageColor;
        changeSpriteColor = true;
        Debug.Log(curHealth);

        if (curHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        Debug.Log("player died");
    }

    public float GetHealthRegenSpeed()
    {
        return healthRegenSpeed;
    }
    public float GetMaxHealthRegenTime()
    {
        return maxHealthRegenTime;
    }
    public WeaponBaseClass GetPrimaryWeapon()
    {
        return primaryWeapon;
    }

    public void SetCurHealth(float health)
    {
        curHealth = health;
    }
    public void SetHealthRegenSpeed(float regenSpeed)
    {
        healthRegenSpeed = regenSpeed;
    }
    public void SetMaxHealthRegenTime(float regenTime)
    {
        maxHealthRegenTime = regenTime;
    }
    public void SetSpectreRounds(SpectreRounds spectreRounds)
    {
        this.spectreRounds = spectreRounds;
    }
    public void SetPrimaryWeapon(WeaponBaseClass weapon)
    {
        this.primaryWeapon = weapon;
    }
}

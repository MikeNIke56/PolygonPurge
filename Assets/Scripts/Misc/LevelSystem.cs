using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/**
 * handles the leveling of the player and when to initiate upgrades selection
 */
public class LevelSystem : MonoBehaviour
{
    [Header("XP Curve")]
    public float baseXP = 10f;
    public float exponent = 1.5f;
    public float growthMultiplier = 1.5f;

    [Header("State")]
    public int curPlayerLevel = 1;
    public int currentXP = 0;
    public int levelsGained = 0;

    public Slider expBar;

    private UnityEvent<int> onLevelUp;

    public int XPToNextLevel => GetXPRequired(curPlayerLevel);

    public static LevelSystem i { get; private set; }

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
    }

    private void Start()
    {
        if (onLevelUp == null)
            onLevelUp = new UnityEvent<int>();

        onLevelUp.AddListener(OnEventTriggeredLevelUp);
    }

    /**
     * adds exp to current player exp and levels up when necessary
     */
    public void AddXP(int amount)
    {
        currentXP += amount;
        while (currentXP >= XPToNextLevel)
        {
            currentXP -= XPToNextLevel;
            curPlayerLevel++;
            onLevelUp?.Invoke(curPlayerLevel);
        }

        expBar.value = GetFillAmount();

        if(levelsGained > 0)     
            StartCoroutine(GameManager.i.HandleUpgradesMenu());
    }

    private void OnEventTriggeredLevelUp(int level)
    {
        if (level < 10)
            growthMultiplier = 1.5f;
        else if (level < 20)
            growthMultiplier = 1.3f;
        else if (level < 30)
            growthMultiplier = 1.15f;

        levelsGained++;
    }

    public int GetXPRequired(int lvl)
    {
        return Mathf.RoundToInt(baseXP * Mathf.Pow(lvl, exponent) * 
            growthMultiplier);
    }

    public float GetFillAmount()
    {
        return (float)currentXP / XPToNextLevel;
    }
}

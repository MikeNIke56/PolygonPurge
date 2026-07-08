using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using static UpgradesManager;

/**
 * the main driver of the game
 */
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        None,
        MainMenu,
        InGame,
        UpgradesMenu,
        Pause
    }
    public GameState curState;
    public GameState prevState;

    public float multipleUpgradesDelayTime;
    public int curWave = 0;

    public TextMeshProUGUI waveText;

    public static GameManager i { get; private set; }

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

    /**
     * brings up the upgrades menu and pauses the active game
     */
    public IEnumerator HandleUpgradesMenu()
    {
        prevState = curState;
        curState = GameState.UpgradesMenu;

        int tempLevel = LevelSystem.i.levelsGained;
        while(tempLevel > 0)
        {
            UpgradesManager.i.InitializeUpgradesMenu();

            yield return new WaitUntil(() => tempLevel != 
                LevelSystem.i.levelsGained);
            tempLevel = LevelSystem.i.levelsGained;

            yield return new WaitForSecondsRealtime(multipleUpgradesDelayTime);
        }

        prevState = curState;
        curState = GameState.InGame;
        Time.timeScale = 1f;
        yield return null;
    }

    public void SetWaveNumber(int wave)
    {
        curWave = wave;
        waveText.text = wave.ToString();
    }
}

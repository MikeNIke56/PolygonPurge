using System;
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

    private void Update()
    {
        HandleStates();
    }

    /**
     * driver for the player when the game transitions from state to state
     */
    private void HandleStates()
    {
        switch (curState)
        {
            //applies the buffs to the player's current weapon
            case GameState.MainMenu:

                break;
            case GameState.InGame:

                break;
            case GameState.UpgradesMenu:

                break;
            case GameState.Pause:

                break;
            case GameState.None:

                break;
            default:
                Debug.Log("invalid game state");
                break;
        }
    }

    public void InitializeUpgradesMenu()
    {
        
    }
}

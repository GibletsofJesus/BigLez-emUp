using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour {

    public static GameStateManager instance;

    public enum GameState
    {
        Gameplay,
        Paused,
    }

    public GameState currentState, previousState;

    void Awake()
    {
        instance = this;
    }

    public void ChangeState(GameState newSate)
    {
        previousState = currentState;
        currentState = newSate;
    }
}

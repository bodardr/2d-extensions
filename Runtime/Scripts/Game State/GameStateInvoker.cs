using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInvoker : MonoBehaviour
{
    public void InvokeStart()
    {
        GameStateHandler.Invoke(GameState.START);
    }
    
    public void InvokeRestart()
    {
        GameStateHandler.Invoke(GameState.RESTART);
    }

    public void InvokeResume()
    {
        GameStateHandler.Invoke(GameState.RESUME);
    }

    public void InvokePause()
    {
        GameStateHandler.Invoke(GameState.PAUSE);
    }

    public void InvokeEnd()
    {
        GameStateHandler.Invoke(GameState.END);
    }

    public void InvokeGameOver()
    {
        GameStateHandler.Invoke(GameState.GAMEOVER);
    }
    
    public void Invoke(GameState state)
    {
        GameStateHandler.Invoke(state);
    }
}
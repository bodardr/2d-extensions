using System;
using UnityEngine;
using UnityEngine.Events;

public class GameStateListener : MonoBehaviour
{
    [Serializable]
    public class Entry
    {
        public GameState state;
        public UnityEvent unityEvent;
    }

    [SerializeField]
    private Entry[] entries;

    private void Awake()
    {
        foreach (var entry in entries)
        {
            switch (entry.state)
            {
                case GameState.START:
                    GameStateHandler.OnStart += entry.unityEvent.Invoke;
                    break;
                case GameState.RESTART:
                    GameStateHandler.OnRestart += entry.unityEvent.Invoke;
                    break;
                case GameState.PAUSE:
                    GameStateHandler.OnPause += entry.unityEvent.Invoke;
                    break;
                case GameState.RESUME:
                    GameStateHandler.OnResume += entry.unityEvent.Invoke;
                    break;
                case GameState.END:
                    GameStateHandler.OnEnd += entry.unityEvent.Invoke;
                    break;
                case GameState.GAMEOVER:
                    GameStateHandler.OnGameOver += entry.unityEvent.Invoke;
                    break;
            }
        }
    }
}
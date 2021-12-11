using System;

public class GameStateHandler
{
    public static event Action OnStart;
    public static event Action OnRestart;
    public static event Action OnPause;
    public static event Action OnResume;
    public static event Action OnEnd;
    public static event Action OnGameOver;

    public static void ClearListeners()
    {
        OnStart = OnRestart = OnPause = OnResume = OnEnd = OnGameOver = null;
    }
    
    public static void Invoke(GameState state)
    {
        switch (state)
        {
            case GameState.START:
                OnStart?.Invoke();
                break;
            case GameState.RESTART:
                OnRestart?.Invoke();
                break;
            case GameState.PAUSE:
                OnPause?.Invoke();
                break;
            case GameState.RESUME:
                OnResume?.Invoke();
                break;
            case GameState.END:
                OnEnd?.Invoke();
                break;
            case GameState.GAMEOVER:
                OnGameOver?.Invoke();
                break;
        }
    }
}
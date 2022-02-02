using UnityEditor;

public static class GameStateUtility
{
    [MenuItem("Game State/Invoke Start")]
    public static void InvokeStart()
    {
        GameStateHandler.Invoke(GameState.START);
    }

    [MenuItem("Game State/Invoke Restart")]
    public static void InvokeRestart()
    {
        GameStateHandler.Invoke(GameState.RESTART);
    }

    [MenuItem("Game State/Invoke Resume")]
    public static void InvokeResume()
    {
        GameStateHandler.Invoke(GameState.RESUME);
    }

    [MenuItem("Game State/Invoke Pause")]
    public static void InvokePause()
    {
        GameStateHandler.Invoke(GameState.PAUSE);
    }

    [MenuItem("Game State/Invoke End")]
    public static void InvokeEnd()
    {
        GameStateHandler.Invoke(GameState.END);
    }

    [MenuItem("Game State/Invoke Game Over")]
    public static void InvokeGameOver()
    {
        GameStateHandler.Invoke(GameState.GAMEOVER);
    }

    public static void Invoke(GameState state)
    {
        GameStateHandler.Invoke(state);
    }
}
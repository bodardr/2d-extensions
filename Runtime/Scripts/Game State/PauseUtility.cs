using UnityEngine;

public class PauseUtility : MonoBehaviour
{
    public void Pause() => PauseHandler.IsPaused = true;
    public void Unpause() => PauseHandler.IsPaused = false;
    public void TogglePause() => PauseHandler.TogglePause();
}
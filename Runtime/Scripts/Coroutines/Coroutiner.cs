using Bodardr.UI.Runtime;

/// <summary>
/// A static DontDestroyOnLoad instance allowing to call a Coroutine without fearing
/// GameObject deactivation.
/// </summary>
public class Coroutiner : DontDestroyOnLoad<Coroutiner>
{
}
#if HAS_CINEMACHINE
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Camera), typeof(CinemachineImpulseSource))]
public class ScreenShake : MonoSingleton<ScreenShake>
{
    private CinemachineImpulseSource impulseSource;

    protected override void Awake()
    {
        base.Awake();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    
    public static void Shake(float intensity = 1)
    {
        var instance = Instance;
        instance.impulseSource.GenerateImpulseAt(instance.transform.position, new Vector3(intensity, intensity, intensity));
    }
    
    public static void Shake(Vector3 intensity)
    {
        var instance = Instance;
        instance.impulseSource.GenerateImpulseAt(instance.transform.position, intensity);
    }
}
#endif

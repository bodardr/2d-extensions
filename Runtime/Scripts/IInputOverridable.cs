using UnityEngine;

public interface IInputOverridable
{
    void OnControlRegained();
    Vector2 OverrideInputUpdate(Vector2 inputVector);
}
using Assets.Scripts;
using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public abstract void ApplyEffect(PlayerControllers controller);

    public abstract void RemoveEffect(PlayerControllers stats);
}

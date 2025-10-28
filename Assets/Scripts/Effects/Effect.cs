using Assets.Scripts;
using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public abstract void ApplyEffect(PlayerController controller);

    public abstract void RemoveEffect(PlayerController stats);
}

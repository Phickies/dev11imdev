using UnityEngine;
using Assets.Scripts;

[CreateAssetMenu(menuName = "Card System/Effects/Dash")]
public class Dash : Effect
{
    public PlayerControllers controller;

    public override void ApplyEffect(PlayerControllers controller)
    {
        controller.Dash();
    }

    public override void RemoveEffect(PlayerControllers controller)
    {
        
    }
}

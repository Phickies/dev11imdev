using UnityEngine;
using Assets.Scripts;

[CreateAssetMenu(menuName = "Card System/Effects/Slam")]
public class Slam : Effect
{
    public PlayerControllers controller;

    public override void ApplyEffect(PlayerControllers controller)
    {
        controller.Slam();
    }

    public override void RemoveEffect(PlayerControllers controller)
    {
        
    }
}

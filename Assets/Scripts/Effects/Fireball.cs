using UnityEngine;
using Assets.Scripts;

[CreateAssetMenu(menuName = "Card System/Effects/Fireball")]
public class Fireball : Effect
{
    public PlayerControllers controller;

    public override void ApplyEffect(PlayerControllers controller)
    {
        controller.Shoot();
    }

    public override void RemoveEffect(PlayerControllers controller)
    {
        
    }
}

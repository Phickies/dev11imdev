using UnityEngine;
using Assets.Scripts;

[CreateAssetMenu(menuName = "Card System/Effects/Fireball")]
public class Fireball : Effect
{
    public PlayerController controller;

    public override void ApplyEffect(PlayerController controller)
    {
        controller.Shoot();
    }

    public override void RemoveEffect(PlayerController controller)
    {
        
    }
}

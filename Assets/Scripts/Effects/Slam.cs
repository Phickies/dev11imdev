using UnityEngine;
using Assets.Scripts;

[CreateAssetMenu(menuName = "Card System/Effects/Slam")]
public class Slam : Effect
{
    public PlayerController controller;

    public override void ApplyEffect(PlayerController controller)
    {
        controller.Slam();
    }

    public override void RemoveEffect(PlayerController controller)
    {
        
    }
}

using UnityEngine;
using Assets.Scripts;

[CreateAssetMenu(menuName = "Card System/Effects/Dash")]
public class Dash : Effect
{
    public PlayerController controller;

    public override void ApplyEffect(PlayerController controller)
    {
        // controller.Dash();
    }

    public override void RemoveEffect(PlayerController controller)
    {
        
    }
}

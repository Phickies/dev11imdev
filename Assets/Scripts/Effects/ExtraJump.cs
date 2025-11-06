using UnityEngine;
using Assets.Scripts;
using System.Runtime.Serialization;

[CreateAssetMenu(menuName = "Card System/Effects/Extra Jump")]
public class ExtraJump : Effect
{

    public override void ApplyEffect(PlayerControllers controller)
    {
        controller.ForceJump();
    }

    public override void RemoveEffect(PlayerControllers controller)
    {

    }
}

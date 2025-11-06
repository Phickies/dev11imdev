using UnityEngine;
using Assets.Scripts;

[CreateAssetMenu(menuName = "Card System/Effects/Speed Boost")]
public class DoubleSpeedEffect : Effect
{
    public float speedMultiplier = 2f;

    public override void ApplyEffect(PlayerControllers controller)
    {
        controller.walkSpeed *= speedMultiplier;
    }

    public override void RemoveEffect(PlayerControllers controller)
    {
        //TODO: a bit unsafe so ill have to check
        controller.walkSpeed /= speedMultiplier;
    }
}

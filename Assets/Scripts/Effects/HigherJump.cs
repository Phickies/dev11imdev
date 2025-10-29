using UnityEngine;
using Assets.Scripts;

[CreateAssetMenu(menuName = "Card System/Effects/Higher Jump")]
public class HigherJump : Effect
{
    public float jumpMultiplier = 2f;

    public override void ApplyEffect(PlayerController controller)
    {
        controller.jumpHeight *= jumpMultiplier;
    }

    public override void RemoveEffect(PlayerController controller)
    {
        //TODO: unsafe
        controller.jumpHeight /= jumpMultiplier;
    }
}

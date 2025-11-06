using UnityEngine;
using Assets.Scripts;
using System.Runtime.Serialization;

[CreateAssetMenu(menuName = "Card System/Effects/Float")]
public class Float : Effect
{
    public float floatGravity = 0f;

    public override void ApplyEffect(PlayerControllers controller)
    {
        GameManager manager = controller.gameManager;
        manager.UpdateGravity(floatGravity);
        controller.jumpHeight *= 1.8f;
    }

    public override void RemoveEffect(PlayerControllers controller)
    {
        GameManager manager = controller.gameManager;
        manager.GetComponent<GameManager>().ResetGravity();
        controller.jumpHeight /= 1.8f;
    }
}

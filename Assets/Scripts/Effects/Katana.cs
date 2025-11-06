using UnityEngine;
using Assets.Scripts;

[CreateAssetMenu(menuName = "Card System/Effects/Katana")]
public class Katana : Effect
{
    public override void ApplyEffect(PlayerControllers controller)
    {
        controller.UseKatana();
    }

    public override void RemoveEffect(PlayerControllers controller)
    {
        
    }
}

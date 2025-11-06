using UnityEngine;

[CreateAssetMenu(menuName = "Card System/Card")]
public class Card : ScriptableObject
{
    public int index;
    public string cardName;
    public string description;
    public float duration;
    public Effect effect;
}

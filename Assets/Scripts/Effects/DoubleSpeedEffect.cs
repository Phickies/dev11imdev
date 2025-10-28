using UnityEngine;
using Effect.cs;
using System.Diagnostics;

public class DoubleSpeedEffect : Effect
{
    public float speedMultiplier = 2f;
    
    void ApplyEffect()
    {
        Debug.log("triggered");
    }
  
}

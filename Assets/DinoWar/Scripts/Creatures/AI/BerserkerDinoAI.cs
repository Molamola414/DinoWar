using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Berserker behavior
// Become stronger when it's hp is low
public class BerserkerDinoAI : FigherDinoAI
{
    private bool isBerserkerModeOn = false;
    private float berserkerSpeedBoostRatio = 1.5f;

    public override void OnHit()
    {
        base.OnHit();
        
        if(!isBerserkerModeOn && (_creature.currentHp / _creature.hpMax) <= 0.5f) {
            isBerserkerModeOn = true;
            
            _creature.SetCrouching(true);
            _creature.speed *= berserkerSpeedBoostRatio;
        }
    }


}

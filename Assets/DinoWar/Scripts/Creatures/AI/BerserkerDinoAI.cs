using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Berserker behavior
// Become stronger when it's hp is low
public class BerserkerDinoAI : FigherDinoAI
{
    bool isBerserkerModeOn = false;

    public override void OnHit()
    {
        base.OnHit();

        Debug.Log("Berserker OnHit");
        
        if(!isBerserkerModeOn && (_creature.currentHp / _creature.hpMax) <= 0.5f) {
            isBerserkerModeOn = true;
            
            _creature.SetCrouching(true);
            _creature.speed *= 1.5f;
        }
    }


}

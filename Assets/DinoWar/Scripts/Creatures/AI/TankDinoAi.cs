using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDinoAi : FigherDinoAI
{
    public override void Start() {
        base.Start();

        _creature.animator.speed = 0.7f;
    }

    public float GetTankWalkingSpeed() {
        return _creature.speed * walkSpeedRatio;
    }

}

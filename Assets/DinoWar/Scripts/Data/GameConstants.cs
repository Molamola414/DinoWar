using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public enum EnemeyType
    {
        Melee = 0,
        ShortRanged,
        Ranged,
        Special,
        Support,
        Boss
    }

    public static int LayerEnvironment = LayerMask.NameToLayer("Environment");
    public static int LayerCreature = LayerMask.NameToLayer("Creatures");
    public static int LayerObstacle = LayerMask.NameToLayer("Obstacle");
    public static int LayerImpactField = LayerMask.NameToLayer("ImpactField");

}

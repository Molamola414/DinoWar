using System;
using Assets.MicroWar.Scripts.Enums;
using UnityEngine;

namespace Assets.MicroWar.Scripts.Data
{
    /// <summary>
    /// Common creature params
    /// </summary>
    [Serializable]
    public class CreatureParams
    {
        [Header("Common")]
        public float HpMax;
        public float Speed;
        public float Jump;
        public float Damage;
        public AttackType AttackType;
        public Throwing ShotPrefab;
        public Vector3 ShotSpot;
        
        [Header("Player")]
        public float MpMax;
        public float MpRestore;
        public float MpConsumption;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueWeapon : RangedWeapon
{

    public enum GlueSkill{
        Slow_Disbuff_Level_01 = 900310,
        Slow_Disbuff_Level_02 = 900311,

        Slow_Disbuff_Play_Igrne = 9003111,

        Slow_Disbuff_Level_03 = 900312,
        Slow_Disbuff_Level_04 = 900313,

        Effect_Duration_Increase_Level_01 = 900321,
        Effect_Duration_Increase_Level_02 = 900322,
        Effect_Duration_Increase_Level_03 = 900323,
        
        Effect_Size_Increase_Level_01 = 900331,
        Effect_Size_Increase_Level_02 = 900332,        
        Kill_Refresh_Effect = 900333,
        Effect_Buff = 90033,
        Effect_With_Damage = 900334,

        Path_Pop_Effect = 9003231,

        Trigger_Pop_Effect = 9003311,

    };

    
    public List<GlueSkill> gainSkills = new List<GlueSkill>();

    public override BulletShell createBullet(Vector3 attackDirection){
        BulletShell shot = base.createBullet(attackDirection);
        
        GlueBullet block = shot as GlueBullet;
        buffBulletWithSkillSet(block); 

        return shot;
    }

    private void buffBulletWithSkillSet(GlueBullet bullet ){
        bullet.resetBuffedValue(); 
        foreach(GlueSkill skillIdx in gainSkills){
            switch(skillIdx){
            case GlueSkill.Slow_Disbuff_Level_01   :
            break;

            case GlueSkill.Slow_Disbuff_Level_02   :
            break;

            case GlueSkill.Slow_Disbuff_Play_Igrne   :
            break;

            case GlueSkill.Slow_Disbuff_Level_03   :
            break;
            
            case GlueSkill.Slow_Disbuff_Level_04   :
            break;

            case GlueSkill.Effect_Duration_Increase_Level_01 :
            break;
            case GlueSkill.Effect_Duration_Increase_Level_02 :
            break;
            case GlueSkill.Effect_Duration_Increase_Level_03 :
            break;

            case GlueSkill.Effect_Size_Increase_Level_01 :
            break;
            case GlueSkill.Effect_Size_Increase_Level_02 :       
            break;
            
            case GlueSkill.Kill_Refresh_Effect:
            break;

            case GlueSkill.Effect_Buff :
            break;

            case GlueSkill.Effect_With_Damage :
            break;

            case GlueSkill.Path_Pop_Effect :
            break;

            case GlueSkill.Trigger_Pop_Effect :
            break;

            }
            
        }
    }

}

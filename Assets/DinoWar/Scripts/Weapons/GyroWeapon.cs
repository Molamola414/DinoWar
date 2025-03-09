using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroWeapon : RangedWeapon
 {



    public enum GyroSkill{
        Power_Up_Level_01 = 900210, 
        Power_Up_Level_02 = 900211, 
        Power_Up_Level_03 = 900212, 

        Increase_Attack_Times_Level_01 = 900220, 
        Increase_Attack_Times_Level_02 = 900221, 
        Increase_Attack_Times_Level_03 = 900222, 
        Increase_Attack_Times_Level_04 = 900223, 
        Increase_Attack_Range_Level_01 = 9002221, 
        Increase_Attack_Range_Level_02 = 9002222, 
        Push_Power_Up_Level_01 = 9002211, 
        Push_Power_Up_Level_02 = 9002212, 
        Kill_Pop_New_One = 9002213, 
    };

    public List<GyroSkill> gainSkills = new List<GyroSkill>();

    public override BulletShell createBullet(Vector3 attackDirection){
        BulletShell shot = base.createBullet(attackDirection);
        
        GyroBullet block = shot as GyroBullet;
        buffBulletWithSkillSet(block); 

        return shot;
    }


    private void buffBulletWithSkillSet(GyroBullet bullet ){
        bullet.resetBuffedValue(); 

        foreach(GyroSkill skillIdx in gainSkills){

            switch(skillIdx){
                case GyroSkill.Power_Up_Level_01 : 
                bullet.buff_damage += 3; 
                break;

                case GyroSkill.Power_Up_Level_02 : 
                bullet.buff_damage += 3; 
                break;

                case GyroSkill.Power_Up_Level_03 : 
                bullet.buff_damage += 3; 
                break;

                case GyroSkill.Increase_Attack_Times_Level_01 :
                bullet.buff_hitCountLimit += 1; 
                break;

                case GyroSkill.Increase_Attack_Times_Level_02 :
                bullet.buff_hitCountLimit += 1; 
                break; 

                case GyroSkill.Increase_Attack_Times_Level_03 :
                bullet.buff_hitCountLimit += 1; 
                break; 

                case GyroSkill.Increase_Attack_Times_Level_04 :
                bullet.buff_hitCountLimit += 1; 
                break; 

                case GyroSkill.Increase_Attack_Range_Level_01 : 
                bullet.buff_hitCountLimit += 1; 
                break; 

                case GyroSkill.Increase_Attack_Range_Level_02 : 
                bullet.buff_hitCountLimit += 1; 
                break; 

                case GyroSkill.Push_Power_Up_Level_01 : 
                case GyroSkill.Push_Power_Up_Level_02 : 

                case GyroSkill.Kill_Pop_New_One :
                break;
            }
        }        
    }


    public override void TriggerWeapon(Vector3 attackDirection, int eventIdx){
        if(bulletLeft == 0) {
            return;
        }

       // base.TriggerWeapon(attackDirection); 
    }    
}

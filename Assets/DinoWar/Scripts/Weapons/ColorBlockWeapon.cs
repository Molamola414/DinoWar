using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBlockWeapon : RangedWeapon
{
    public enum ColorBlockSkill{
        No_More_Random_Direction = 900110, 
        Reduce_Cold_Down_Level_01 = 900111, 
        Reduce_Bounce_Distance = 900112, 
        Increase_Mini_Block_Level_01 = 900120,
        Increase_Mini_Block_Level_02 = 900121,
        Increase_Mini_Block_Level_03 = 900122, 

        Mini_Block_Damage_Increase_Level_01  = 9001220, 
        Mini_Block_Damage_Increase_Level_02  = 9001221, 

        Kill_Pop_MiniBlock = 900130, 
        Kill_Case_Explosion = 900131, 
    };

    public List<ColorBlockSkill> gainSkills = new List<ColorBlockSkill>();

    public override BulletShell createBullet(Vector3 attackDirection){
        BulletShell shot = base.createBullet(attackDirection);
        
        BlockBullet block = shot as BlockBullet;
        buffBulletWithSkillSet(block); 

        return shot;
    }


    private void buffBulletWithSkillSet(BlockBullet bullet ){

        bullet.resetBuffedValue(); 

        GenMiniBlock genMiniBlockComponent = bullet.GetComponent<GenMiniBlock>(); 
        genMiniBlockComponent.buff_numOfMiniBlockGen = 0; 

        foreach(ColorBlockSkill skillIdx in gainSkills){
            switch(skillIdx){
                case ColorBlockSkill.No_More_Random_Direction:
                bullet.randomDirectionThreshold = 0; 
                break;

                case ColorBlockSkill.Reduce_Cold_Down_Level_01:
                break;

                case ColorBlockSkill.Reduce_Bounce_Distance:
                bullet.randomDirectionThreshold = 0;
                break;


                case ColorBlockSkill.Increase_Mini_Block_Level_01:
                genMiniBlockComponent.buff_numOfMiniBlockGen++; 
                break;

                case ColorBlockSkill.Increase_Mini_Block_Level_02:
                genMiniBlockComponent.buff_numOfMiniBlockGen++; 
                break;

                case ColorBlockSkill.Increase_Mini_Block_Level_03:
                genMiniBlockComponent.buff_numOfMiniBlockGen++; 
                break;

                case ColorBlockSkill.Mini_Block_Damage_Increase_Level_01  :
                genMiniBlockComponent.miniBlockDamage = (int)(bullet.getBulletDamage() * 0.2f);
                break;

                case ColorBlockSkill.Mini_Block_Damage_Increase_Level_02  :
                genMiniBlockComponent.miniBlockDamage = (int)(bullet.getBulletDamage() * 0.3f);
                break;

                case ColorBlockSkill.Kill_Pop_MiniBlock:
                bullet.isGenMiniBlockWhenKill = true;
                break;

                case ColorBlockSkill.Kill_Case_Explosion:
                bullet.isExplosionWhenKill = true;
                break;
            }
        }
    }

}

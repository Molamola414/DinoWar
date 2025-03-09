using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBlockBullet : BulletShell
{
    public Material[] materialList;
    public MeshRenderer cubeMeshRender;


    public override void Initialize(Vector3 d, int team) {
        this.team = team;

        cubeMeshRender.material = materialList[Random.Range(0, materialList.Length)];

        direction = new  Vector3(Random.value - 0.5f, 0, Random.value - 0.5f).normalized; 
        bulletRB.velocity = new Vector3(direction.x * speed, 40, direction.z * speed);
    }

    public override void Update()
    {
        base.Update();   
    }

}

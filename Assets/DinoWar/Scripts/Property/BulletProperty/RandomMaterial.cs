using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterial : MonoBehaviour
{
    public Material[] materialList;
    void OnEnable()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if(mr != null && materialList.Length > 0) {
            mr.material = materialList[Random.Range(0, materialList.Length)];
        }
    }
}

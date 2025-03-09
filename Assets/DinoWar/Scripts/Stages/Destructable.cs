using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple destructable script
// Can be applied to obstacle which is supposed to break by attack
public class Destructable : MonoBehaviour
{
    public float hpMax = 100;
    private float currentHp;
    Renderer rend;

    Color colorStart = Color.white;
    Color colorEnd = Color.red;

    public virtual void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        currentHp = hpMax;
        rend.material.color = colorStart;
    }

    public virtual void GetDamage(float damage)
    {
        // TO-FIX: Multiple damage may cause damage & die messing together
        currentHp -= damage;

        if (currentHp <= 0)
        {
            GameObject.Destroy(gameObject);
        }

        rend.material.color = Color.Lerp(colorStart, colorEnd, 1f - (currentHp / hpMax));
    }
}

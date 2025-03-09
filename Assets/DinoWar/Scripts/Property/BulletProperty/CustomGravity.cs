using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    Rigidbody _rigidbody;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        _rigidbody.AddForce(Physics.gravity * _rigidbody.mass * _rigidbody.mass);
    }
}

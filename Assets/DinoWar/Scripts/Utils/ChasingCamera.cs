using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingCamera : MonoBehaviour
{
    public enum ChasingMode {
        TopDown,
        ThirdPersonView
    }
    
    [HideInInspector]
    public ChasingMode mode = ChasingMode.TopDown;

    [SerializeField]
    private Transform m_followTargetTran = null;

    [SerializeField]
    private Vector3 m_topDownOffset = new Vector3(0, 60, -40);
    [HideInInspector]
    private Vector3 m_tpvOffset = new Vector3(0, 20, 30);
    private Vector3 m_currentOffset = new Vector3(0, 100, 100);
    private float smooth = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_followTargetTran != null)
        {
            switch(mode) {
                case ChasingMode.TopDown:
                {
                    m_currentOffset = Vector3.Lerp(m_currentOffset, m_topDownOffset, smooth * Time.deltaTime);

                    transform.position = Vector3.Lerp(transform.position, m_followTargetTran.transform.position + m_currentOffset, smooth * Time.deltaTime);
                }
                break;

                case ChasingMode.ThirdPersonView:
                {
                    m_currentOffset = Vector3.Lerp(m_currentOffset, m_tpvOffset, smooth * Time.deltaTime);

                    transform.position = Vector3.Lerp(transform.position, m_followTargetTran.transform.position - m_followTargetTran.transform.forward * m_tpvOffset.z + new Vector3(0, m_tpvOffset.y, 0), smooth * Time.deltaTime);
                }
                break;
            }
            transform.LookAt(m_followTargetTran);
        }
    }

    public void SetTopDownOffsetFormTarget(Vector3 offset)
    {
        m_topDownOffset = offset;
    }

    public void SetFollowingTarget (Transform transform)
    {
        m_followTargetTran = transform;
    }
}

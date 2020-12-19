using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private Transform _targetA, _targetB;

    private Transform _CurrentTarget;

    [SerializeField]
    private float _Speed = 1;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position == _targetA.position)
        {
            _CurrentTarget = _targetB;
        }
        if (transform.position == _targetB.position)
        {
            _CurrentTarget = _targetA;
        }
        transform.position = Vector3.MoveTowards(transform.position, _CurrentTarget.position, _Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.parent = this.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.parent = null;
        }
    }
}

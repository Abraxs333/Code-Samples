using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearthTrail : MonoBehaviour
{

    private Transform _CurrentTarget;

    [SerializeField]
    private float _Speed = 5;

    private UIManager _UIManager;
    // Start is called before the first frame update
    void Start()
    {
        _CurrentTarget = GameObject.FindGameObjectWithTag("Scoretext").GetComponent<Transform>();
        _UIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        //Get the location of the UI element you want the 3d onject to move towards


    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPoint = _CurrentTarget.transform.position + new Vector3(0, 0, 5);  //the "+ new Vector3(0,0,5)" ensures that the object is so close to the camera you dont see it

        //find out where this is in world space
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPoint);

        //move towards the world space position
        transform.position = Vector3.MoveTowards(transform.position, worldPos, _Speed * Time.deltaTime);

        if (transform.position == worldPos)
        {

            _UIManager.UpdateScore(10);
            Destroy(this.gameObject);
        }

        //transform.position = Vector3.MoveTowards(transform.position, _CurrentTarget.position, );
    }
}

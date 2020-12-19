using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private Player _Player;
    [SerializeField]
    private GameObject HerathTrail;
    

    private void Start()
    {
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
         
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _Player = other.GetComponent<Player>();
            if (_Player != null)
            {
                _Player.ObtainedCollectable(this.gameObject.tag);
            }
            Instantiate(HerathTrail,transform.position,Quaternion.identity,null);

            Destroy(this.gameObject);

        }
    }
}

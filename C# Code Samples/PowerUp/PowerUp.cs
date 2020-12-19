using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3;

    [SerializeField]
    private PowerUpID ID;

    enum PowerUpID
    {
        TripleShot,
        Speed,
        Shield,
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        ExitView();
    }

    void Move()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.tag;
        switch (tag)
        {
            case "Player":
                PlayerHit(other.gameObject);
                break;


            default:
                Debug.Log("Hit: " + tag);
                break;
        }

    }

    void PlayerHit(GameObject Player)
    {

        Player Myplayer = Player.GetComponent<Player>();

        if (Myplayer != null)
        {
            switch (ID)
            {

                case PowerUpID.TripleShot:
                    Myplayer.TripleShotObtained();
                    break;

                case PowerUpID.Speed:
                    Myplayer.SpeedObtained();
                    break;

                case PowerUpID.Shield:
                    Myplayer.ActivateShield();
                    break;



                default:
                    Debug.Log("Hit: " + tag);
                    break;

            }

          
            
        }

        Destroy(this.gameObject);
    }

    void ExitView()
    {

        
        if (transform.position.y <= -6.5)
        {
            Destroy(this.gameObject);
        }
    }

}

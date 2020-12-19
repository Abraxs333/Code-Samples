using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{

    bool finishloop = false;
    [SerializeField]
    GameObject Enemy;
    [SerializeField]
    private float _spawnRate = 3;
    [SerializeField]
    private float _spawnRatemin = 5;
    [SerializeField]
    private float _spawnRatemax = 10;

    // Start is called before the first frame update
    void Start()
    {
        StartSpawning();
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemy());

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            finishloop = !finishloop;
            StartCoroutine(SpawnEnemy());

        }
    }
    IEnumerator SpawnEnemy()
    {

        while (finishloop == false)
        {
            Vector3 offset = new Vector3(0, Random.Range(0, 3.0f), 0);
            _spawnRate = Random.Range(_spawnRatemin, _spawnRatemax);

            Instantiate(Enemy,transform.position,Quaternion.identity,null);

            yield return new WaitForSeconds(_spawnRate);
        }

    }

}
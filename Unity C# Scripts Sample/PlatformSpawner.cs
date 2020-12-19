using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{

    private bool Emptyspace=false;
    private bool previousemptystate=false;

    bool finishloop = false;
    [SerializeField]
    GameObject[] Platform;
    [SerializeField]
    GameObject Skeleton;
    GameObject currenskeleton;

    [SerializeField]
    GameObject PlatformParents;

  

    [SerializeField]
    private float[] ProbPlatfLvl = { 100f, 65f, 35f, 15f };

    [SerializeField]
    private float[] Prob3Hearths = { 5f, 35f, 65f, 90f };
    [SerializeField]
    private float[] Prob2Hearths = { 20f, 45f, 30f, 7f };
    [SerializeField]
    private float[] Prob1Hearths = { 65f, 15f, 4f, 2f };
    [SerializeField]
    private float[] Prob0Hearths = { 20f, 5f, 1f, 1f };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        Emptyspace = true;
    }

    private void OnTriggerStay(Collider other)
    {
        Emptyspace = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Emptyspace && previousemptystate != Emptyspace)
        {
            OnEmptySpace();
        }
    }

    private void OnEmptySpace()
    {
        Vector3 offset = new Vector3(transform.position.x+ 1.7f, 0, transform.position.z);


        Debug.Log("Started empty space");
        currenskeleton = Instantiate(Skeleton, offset, Quaternion.identity, PlatformParents.transform);
        populateskeleton(currenskeleton);   


    }
    private float chance = 100;
    private int platftype = 0;
    private void populateskeleton(GameObject _Placeholder)
    {
        int i = 0;
        foreach(Transform child in _Placeholder.transform)
        {
            Debug.Log("Empezando el floor: " + i);
            foreach (Transform grandchild in child)
            {
                Debug.Log("Usando el placeholder: " + grandchild.name);
                if (grandchild.tag == "PlatformShell")
                {

                    chance = Random.Range(0f, 100f);
                    Debug.Log("Chance: " + chance);
                    if (chance < ProbPlatfLvl[i])
                    {

                        if (chance < Prob3Hearths[i])
                        {
                            platftype = 3;
                        }
                        else if (chance < Prob2Hearths[i]+ Prob3Hearths[i])
                        {
                            platftype = 2;
                        }
                        else if (chance < Prob1Hearths[i]+ Prob2Hearths[i] + Prob3Hearths[i])
                        {
                            platftype = 1;
                        }
                        else
                        {
                            platftype = 0;
                        }

                        Instantiate(Platform[platftype], grandchild.position, Quaternion.identity, grandchild);
                    }


                }
            }
            Debug.Log("Terminaste el floor: "+i);
            i++;
        }
    }

    private void LateUpdate()
    {
        previousemptystate = Emptyspace;
    }
}

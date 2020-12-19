using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private float Idlestart = 0;
    private float timeIdle = 0;

    [SerializeField]
    private Animator Model;

    // Start is called before the first frame update
    void Start()
    {
        Idlestart = Time.timeSinceLevelLoad;
    }

    // Update is called once per frame
    void Update()
    {
        timeIdle = Time.timeSinceLevelLoad - Idlestart;

        if(Model.GetCurrentAnimatorStateInfo(0).IsName("Idle") && timeIdle > 10.5f)
        {
            if (Random.Range(0, 100) > 50)
            {
                Model.SetTrigger("A");
            }
            else
            {
                Model.SetTrigger("B");
            }
            Idlestart = Time.timeSinceLevelLoad;
        }else if (!Model.GetCurrentAnimatorStateInfo(0).IsName("Idle") && timeIdle > 12.5f)
        {
            Model.SetTrigger("C");
            Idlestart = Time.timeSinceLevelLoad;
        }

    }


    public void StartLvl()
    {
        SceneManager.LoadScene(1);
    }
}

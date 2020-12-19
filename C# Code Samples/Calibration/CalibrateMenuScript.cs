using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TechTweaking.Bluetooth;
#if !UNITY_ANDROID  || UNITY_EDITOR
using System.IO.Ports;

#endif
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(VR_Footwear))]

public class CalibrateMenuScript : MonoBehaviour
{
    public Image calibrating_status_foreground;
    public GameObject calibrating_status_background;
    public Image Pose_IMG;
    public GameObject calibrating_text;
    public GameObject Player;
    public GameObject Return_BTN;
    public GameObject Calibrate_BTN;
    //int Pose = 0;
    int Pose = 1;
    public Sprite Pose_Sprite0;
    public Sprite Pose_Sprite1;
    public Sprite Pose_Sprite2;

    public GameObject Calibrate_Menu;
    public GameObject Pause_Menu;

    Player player;
    VR_Footwear VR_Footwear;

    public int min = 10;
    public int max = 90;




    bool calibratin_pressed = false;
    // Use this for initialization
    void Start()
    {

        VR_Footwear = GameObject.Find("Player").GetComponent<VR_Footwear>();
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void calibrating()
    {
        calibratin_pressed = true;
        Return_BTN.SetActive(false);
        Calibrate_BTN.SetActive(false);
        calibrating_status_background.SetActive(true);
        calibrating_status_foreground.gameObject.SetActive(true);
        calibrating_text.SetActive(true);

        StartCoroutine(calibrate(Pose));


    }

    void endcalibration()
    {
        Pose++;
        Calibrate_BTN.SetActive(true);
        calibrating_status_background.SetActive(false);
        calibrating_status_foreground.gameObject.SetActive(false);
        calibrating_text.SetActive(false);
        Debug.Log("ACTIVE False");


        if (Pose > 2)
        { Pose = 1; }

        switch (Pose)
        {
            case 0:
                Pose_IMG.sprite = Pose_Sprite0;
                
                break;
            case 1:
                Pose_IMG.sprite = Pose_Sprite1;
                Return_BTN.SetActive(true);
                Calibrate_Menu.SetActive(false);
                Pause_Menu.SetActive(true);
                break;
            case 2:
                Pose_IMG.sprite = Pose_Sprite2;
                break;
            default:
                Pose_IMG.sprite = Pose_Sprite0;
                Pose_IMG.sprite = Pose_Sprite1;
                break;
        }
    }


    static double Percentile(double[] sortedData, double p)
    {
        
        if (p >= 100.0d) return sortedData[sortedData.Length - 1];

        double position = (sortedData.Length + 1) * p / 100.0;
        double leftNumber = 0.0d, rightNumber = 0.0d;

        double n = p / 100.0d * (sortedData.Length - 1) + 1.0d;

        if (position >= 1)
        {
            leftNumber = sortedData[(int)Math.Floor(n) - 1];
            rightNumber = sortedData[(int)Math.Floor(n)];
        }
        else
        {
            leftNumber = sortedData[0]; // first data
            rightNumber = sortedData[1]; // first data
        }

        
        if (Equals(leftNumber, rightNumber))
            return leftNumber;
        double part = n - Math.Floor(n);
        return leftNumber + part * (rightNumber - leftNumber);
    } // end of internal function percentile

    public IEnumerator calibrate(int iterator)
    {

        
        Debug.Log("iniciando loop");
        double[] Values_Left = new double[50];
        double[] Values_Right = new double[Values_Left.Length];

        for (int i = 0; i < Values_Left.Length; i++)
        {
#if !UNITY_ANDROID  || UNITY_EDITOR
            calibrating_status_foreground.fillAmount = Convert.ToSingle( ExtensionMethods.Map(i, 0, Values_Left.Length, 0, 1));
            VR_Footwear.Left_Foot_port.WriteLine("S");
            VR_Footwear.Right_Foot_port.WriteLine("S");
            yield return new WaitForSecondsRealtime(0.1f);
            double.TryParse(VR_Footwear.Left_Foot_port.ReadLine(), out Values_Left[i]);
            double.TryParse(VR_Footwear.Right_Foot_port.ReadLine(), out Values_Right[i]);
            calibrating_status_foreground.fillAmount = Convert.ToSingle( ExtensionMethods.Map(i, 0, Values_Left.Length, 0, 1));
#endif

        }

#if UNITY_ANDROID  && !UNITY_EDITOR
        yield return new WaitForSecondsRealtime(0.1f);
#endif

        Array.Sort(Values_Left);
        Array.Sort(Values_Right);



        switch (iterator)
        {
            case 0:
                VR_Footwear.Left_Stationary_Lower_limit = (int)Percentile(Values_Left, min);
                VR_Footwear.Right_Stationary_Lower_limit = (int)Percentile(Values_Right, min);
                VR_Footwear.Left_Stationary_Upper_limit = (int)Percentile(Values_Left, max);
                VR_Footwear.Right_Stationary_Upper_limit = (int)Percentile(Values_Right, max);
                break;
            case 1:
                VR_Footwear.Left_DOWN_Lower_limit = (int)Percentile(Values_Left, min);
                VR_Footwear.Right_UP_Lower_limit = (int)Percentile(Values_Right, min);
                VR_Footwear.Left_DOWN_Upper_limit = (int)Percentile(Values_Left, max);
                VR_Footwear.Right_UP_Upper_limit = (int)Percentile(Values_Right, max);
                break;
            case 2:
                VR_Footwear.Left_UP_Lower_limit = (int)Percentile(Values_Left, min);
                VR_Footwear.Right_DOWN_Lower_limit = (int)Percentile(Values_Right, min);
                VR_Footwear.Left_UP_Upper_limit = (int)Percentile(Values_Left, max);
                VR_Footwear.Right_DOWN_Upper_limit = (int)Percentile(Values_Right, max);
                break;

            default:
                break;

        }


        endcalibration();
    }

}

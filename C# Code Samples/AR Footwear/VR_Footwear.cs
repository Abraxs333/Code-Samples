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

public class VR_Footwear : MonoBehaviour
{
    public Text Speed_Text;
    public Text Left_Foot_Raw_Text;
    public Text Right_Foot_Raw_Text;
    public Text Left_Foot_Converted_Text;
    public Text Right_Foot_Converted_Text;
    public Text Left_Foot_Translated_Text;
    public Text Right_Foot_Translated_Text;

    public bool direccion = true;

    int Left_Foot_Raw;
    int Right_Foot_Raw;
    int Left_Foot_Converted;
    int Right_Foot_Converted;
    string Left_Foot_Translated;
    string Right_Foot_Translated;

    float target_velocity = 0f;

    Player player;
    Scroll scroll;
    //BarraVelocidad barraVelocidad;


#if !UNITY_ANDROID || UNITY_EDITOR
    public SerialPort Left_Foot_port;
    public SerialPort Right_Foot_port;
#endif


#if UNITY_ANDROID && !UNITY_EDITOR
    private BluetoothDevice Left_Foot_port;
    private BluetoothDevice Right_Foot_port;
#endif

    public int Left_DOWN_Lower_limit = 50;
    public int Left_DOWN_Upper_limit = 70;
    public int Left_UP_Lower_limit = 25;
    public int Left_UP_Upper_limit = 47;
    public int Left_Stationary_Lower_limit = 48;
    public int Left_Stationary_Upper_limit = 49;

    public int Right_DOWN_Lower_limit = 150;
    public int Right_DOWN_Upper_limit = 200;
    public int Right_UP_Lower_limit = 15;
    public int Right_UP_Upper_limit = 40;
    public int Right_Stationary_Lower_limit = 41;
    public int Right_Stationary_Upper_limit = 149;

    bool Left_alternancia = false;


    // Use this for initialization
    void Start()
    {
        player = GetComponent<Player>();
        //scroll= GameObject.Find("Background").GetComponent<Scroll>();
        //barraVelocidad = GetComponent<BarraVelocidad>();
        Left_Foot_Raw = 0;
        Right_Foot_Raw = 0;
        Left_Foot_Converted = 0;
        Right_Foot_Converted = 0;
        Left_Foot_Translated = "DOWN";
        Right_Foot_Translated = "UP";


        Left_DOWN_Lower_limit = 50;
        Left_DOWN_Upper_limit = 70;
        Left_UP_Lower_limit = 25;
        Left_UP_Upper_limit = 47;
        Left_Stationary_Lower_limit = 48;
        Left_Stationary_Upper_limit = 49;

        Right_DOWN_Lower_limit = 150;
        Right_DOWN_Upper_limit = 200;
        Right_UP_Lower_limit = 15;
        Right_UP_Upper_limit = 40;
        Right_Stationary_Lower_limit = 41;
        Right_Stationary_Upper_limit = 149;

        if (player.tipo_de_control == 8)
        { open_ports(); }
        Start_Time = Time.time - .1f;

    }

    // Update is called once per frame
    void Update()
    {
        if (player.tipo_de_control == 8)
        {
            
            Obtain_Both_Foot();


            //player.runVelocitySpeed += .1f;

            //scroll.speed = player.velocity.x;

            Update_speed();
            Update_DebugScreen();
        }


    }

    float Start_Time;
    float End_Time;
    float Now_time;

    int RFC_Previous_value;
    int LFC_Previous_value;
    enum last_step_options { Left, Right }
    int last_step = 5;
    enum Step_State_options { Left_UP, Both_DOWN, Right_UP }
    int Last_Step_State;
    int Step_State;
    //enum Right_Options { Left_UP, Both_DOWN, Right_UP, Both_UP }
    int Left_Interpretation;
    int Right_Interpretation;
    enum Agreement_Options { Agree, Disagree }
    int Agreement;
    int Last_Agreement;
    bool Is_Stationary;

    public float step_lenght = 1f;

    float calculate_target_velocity()
    {
        if (player.tipo_de_control == 8)
        {
            interprete_Left();
            interprete_Right();
            Check_State();




        }
        else
        {
            target_velocity = UnityEngine.Random.Range(0.0f, 30.0f);
        }

        return (target_velocity);
    }

    public IEnumerator Desaceleration()
    {
        yield return null;
    }

    float Calculate_Step_Velocity()
    {
        float speed = 0;
        if (Step_State != Last_Step_State)
        {
            if (Step_State != (int)Step_State_options.Left_UP && Last_Step_State == (int)Step_State_options.Left_UP)
            {
                if (last_step != (int)last_step_options.Left)
                {
                    last_step = (int)last_step_options.Left;
                    End_Time = Time.time;
                    Last_Step_State = Step_State;
                    if (End_Time - Start_Time > .15)
                    { speed = (step_lenght / (End_Time - Start_Time)); }
                    else
                    { speed = target_velocity; }



                    Start_Time = Time.time;
                    return (speed);
                }
                else
                {
                    Last_Step_State = Step_State;
                    return target_velocity;
                }
            }
            else if (Step_State != (int)Step_State_options.Right_UP && Last_Step_State == (int)Step_State_options.Right_UP)
            {
                if (last_step != (int)last_step_options.Right)
                {
                    last_step = (int)last_step_options.Right;
                    End_Time = Time.time;
                    Last_Step_State = Step_State;
                    if (End_Time - Start_Time > .15)
                    { speed = (step_lenght / (End_Time - Start_Time)); }
                    else
                    { speed = target_velocity; }
                    Start_Time = Time.time;
                    return (speed);

                }
                else
                {
                    Last_Step_State = Step_State;
                    return target_velocity;
                }
            }
            else
            {
                Last_Step_State = Step_State;
                return target_velocity;
            }
        }
        else
        {
            Last_Step_State = Step_State;
            return target_velocity;
        }

        Last_Step_State = Step_State;
        return target_velocity;
    }

    int time_until_stop = 3;

    void Check_State()
    {
        Agreement = (Left_Interpretation == Right_Interpretation) ? (int)Agreement_Options.Agree : (int)Agreement_Options.Disagree;




        if (Agreement != Last_Agreement && Agreement == (int)Agreement_Options.Agree)
        {
            Step_State = Right_Interpretation;


            target_velocity = Calculate_Step_Velocity();

            //target_velocity = 1;

        }
        if ((Left_Foot_Translated == Right_Foot_Translated) && (Left_Foot_Translated == "UP"))
        {

            float valoranalogicoactual = (float)ExtensionMethods.Map(target_velocity, 0, 4, 0, 1);
            float angulo = (float)ExtensionMethods.Map(target_velocity, 0, 4, 16, 160);
            player.valor_RT = valoranalogicoactual;
            player.valor_angulo = angulo;
            player.OnJumpInputDown(valoranalogicoactual);
        }
        if (Agreement == Last_Agreement && Agreement == (int)Agreement_Options.Disagree && Right_Interpretation == (int)Step_State_options.Left_UP && Left_Interpretation == (int)Step_State_options.Right_UP)
        {



            End_Time = Time.time;
            if ((End_Time - Start_Time) > time_until_stop)
            {
                target_velocity = 0;
            }
        }
        Last_Agreement = Agreement;
    }

    void interprete_Left()
    {
        if (LFC_Previous_value != Left_Foot_Converted)
        {

            Left_Interpretation = (Left_Foot_Converted == 1 && LFC_Previous_value != 1) ? (int)Step_State_options.Left_UP : (int)Step_State_options.Right_UP;
        }
        else
        {

        }

        LFC_Previous_value = Left_Foot_Converted;

    }

    void interprete_Right()
    {
        if (RFC_Previous_value != Right_Foot_Converted)
        {
            
            Right_Interpretation = (Right_Foot_Converted == 1 && RFC_Previous_value != 1) ? (int)Step_State_options.Right_UP : (int)Step_State_options.Left_UP;
        }
        else
        {

        }

        RFC_Previous_value = Right_Foot_Converted;

    }

    void Update_speed()
    {
        if (direccion)
        { player.runVelocitySpeed = 1; }
        else { player.runVelocitySpeed = -1; }

        player.moveSpeed = calculate_target_velocity();

    }

    void Update_DebugScreen()
    {
        Speed_Text.text = player.velocity.x.ToString("n2") + " m/s";
        Left_Foot_Raw_Text.text = Left_Foot_Raw.ToString();
        Right_Foot_Raw_Text.text = Right_Foot_Raw.ToString();
        Left_Foot_Converted_Text.text = Left_Foot_Converted.ToString();
        Right_Foot_Converted_Text.text = Right_Foot_Converted.ToString();
        Left_Foot_Translated_Text.text = Left_Foot_Translated;
        Right_Foot_Translated_Text.text = Right_Foot_Translated;

    }

    void open_ports()
    {

#if !UNITY_ANDROID || UNITY_EDITOR
        if (player.tipo_de_control == 8)
        {
            Left_Foot_port = new SerialPort(@"\\.\" + "COM16", 9600);
            Left_Foot_port.Open();

            Right_Foot_port = new SerialPort(@"\\.\" + "COM15", 9600);
            Right_Foot_port.Open();

        }
#endif

    }
  
    public void disconnect()
    {

#if !UNITY_ANDROID  || UNITY_EDITOR
        Left_Foot_port.Close();
        Right_Foot_port.Close();
#endif

#if UNITY_ANDROID  && !UNITY_EDITOR
        //device.close();
  
#endif
    }

    private void OnDestroy()
    {
        try
        {
            disconnect();
        }
        catch (Exception e)
        {
            Debug.Log("NO hay dispositivo para desconectarse");
        }
    }

    // BUSCAR COMO HACERLO MAS PEQUENO PARA NO USAR DOS FUNCIONES DIFERENTES

    string LF_arduino_string;

    void Obtain_Left_Foot()
    {
        StartCoroutine
  (
      AsynchronousReadFrom_Left_Foot

      ((string L) => pasarLF(L),     // Callback
          () => Debug.Log("Error!"), // Error callback
           10f                             // Timeout (seconds)
       )
   );
        Left_Foot_Raw = int.Parse(LF_arduino_string);
        //Left_alternancia = false;
        Convert_Left_Foot();


    }

    void Convert_Left_Foot()
    {

        
        if (Left_Foot_Raw <= (Left_DOWN_Upper_limit + mensodelrango) && Left_Foot_Raw > (Left_UP_Upper_limit + mensodelrango))
        {
            Left_Foot_Converted = -1;
            Left_Foot_Translated = "DOWN";
        }
        
        else if (Left_Foot_Raw >= (Left_UP_Lower_limit - mensodelrango) && Left_Foot_Raw < (Left_DOWN_Lower_limit - mensodelrango))
        {
            Left_Foot_Converted = 1;
            Left_Foot_Translated = "UP";
        }
       
        else
        {
        }

       
    }

    public IEnumerator AsynchronousReadFrom_Left_Foot(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);


#if !UNITY_ANDROID || UNITY_EDITOR
        do
        {
            Left_Foot_port.WriteLine("S");
            try
            {

                LF_arduino_string = Left_Foot_port.ReadLine();
      
            }
            catch (TimeoutException)
            {
                LF_arduino_string = "1234";
            }

            if (LF_arduino_string != null)
            {
                pasarLF(LF_arduino_string);
                callback(LF_arduino_string);

                yield return null;
            }
            else
                yield return new WaitForSeconds(.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

#endif


        if (fail != null)
            fail();
        yield return null;
    }

    void pasarLF(string L)
    {
        LF_arduino_string = L;
        Debug.Log(L);

    }













    // FUNCION PARA OBTENER LO DEL DERECHO

    string RF_arduino_string;

    void Obtain_Right_Foot()
    {
        StartCoroutine
  (
      AsynchronousReadFrom_Right_Foot

      ((string R) => pasarRF(R),     // Callback
          () => Debug.Log("Error!"), // Error callback
           10f                             // Timeout (seconds)
       )
   );
        Right_Foot_Raw = int.Parse(RF_arduino_string);
        //Left_alternancia = true;
        Convert_Right_Foot();
    }

    public int mensodelrango = 5;
    void Convert_Right_Foot()
    {
       
        if (Right_Foot_Raw <= (Right_DOWN_Upper_limit + mensodelrango) && Right_Foot_Raw > (Right_UP_Upper_limit + mensodelrango))
        {
            Right_Foot_Converted = -1;
            Right_Foot_Translated = "DOWN";
        }
       

        else if (Right_Foot_Raw >= (Right_UP_Lower_limit - mensodelrango) && Right_Foot_Raw < (Right_DOWN_Lower_limit - mensodelrango))
        {
            Right_Foot_Converted = 1;
            Right_Foot_Translated = "UP";
        }
       
        else
        {
        }

    }

    public IEnumerator AsynchronousReadFrom_Right_Foot(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

#if !UNITY_ANDROID || UNITY_EDITOR

        do
        {
            Right_Foot_port.WriteLine("S");
            try
            {
                RF_arduino_string = Right_Foot_port.ReadLine();
            }
            catch (TimeoutException)
            {
                RF_arduino_string = "1234";
            }

            if (RF_arduino_string != null)
            {
                pasarRF(RF_arduino_string);
                callback(RF_arduino_string);

                yield return null;
            }
            else
                yield return new WaitForSeconds(.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

#endif


        if (fail != null)
            fail();
        yield return null;
    }

    void pasarRF(string R)
    {
        RF_arduino_string = R;
        Debug.Log(R);

    }





    //READ FROM BOTH


    public IEnumerator AsynchronousReadFrom_Both_Foot(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

#if !UNITY_ANDROID  || UNITY_EDITOR

        do
        {
            Left_Foot_port.WriteLine("S");
            Right_Foot_port.WriteLine("S");
            try
            {
                LF_arduino_string = Left_Foot_port.ReadLine();
                RF_arduino_string = Right_Foot_port.ReadLine();
            }
            catch (TimeoutException)
            {
                LF_arduino_string = "1234";
                RF_arduino_string = "1234";
            }

            if (RF_arduino_string != null)
            {
                pasarRF(RF_arduino_string);
                callback(RF_arduino_string);

                yield return null;
            }
            if (LF_arduino_string != null)
            {
                pasarLF(LF_arduino_string);
                callback(LF_arduino_string);

                yield return null;
            }
            if( LF_arduino_string != null && RF_arduino_string != null  )
            {

            }
            else
                yield return new WaitForSeconds(.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

#endif


        if (fail != null)
            fail();
        yield return null;
    }


    void Obtain_Both_Foot()
    {
        StartCoroutine
  (
      AsynchronousReadFrom_Both_Foot

      ((string R) => pasarRF(R),     // Callback
          () => Debug.Log("Error!"), // Error callback
           10f                             // Timeout (seconds)
       )
   );
        Left_Foot_Raw = int.Parse(LF_arduino_string);
        Right_Foot_Raw = int.Parse(RF_arduino_string);

        //Left_alternancia = false;
        Convert_Right_Foot();
        Convert_Left_Foot();


    }
}

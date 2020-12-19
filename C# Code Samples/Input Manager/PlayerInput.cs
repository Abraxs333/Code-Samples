using UnityEngine;
using System.Collections;
using UnityEngine.UI;


using System;

#if UNITY_ANDROID  && !UNITY_EDITOR
using TechTweaking.Bluetooth;

#endif

#if !UNITY_ANDROID  || UNITY_EDITOR
using System.IO.Ports;

#endif

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    public Text inputtext;
    public Text Debugueo;
    Player player;

#if !UNITY_ANDROID || UNITY_EDITOR
    SerialPort stream;
#endif

#if UNITY_ANDROID  && !UNITY_EDITOR
    private BluetoothDevice device;
   // public Text statusText;
    //public Text inputtext;
#endif


    string arduinostring;
    float arduinofloat;

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

    void Start()
    {
        player = GetComponent<Player>();


#if UNITY_ANDROID  && !UNITY_EDITOR
        BluetoothAdapter.enableBluetooth();//Force Enabling Bluetooth
        device = new BluetoothDevice();
        if (player.tipo_de_control == 8)
        {
            //device.Name = "BETA";
            device.MacAddress = "00:13:03:19:01:52";
        }

        if (player.tipo_de_control == 3)
        { device.Name = "BG Glove"; }

#endif

#if !UNITY_ANDROID  || UNITY_EDITOR

        if (player.tipo_de_control == 5)
        {


            stream = new SerialPort(@"\\.\" + "COM10", 9600);
            //stream.ReadTimeout = 50;
            //stream.Open();

           connect();


        }
        if (player.tipo_de_control == 3)
        {


            stream = new SerialPort("COM5", 9600);
            // stream.ReadTimeout = 25;
            //stream.Open();
           connect();


        }

#endif


#if UNITY_ANDROID  && !UNITY_EDITOR

        if (player.tipo_de_control == 8 || player.tipo_de_control == 3)
        { connect(); }

#endif


    }

    public void connect()
    {


#if !UNITY_ANDROID  || UNITY_EDITOR
        stream.Open();
#endif

#if UNITY_ANDROID  && !UNITY_EDITOR
        device.connect();

#endif

    }

    public void disconnect()
    {

#if !UNITY_ANDROID  || UNITY_EDITOR
        stream.Close();
#endif

#if UNITY_ANDROID  && !UNITY_EDITOR
        device.close();
  
#endif
    }


#if UNITY_ANDROID  && !UNITY_EDITOR
    public void Sendmsg(string msgout)
    {
        if (device != null)
        {
            /*
			 * Send and Read works only with bytes. You need to convert everything to bytes.
			 * Different devices with different encoding is the reason for this. You should know what encoding you're using.
			 * In the method call below I'm using the ASCII encoding to send "Hello" + a new line.
			 */
            device.send(System.Text.Encoding.ASCII.GetBytes(msgout + "\n"));
        }
    }
#endif


    string dataString = null;
    string dataStringxxx = null;
    bool banderadata = false;

    string menique = null;
    string indice = null;
    string medio = null;
    string anular = null;

    string sensoranalogico = null;

    string[] entradas = { "0",  "0", "0000" };


    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        //Debugueo.text="XCCC";

        do
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            stream.WriteLine("S");
            Debugueo.text="ENVIADO";
#endif
#if UNITY_ANDROID  && !UNITY_EDITOR
            Sendmsg("S");
            yield return new WaitForSeconds(0.02f);
#endif

            try
            {
#if !UNITY_ANDROID  || UNITY_EDITOR
                dataString = stream.ReadLine();
                if (player.tipo_de_control > 2)
                { //Debugueo.text = dataString; 
                }

#endif
#if UNITY_ANDROID && !UNITY_EDITOR

                byte[] msg = device.read();

                if (msg != null)
                {

                    /* Send and read in this library use bytes. So you have to choose your own encoding.
                     * The reason is that different Systems (Android, Arduino for example) use different encoding.
                     */
                    string content = System.Text.ASCIIEncoding.ASCII.GetString(msg);

                    dataString = content;

                }
                //         else {
                //             StartCoroutine
                //             (
                //    AsynchronousReadFromArduino
                //    ((string s) => pasars(s),     // Callback
                //        () => Debug.Log("Error!"), // Error callback
                //        100f                             // Timeout (seconds)
                //    )
                //);

                //             yield break;
                //         }

#endif
                if (player.tipo_de_control == 50)
                {

                    if (dataString.Length < 5)
                    { dataString = dataStringxxx; }
                    else
                    { dataStringxxx = dataString; }

                   // Debugueo.text = dataString;
                }

                if (player.tipo_de_control == 3)
                {
                    
                    if (dataString.Length < 7)
                    { dataString = dataStringxxx;
                      Debugueo.text = dataString;
                    }
                    else
                    { dataStringxxx = dataString; }

                }

               

                if (player.tipo_de_control == 3 && dataString.Length > 6)
                {
                    //Debugueo.text = "MAS DE6";
                    char[] separators = { ';', '\n', '\r' };
                    string value = dataString;
                    entradas = value.Split(separators, StringSplitOptions.None);

                    if (entradas[0].Length < 2)
                    {
                        if (Convert.ToInt32(entradas[0]) < 2)
                        { indice = entradas[0]; }
                    }
                  
                    if (entradas[1].Length < 2)
                    {
                        if (Convert.ToInt32(entradas[1]) < 2)
                        { menique = entradas[1]; }
                    }


                    if (entradas[2].Length > 2)
                    { sensoranalogico = entradas[2]; }

                    //Debugueo.text = "dedos: " + indice + medio + anular + menique + '\n' + "ANG: " + sensoranalogico;
                    Debugueo.text = "CONNECTED";
                }


            }
            catch (TimeoutException)
            {
                dataString = "1000";
            }

            if (dataString != null)
            {
                pasars(dataString);
                callback(dataString);
                if (player.tipo_de_control == 3)
                { callback(sensoranalogico); }
                if (player.tipo_de_control == 50)
                { callback(dataString); }

                yield return null;
            }
            else
                yield return new WaitForSeconds(.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }

    float valoranalogicoactual;
    float valoranalogicoprevio;

    void pasars(string s)
    {
        arduinostring = s;
        Debug.Log(s);

    }



    void Update()
    {

        /*revisar tipos de control esta es la lista tentativa
 * 0    NADA         BIEN
 * 1    KINECT       N/A
 * 2    XBOX         BIEN
 * 3    GUANTE       BIEN
 * 4    JOYCON       N/A
 * 5    MANCUERNA    N/A
 * 6    N/A
 * 7    IMUS         N/A
 * 8    AR FOOWEAR   N/A
 * 9    TOUCHSCREEN  BIEN
 * */

        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.cronometrarinicio();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            player.cronometrarfinal();
        }


        //player.runVelocityVector.y = Input.GetAxis("LT");
        //presionado es 1



        ////control xbox

        if (player.tipo_de_control == 2)
        {
            valoranalogicoactual = Input.GetAxis("RT");


            actualizar_Input(valoranalogicoactual);

            float angulo = (float)ExtensionMethods.Map(valoranalogicoactual, 0, 1, 16, 160);



            player.valor_angulo = (float)angulo;

        }

        ////control joycon

        if (player.tipo_de_control == 4)
        {
            valoranalogicoactual = Input.GetAxis("JOYCON");
            player.DistanciaText.text = valoranalogicoactual.ToString();

            actualizar_Input(valoranalogicoactual);

            float angulo = (float)ExtensionMethods.Map(valoranalogicoactual, 0, 1, 16, 160);



            player.valor_angulo = (float)angulo;

        }

        ////GUANTE

        if (player.tipo_de_control == 3)
        {


            StartCoroutine
    (
        AsynchronousReadFromArduino
        ((string s) => pasars(s),     // Callback
            () => Debug.Log("Error!"), // Error callback
            10f                             // Timeout (seconds)
        )
    );

            //arduinostring = stream.ReadLine();

            arduinofloat = float.Parse(arduinostring);
            player.valorcrudo = arduinofloat;
            valoranalogicoactual = (float)ExtensionMethods.Map(arduinofloat, 170, 270, 0, 1);

            actualizar_Input(valoranalogicoactual);

            float angulo = (float)ExtensionMethods.Map(arduinofloat, 170, 270, 16, 160);



            player.valor_angulo = (float)angulo;

        }

        //CELULAR
        if (player.tipo_de_control == 9)
        {
            /*
#if UNITY_EDITOR
            valoranalogicoactual = (float)Input.GetMouseButton(1);
#endif
*/
            arduinofloat = Input.GetTouch(0).position.y;

            valoranalogicoactual = (float)ExtensionMethods.Map(arduinofloat, 0, Screen.height, 0, 1);

            actualizar_Input(valoranalogicoactual);

            float angulo = (float)ExtensionMethods.Map(arduinofloat, 0, Screen.height, 16, 160);



            player.valor_angulo = (float)angulo;
        }

        // Mancuerna
        if (player.tipo_de_control == 5)
        {
            try
            {
                StartCoroutine
        (
            AsynchronousReadFromArduino
            ((string s) => pasars(s),     // Callback
                () => Debug.Log("Error!"), // Error callback
                10f                             // Timeout (seconds)
            )
        );

                //arduinostring = stream.ReadLine();

                arduinofloat = float.Parse(arduinostring);
                player.valorcrudo = arduinofloat;
                valoranalogicoactual = (float)ExtensionMethods.Map(arduinofloat, 0, -180, 0, 1);

                actualizar_Input(valoranalogicoactual);

                float angulo = (float)ExtensionMethods.Map(arduinofloat, 0, -180, 16, 160);



                player.valor_angulo = (float)angulo;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

        }

    }

    //public void actualizar_giro(float valor)
    //{
    //    player.transform.Rotate(0, 0, 180 + valor);
    //}


    int indiceantes = 0;
    int indiceahora = 0;

    int meniqueantes = 0;
    int meniqueahora = 0;

    

    public void actualizar_Input(float valoranalogico)
    {

        valoranalogicoactual = valoranalogico;
        player.valor_RT = valoranalogicoactual;


        if (player.tipo_de_control == 3)

        {
            indiceahora = Convert.ToInt32(indice);
            if (indiceahora == 1 && indiceahora != indiceantes)
            {
                player.changeRunDirection("Derecha");
            }
            indiceantes = indiceahora;

            meniqueahora = Convert.ToInt32(menique);
            if (meniqueahora == 1 && meniqueahora != meniqueantes)
            {
                player.changeRunDirection("Izquierda");
            }
            meniqueantes = meniqueahora;



        }

        float deltaanalogica = valoranalogicoactual - valoranalogicoprevio;
        if (player.tipo_de_control == 3)
        {
            //esto quiere decir que aplica el control bt echo a la medida
            if (deltaanalogica > 0.15)
            {

                player.OnJumpInputDown(valoranalogico);

            }
        }
        else
        {

            if (deltaanalogica > 0.01)
            {

                player.OnJumpInputDown(valoranalogico);

            }
        }
        valoranalogicoprevio = valoranalogicoactual;
    }


}



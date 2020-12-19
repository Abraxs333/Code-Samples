using UnityEngine;
using System.Collections;
using System.Threading;
using UnityEngine.UI;

[RequireComponent (typeof (Controller2D))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Spriterotate))]


public class Player : MonoBehaviour {

    Animator animator;
    enum Animations { IniciarSalto, BajarVuelo, Correr, IniciarVuelo, Volar, TerminarVuelo, BardaDerecha };
    enum Direccion { Derecha, Izquierda};
    public int Direcciondemov=0;
    private Thread _t1;
    public Text FlightText;
    public Text DistanciaText;
    

    public Vector2 runVelocityVector;
    public float runVelocitySpeed;

    public int controllerScheme = 0;

	public float maxJumpHeight;
	public float minJumpHeight = 1;
	public float timeToJumpApex = 1f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 6;

    public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeToWallUnstick;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	public Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;

	Vector2 directionalInput;
	bool wallSliding;
	int wallDirX;

    float Level_start_time;
    float Level_end_time;
    float Current_time;
    float Jump_start_time;
    public float maxJumpTime;
    float time_since_jump;

    float Current_position;
    public bool estoyvolando = false;
    Player player;
    Spriterotate spriterotator;
    
    public float valor_angulo;
    public float valor_RT;
    public int estadoanimacion;
    public float valorcrudo;
    public int tipo_de_control;
    public float tiempodevuelo = 0;

	void Start() {
        tipo_de_control = Variables.tipodecontrol;
        
        if (tipo_de_control != 1)
        {
            runVelocitySpeed = .8f;
        }


        player = GetComponent<Player>();
        controller = GetComponent<Controller2D> ();
        //FlightText.text = "adios";
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
        Level_start_time = Time.time;
        
        //FlightText.text = "holamundo";
        animator = GetComponentInChildren<Animator>();
        Animations Animations;
        estadoanimacion = (int)Animations.Correr;
        
    }

    void Update() {
        HandleAnimation(estadoanimacion);
        CalculateVelocity();
        HandleWallSliding();
        HandleScore();
        HandleAnimation(estadoanimacion);
        //FlightText.text = textodevuelo;
        Current_time = Time.time;
        Current_position = transform.position.y;
        controllerScheme = controller.controllerscheme;
        
        controller.Move(velocity * Time.deltaTime, directionalInput);
        HandleAnimation(estadoanimacion);

        if (controller.collisions.above || controller.collisions.below)
        {

            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
                runVelocityVector.x = runVelocitySpeed;
            }
        }

    
    }



    void HandleAnimation (int caso)
    {
        switch (caso)
        {
            case (int)Animations.Correr:

               
                animator.ResetTrigger("Start_Jumping");
                animator.ResetTrigger("Fly");
                animator.ResetTrigger("Start_Fly");
                animator.ResetTrigger("End_Fly");
                animator.ResetTrigger("Fly_Down");
                animator.ResetTrigger("Barda_Derecha");

                animator.SetTrigger("Start_Running");
                break;
            case (int)Animations.IniciarSalto:
                animator.ResetTrigger("Start_Running");
            
                animator.ResetTrigger("Fly");
                animator.ResetTrigger("Start_Fly");
                animator.ResetTrigger("End_Fly");
                animator.ResetTrigger("Fly_Down");
                animator.ResetTrigger("Barda_Derecha");
                animator.SetTrigger("Start_Jumping");
                break;
            case (int)Animations.Volar:
                animator.ResetTrigger("Start_Running");
                animator.ResetTrigger("Start_Jumping");
              
                animator.ResetTrigger("Start_Fly");
                animator.ResetTrigger("End_Fly");
                animator.ResetTrigger("Fly_Down");
                animator.ResetTrigger("Barda_Derecha");
                animator.SetTrigger("Fly");
                break;
            case (int)Animations.IniciarVuelo:
                animator.ResetTrigger("Start_Running");
                animator.ResetTrigger("Start_Jumping");
                animator.ResetTrigger("Fly");
               
                animator.ResetTrigger("End_Fly");
                animator.ResetTrigger("Fly_Down");
                animator.ResetTrigger("Barda_Derecha");
                animator.SetTrigger("Start_Fly");
                break;
            case (int)Animations.TerminarVuelo:
                animator.ResetTrigger("Start_Running");
                animator.ResetTrigger("Start_Jumping");
                animator.ResetTrigger("Fly");
                animator.ResetTrigger("Start_Fly");
           
                animator.ResetTrigger("Fly_Down");
                animator.ResetTrigger("Barda_Derecha");
                animator.SetTrigger("End_Fly");
                break;
            case (int)Animations.BajarVuelo:
                animator.ResetTrigger("Start_Running");
                animator.ResetTrigger("Start_Jumping");
                animator.ResetTrigger("Fly");
                animator.ResetTrigger("Start_Fly");
                animator.ResetTrigger("End_Fly");
           
                animator.ResetTrigger("Barda_Derecha");
                animator.SetTrigger("Fly_Down");
                break;
            case (int)Animations.BardaDerecha:
                animator.ResetTrigger("Start_Running");
                animator.ResetTrigger("Start_Jumping");
                animator.ResetTrigger("Fly");
                animator.ResetTrigger("Start_Fly");
                animator.ResetTrigger("End_Fly");
                animator.ResetTrigger("Fly_Down");
        
                animator.SetTrigger("Barda_Derecha");
                break;

            default:
                break;

                
        }
        //return caso;
    }

    public void HandleScore()
    {

    }



    public void SetDirectionalInput (Vector2 input) {
       
        directionalInput = runVelocityVector;
        if (directionalInput.x > 0)
        {
            Direcciondemov = (int)Direccion.Derecha;
        }
        else if (directionalInput.x < 0)
        { Direcciondemov = (int)Direccion.Izquierda; }
    }
    public void changeRunDirection()
    {
        if (runVelocityVector.x > 0)
        {
            
            runVelocityVector.x = -(runVelocityVector.x);
            Direcciondemov = (int) Direccion.Izquierda;
        }
        else if (runVelocityVector.x < 0)
        {
            
            runVelocityVector.x = Mathf.Abs(runVelocityVector.x);
            Direcciondemov = (int)Direccion.Derecha;
        }
        SetDirectionalInput(runVelocityVector);
    }
    public void changeRunDirection(string Direccionamover)
    {
        switch (Direccionamover)
        {
            case "Derecha":
                runVelocitySpeed = Mathf.Abs(runVelocitySpeed);
                runVelocityVector.x = runVelocitySpeed;
                Direcciondemov = (int)Direccion.Derecha;
                break;
            case "Izquierda":
                runVelocitySpeed = -(Mathf.Abs(runVelocitySpeed));
                runVelocityVector.x = runVelocitySpeed;
                Direcciondemov = (int)Direccion.Izquierda;
                break;
        }
    }

    float xinicial;
    float xfinal;
    float Tinicial;
    float Tfinal;

    public void cronometrarinicio()
    {
        xinicial = transform.position.x;
        Tinicial = Current_time;
    }

    public void cronometrarfinal()
    {
        xfinal = transform.position.x;
        Tfinal = Current_time;

        DistanciaText.text = "Distancia: " + (xfinal - xinicial).ToString() + "Tiempo: " + (Tfinal - Tinicial).ToString() + "Velocidad " + ((xfinal - xinicial) / (Tfinal - Tinicial)).ToString();

    }

    public void OnJumpInputDown( float valor) {
             
            if (wallSliding)
            {

                if (wallDirX == directionalInput.x)
                {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;

                }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;

            }
            else
                {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                    changeRunDirection();
                estadoanimacion = (int)Animations.IniciarSalto;
               
            }
            }
            if (controller.collisions.below)
            {
                if (controller.collisions.slidingDownMaxSlope)
                {
                    if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                    { 
                        velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                        velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                    }
                }
                else
                {
                _t1 = new Thread(_salto);
                _t1.Start();
                    //salto();
                //velocity.y = maxJumpVelocity * valor;
                //Debug.Log("Velocidad: " + velocity.y.ToString() +"Registrado" +valor.ToString()+ "Ahorita" + Input.GetAxis("RT").ToString());
                }
            }

      
    }

    private void _salto()
    {
        
        if (controllerScheme == 0)
        {
            estadoanimacion = (int)Animations.IniciarSalto;

            textodevuelo = "no puedo volar";
            //Debug.Log("Velocidad: " + velocity.y.ToString() + "Registrado" + valor_RT.ToString() + "Tsalto" + time_since_jump.ToString());
            Jump_start_time = Current_time;
            time_since_jump = Current_time - Jump_start_time;
            float initial_height = Current_position;
            Debug.Log("Velocidad: " + velocity.y.ToString() + "Registrado" + valor_RT.ToString() + "Tsalto" + time_since_jump.ToString());
            while (time_since_jump < maxJumpTime)
            {
                if (valor_RT == 0)
                {
                    time_since_jump = maxJumpTime;
                }
                else
                {
                    gravity = 0;
                    if (Current_position > (initial_height + maxJumpHeight))
                    {
                        velocity.y = maxJumpVelocity * -.1f;
                    }
                    else if (Current_position > (initial_height + (maxJumpHeight / 3)))
                    {
                        velocity.y = maxJumpVelocity * (conversionanalogica(valor_RT) - .5f);
                    }
                    else
                    {
                        
                        velocity.y = maxJumpVelocity * conversionanalogica(valor_RT);
                        
                    }
                }
                
                //textodevuelo = "Vuelo " + (maxJumpTime - time_since_jump).ToString();
                time_since_jump = Current_time - Jump_start_time;
                Debug.Log("Velocidad: " + velocity.y.ToString() + "Registrado" + valor_RT.ToString() + "Tsalto" + time_since_jump.ToString());
            }
            gravity = gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        }
        else if (controllerScheme == 1)
        {
            estadoanimacion = (int)Animations.IniciarVuelo;
            estoyvolando = true;
            
            
            textodevuelo = "SI VUELO";
            maxJumpTime = 10;
            //Debug.Log("Velocidad: " + velocity.y.ToString() + "Registrado" + valor_RT.ToString() + "Tsalto" + time_since_jump.ToString());
            Jump_start_time = Current_time;
            time_since_jump = Current_time - Jump_start_time;
            float initial_height = Current_position;
            Debug.Log("Velocidad: " + velocity.y.ToString() + "Registrado" + valor_RT.ToString() + "Tsalto" + time_since_jump.ToString());
            while (time_since_jump < maxJumpTime)
            {
                gravity = 0;
                    velocity.y = maxJumpVelocity * (valor_RT-.5f);

             if (time_since_jump>1)
                {
                    estadoanimacion = (int)Animations.Volar;
                }
                time_since_jump = Current_time - Jump_start_time;

                //textodevuelo = "Tiempo de Vuelo restante: "+(maxJumpTime - time_since_jump).ToString();
                tiempodevuelo = maxJumpTime - time_since_jump;


                Debug.Log("Velocidad: " + velocity.y.ToString() + "Registrado" + valor_RT.ToString() + "Tsalto" + time_since_jump.ToString()+ "maxjumptime:" + maxJumpTime.ToString());
            }
            gravity = gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            estadoanimacion = (int)Animations.TerminarVuelo;
            estoyvolando = false;
        }

        while (!controller.collisions.below)
        {
            textodevuelo = "Aun no bajo";
            //estadoanimacion = (int)Animations.TerminarVuelo;
            //estadoanimacion = (int)Animations.IniciarSalto;
        }


        estadoanimacion = (int)Animations.TerminarVuelo;

        //}
        estadoanimacion = (int)Animations.Correr;
        estoyvolando = false;
    }

    string textodevuelo;

    float valoranalogicoactual;
    float [] valoranalogicoprevio= {0,0,0,0,0,0 };
    float analogmaxvalue = 1;
    float analogminvalue = 0;
    float deltaanalogica=0;
    float deltaanalogicaprevianegativa=0;
    float deltaanalogicapreviapositiva = 0;




    float conversionanalogica(float lecturaanalogica)
    {
        
        float valoranalogico = lecturaanalogica;


        //el punto 5 funciona bvien con el control de xbox
        //valoranalogico -= .5f;
        //CON ESTE CODIGO PUEDES MAPEAR PARA ADECUAR A LOS SUJETOS SI CONVIERTES EL SEGUNDO UNO EN LA RESPEUSTA MAXIMA DEL SUJETO

        valoranalogico = (float)ExtensionMethods.Map(lecturaanalogica, 0, 1, 0, 1);
        ////valoranalogicoprevio = (float)ExtensionMethods.Map(valoranalogicoprevio, 0, 1, 0, 1);

        //deltaanalogica = valoranalogicoactual - valoranalogicoprevio[5];

        //if (deltaanalogica < 0)
        //{

        //    valoranalogico = -1 * (valoranalogicoactual+.5f);
        //}
        ////else if (deltaanalogica > .01)
        ////{
        ////    valoranalogico = deltaanalogica + deltaanalogicapreviapositiva;
        ////    deltaanalogicapreviapositiva = deltaanalogica;
        ////}
        //else
        //{

        //    valoranalogico = valoranalogicoactual;
        //}


        //if (valoranalogico < 0)
        //{
        //    valoranalogico = valoranalogico * .5f;
        //}
        //if (valoranalogicoactual == valoranalogicoprevio)
        //{
        //    valoranalogico = 0;
        //}

        valoranalogicoprevio [0]= valoranalogicoactual;
        valoranalogicoprevio[1] = valoranalogicoprevio[0];
        valoranalogicoprevio[2] = valoranalogicoprevio[1];
        valoranalogicoprevio[3] = valoranalogicoprevio[2];
        valoranalogicoprevio[4] = valoranalogicoprevio[3];
        valoranalogicoprevio[5] = valoranalogicoprevio[4];
        return (valoranalogico);
    }
		

	void HandleWallSliding() {
		wallDirX = (controller.collisions.left) ? -1 : 1;
		wallSliding = false;
       
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;
            estadoanimacion = (int)Animations.BardaDerecha;
            HandleAnimation(estadoanimacion);
           
            if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }

        }
        if (wallSliding == false && estadoanimacion == (int)Animations.BardaDerecha)
        { estadoanimacion = (int)Animations.IniciarSalto; }
      
    }

	void CalculateVelocity() {
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;
	}
}

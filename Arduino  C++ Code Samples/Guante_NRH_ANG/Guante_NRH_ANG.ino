
//const int   pulgar=11; //asignacion de puertos digitales para cada dedo
const int   indice=A3;
const int   medio=A2;
const int   anular=A1;
const int   menique=A0;
const int   lateral=10;
const int   flexometro=A0;

void setup(){
  Serial.begin(9600);

  pinMode(indice, INPUT);
 // pinMode(lateral, INPUT);
  pinMode(medio, INPUT);
  pinMode(anular, INPUT);
  pinMode(menique, INPUT);
}
void loop(){

 //if (Serial.available()>0){

    
    Serial.print(digitalRead(indice));
    Serial.print(digitalRead(medio));
    Serial.print(digitalRead(anular));
    Serial.print(digitalRead(menique));
    Serial.println("_");
    Serial1.print("_");
    Serial1.println(analogRead(flexometro));
                    
 //}
  delay(50);
}

  

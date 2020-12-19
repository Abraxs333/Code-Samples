/*
Code written by Carlos A. Pérez-Herrera, Nadia Santillán & Rogelio Escobar &. November, 2014. 
http://analisisdelaconducta.net
National Autonomous University of Mexico
*/

byte Houselight = 11;
byte Feeder = 12;
byte Light_1= 6;
byte Light_2 = 7;

byte Response_1 = A0;
byte Response_2= A1;

String Response_A0="0";
String Response_A1="0";

int Response1_lower_limit = 1;
int Response2_lower_limit = 1;


//Change XXX for the apropiate value 
int Response1_upper_limit = XXX;
int Response2_upper_limit = XXX;

void setup(){
  Serial.begin(9600);
  pinMode(Light_1, OUTPUT);
  pinMode(Light_2, OUTPUT);
  pinMode(Houselight, OUTPUT);
  pinMode(Feeder, OUTPUT);

  pinMode(Response_1,INPUT);
  pinMode(Response_2,INPUT);

  
  digitalWrite (Light_1,LOW);
  digitalWrite (Light_2,LOW);
  digitalWrite (Houselight,LOW);
  digitalWrite (Feeder,LOW);
}
void loop(){

  if (Serial.available()>0){

    char Event=Serial.read();
    switch (Event){

    case 'A':
      digitalWrite (Light_1, HIGH);
      break;
    case 'B':
      digitalWrite (Light_1, LOW);
      break;
    case 'C':
      digitalWrite (Light_2, HIGH);
      break;
    case 'D':
      digitalWrite (Light_2, LOW);
      break;
    case 'S':
      digitalWrite (Houselight, HIGH);
      break;
    case 'R':
      digitalWrite(Feeder,HIGH);
      delay(100);
      digitalWrite (Feeder,LOW);
      break;
    case 'E':
      digitalWrite(Feeder,LOW);
      digitalWrite(Houselight,LOW);
      digitalWrite (Light_1, LOW);
      digitalWrite (Light_2, LOW);
      delay(500);
      break;
    }
  }

    if (analogRead(Response_1)>Response1_upper_limit || analogRead(Response_1)<Response1_lower_limit){
    Response_A0="0";
      }
    else if (analogRead(Response_1)>=Response1_lower_limit && analogRead(Response_1)<=Response1_upper_limit){
     Response_A0="1";
  }
      if (analogRead(Response_2)>Response2_upper_limit || analogRead(Response_2)<Response2_lower_limit){
    Response_A1="0";
  }
    else if (analogRead(Response_2)>=Response2_lower_limit && analogRead(Response_2)<=Response2_upper_limit){
    Response_A1="1";
  }
 
  Serial.println(Response_A0+","+Response_A1);
  delay (30);
}


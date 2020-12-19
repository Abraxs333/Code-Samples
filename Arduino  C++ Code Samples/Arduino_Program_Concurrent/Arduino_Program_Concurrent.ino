/*
Code written by Carlos A. Pérez-Herrera & Rogelio Escobar &. November, 2014. 
http://analisisdelaconducta.net
National Autonomous University of Mexico
*/

byte Houselight = 11;
byte Feeder = 12;
byte Response_1 = 8;
byte Response_2= 9;
byte Light_1= 6;
byte Light_2 = 7;

void setup(){
  Serial.begin(9600);
  pinMode(Light_1, OUTPUT);
  pinMode(Light_2, OUTPUT);
  pinMode(Houselight, OUTPUT);
  pinMode(Feeder, OUTPUT);
  pinMode(Response_1,INPUT_PULLUP);
  pinMode(Response_2,INPUT_PULLUP);
  
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
  Serial.println(String(digitalRead(Response_1))+","+String(digitalRead(Response_2)));
  delay (4);
}


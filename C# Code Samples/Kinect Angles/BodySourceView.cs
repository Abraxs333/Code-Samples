using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

using UnityEngine.UI;
using System;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Player))]
public static class ExtensionMethods
{
    public static double Map(this double value, double fromSource, double toSource, double fromTarget, double toTarget)
{
    return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
}
}

public class BodySourceView : MonoBehaviour
{
    PlayerInput playerInput;
    Player player;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public Text angulotext;
    public Text exitotext;

    public Material BoneMaterial;
    public GameObject BodySourceManager;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    void Update()
    {
        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
               
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach (var body in data)
        {
           
            if (body == null)
            {
               
                continue;
            }

            if (body.IsTracked)
            {

                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);

                }

                RefreshBodyObject(body, _Bodies[body.TrackingId]);

                //ejecutar movimiento
                if (player.tipo_de_control == 1)
                {
                    detectinput(body);
                }

                




            }
            
                

        }
    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }

        
        player.runVelocitySpeed = .8f;
        
        playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
        
        return body;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            //jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
    }



    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;

            case Kinect.TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }



    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {

        return new Vector3((joint.Position.X * 5) + GameObject.Find("BodyView").transform.position.x, (joint.Position.Y * 5) + GameObject.Find("BodyView").transform.position.y, (joint.Position.Z * 5) + GameObject.Find("BodyView").transform.position.z);
    }




    //ejecutarmovimiento

    private void detectinput (Kinect.Body body)
        {
        //double angulo = calcularangulo(body, Kinect.JointType.HipRight, Kinect.JointType.ShoulderRight, Kinect.JointType.WristRight);
        double angulo = calcularangulo(body, Kinect.JointType.HipRight, Kinect.JointType.ShoulderRight, Kinect.JointType.WristRight);
        float exito = (float) ExtensionMethods.Map(angulo, 16, 160, 0, 1);
        //float exito = Porcentaje_exito(angulo, 160);
        angulotext.text = "angulo " + angulo.ToString();
        exitotext.text =  " exito "+ exito.ToString();
        //player = GetComponent<Player>();
        player.valor_angulo = (float)angulo;
        playerInput.actualizar_Input(exito);

    }


    private float Porcentaje_exito(double angulo, double postura)
    {

        float Porcentaje = (float)(angulo / postura);
        return Porcentaje;

    }

    private double calcularangulo (Kinect.Body body, Kinect.JointType a, Kinect.JointType b, Kinect.JointType c )
    {
        Vector3 crossProduct;
        Vector3 joint0tojoint1;
        Vector3 joint1tojoint2;

        //Kinect.Joint joint0 = skeleton.Joints[type0];

        Kinect.Joint joint0 = body.Joints[a];
        Kinect.Joint joint1 = body.Joints[b];
        Kinect.Joint joint2 = body.Joints[c];

        //calcula el vector
        joint0tojoint1 = new Vector3(joint0.Position.X - joint1.Position.X, joint0.Position.Y - joint1.Position.Y, joint0.Position.Z - joint1.Position.Z);
        joint1tojoint2 = new Vector3(joint2.Position.X - joint1.Position.X, joint2.Position.Y - joint1.Position.Y, joint2.Position.Z - joint1.Position.Z);
        joint0tojoint1.Normalize();
        joint1tojoint2.Normalize();

        //magia
        float dotProduct = Vector3.Dot(joint0tojoint1, joint1tojoint2);        
        crossProduct = Vector3.Cross(joint0tojoint1, joint1tojoint2);
        float crossProdLenght = crossProduct.magnitude;       
        float angleFormed = Mathf.Atan2(crossProdLenght, dotProduct);

        //calcual el angulo
        double angleinDegree = angleFormed * (180 / Mathf.PI);
        double roundedAngle = Math.Round(angleinDegree, 2);

        return roundedAngle;
    }

}

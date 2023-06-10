using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAdvanced : MonoBehaviour
{
  //CHANGE: ADDED BODYSOURCEMANAGER
  public GameObject MyBodySourceManager;
  private MyBodySourceManager _BodyManager;

  public Transform Head;
  public Transform Neck;
  public Transform SpineShoulder;
  public Transform ShoulderRight;
  public Transform ElbowRight;
  public Transform WristRight;
  public Transform ShoulderLeft;
  public Transform ElbowLeft;
  public Transform WristLeft;
  public Transform SpineBase;
  public Transform HipRight;
  public Transform KneeRight;
  public Transform FootRight;
  public Transform HipLeft;
  public Transform KneeLeft;
  public Transform FootLeft;
  //CHANGE: PRIVATE TO PUBLIC


  // Use this for initialization
  void Start()
  {
    //CHANGE: ADDED BODYSOURCEMANAGER
    if (MyBodySourceManager == null)
    {
      Debug.Log("No hay script");
      return;
    }

    _BodyManager = MyBodySourceManager.GetComponent<MyBodySourceManager>();
    if (_BodyManager == null)
    {
      Debug.Log("No hay clase");
      return;
    }
    // If discrete cont
  }


  // Update is called once per frame
  void Update()
  {
    // headPosition = GameObject.Find("PK").transform.Find("XR Rig").transform.Find("Camera Offset").transform.Find("Main Camera").transform.localPosition.y;

    // if (!Globals.fixedControllerPosition)
    // {        
    //     // Adjust controller levels according to camera's y-position
    //     lvlOne = headPosition - 0.4f;
    //     lvlTwo = headPosition - 0.4f;
    //     lvlThree = headPosition - 0.6f;
    // }

    // // Force applied by pilot to tilt the paraglide during rotation
    //  controllerTiltZ = 0f;

    // // Restore paraglide's default forward speed and descend speed if it is not affected by the pilot
    // if (restoreSpeed)
    //     speed = Mathf.Lerp(speed, pk.speed, Time.deltaTime);
    // if (restoreDescend)
    //     descend = Mathf.Lerp(descend, pk.descend, Time.deltaTime);
    // HUD.GetComponent<TestHUD>().UpdateHUD();

    // // Get controllers' y-position
    // //CHANGE: Commented and updates
    // //rightY = right.transform.localPosition.y;
    // //leftY = left.transform.localPosition.y;
    // rightY = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.WristRight).y;
    // leftY = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.WristLeft).y;
    Head.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.Head);
    Neck.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.Neck);
    SpineShoulder.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.SpineShoulder);
    ShoulderRight.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.ShoulderRight);
    ElbowRight.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.ElbowRight);
    WristRight.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.WristRight);
    ShoulderLeft.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.ShoulderLeft);
    ElbowLeft.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.ElbowLeft);
    WristLeft.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.WristLeft);
    SpineBase.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.SpineBase);
    HipRight.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.HipRight);
    KneeRight.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.KneeRight);
    FootRight.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.FootRight);
    HipLeft.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.HipLeft);
    KneeLeft.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.KneeLeft);
    FootLeft.position = _BodyManager.GetBodyJointPosModded(Windows.Kinect.JointType.FootLeft);
    // Rotate paraglide according to chosen controls
    //Debug.Log("continuousRotation: " + Globals.continuousRotation + ", isFlying: " + Globals.isFlying + ", isPaused: " + Globals.isPaused);
    //CHANGE: ALWAYS TRUE
    // Globals.SetFlying(true);
    // if (Globals.continuousRotation && Globals.isFlying && !Globals.isPaused)
    //     RotateContinuous(rightY, leftY, controllerTiltZ, headPosition);
    // else if (!Globals.continuousRotation && Globals.isFlying && !Globals.isPaused)
    //     RotatePK(rightY, leftY, controllerTiltZ);

  }
}
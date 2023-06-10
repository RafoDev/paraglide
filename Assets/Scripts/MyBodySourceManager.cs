using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Windows.Kinect;
using System;
using UnityEditor.PackageManager.UI;

public class MyBodySourceManager : MonoBehaviour
{
  public Material BoneMaterial;

  private KinectSensor _Sensor;
  private BodyFrameReader _Reader;
  private Body firstBody = null;
  private Body[] _Data = null;
  private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
  private Dictionary<string, Windows.Kinect.JointType> jointTypeByName = new Dictionary<string, Windows.Kinect.JointType>();
  public Body[] GetData()
  {
    return _Data;
  }

  void Start()
  {
    _Sensor = KinectSensor.GetDefault();

    if (_Sensor != null)
    {
      _Reader = _Sensor.BodyFrameSource.OpenReader();

      if (!_Sensor.IsOpen)
      {
        _Sensor.Open();
      }
    }

    foreach (Windows.Kinect.JointType jointType in Enum.GetValues(typeof(Windows.Kinect.JointType)))
    {
      jointTypeByName[jointType.ToString()] = jointType;
    }
  }

  private void updateFrame()
  {
    if (_Reader != null)
    {
      var frame = _Reader.AcquireLatestFrame();
      if (frame != null)
      {
        if (_Data == null)
          _Data = new Body[_Sensor.BodyFrameSource.BodyCount];

        frame.GetAndRefreshBodyData(_Data);

        // Iterar sobre los cuerpos detectados en _Data
        foreach (Body body in _Data)
        {
          if (body != null && body.IsTracked)
          {
            firstBody = body;
            break;
          }
        }

        frame.Dispose();
        frame = null;
      }
    }
  }

  public bool isThereBody() { return firstBody != null; }
  public Vector3 GetBodyJointPos(JointType jt)
  {
    if (firstBody == null) return Vector3.zero;
    Windows.Kinect.Joint myJoint = firstBody.Joints[jt];
    return new Vector3(myJoint.Position.X * 10, myJoint.Position.Y * 10, myJoint.Position.Z * 10);
  }

  public Vector3 GetBodyJointPosModded(JointType jt)
  {
    if (firstBody == null) return Vector3.zero;
    Windows.Kinect.Joint myJoint = firstBody.Joints[jt];
    return new Vector3(myJoint.Position.X * 1, myJoint.Position.Y * 1, myJoint.Position.Z * 1);
  }

  void Update()
  {
    updateFrame();
  }

  void OnApplicationQuit()
  {
    if (_Reader != null)
    {
      _Reader.Dispose();
      _Reader = null;
    }

    if (_Sensor != null)
    {
      if (_Sensor.IsOpen)
      {
        _Sensor.Close();
      }
      _Sensor = null;
    }
  }
}

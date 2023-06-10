using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //CHANGE: ADDED BODYSOURCEMANAGER
    public GameObject MyBodySourceManager;
    private MyBodySourceManager _BodyManager;

    public float turnThreshold = 0.05f;

    public bool Sp_up;
    public bool speed_up = false;

    public PK pk;                           // Scriptable object of paraglide profile
    public GameObject HUD;

    // DISCRETE CONTROLS //
    // Controllers fixed y-position levels
    public float lvlOne = 1.6f;             // Default controller's position is above 1.6 meters
    public float lvlTwo = 1.4f;             // First level is defined between 1.4 and 1.6 meters
    public float lvlThree = 1.2f;           // Second level is defined between 1.2 and 1.4 meters
                                            // Anything below 1.2 meters is considered as third level

    // Levels of tilting and rotating speed. Basic paraglide's rotation and tilting coefficients are
    // divided into thirds and each third is used accordingly to the controllers' y-position
    private float tiltOne;                   
    private float tiltTwo;
    private float tiltThree;

    private float rotateOne;
    private float rotateTwo;
    private float rotateThree;

    // BASIC MOVEMENT ATTRIBUTES//
    
    private float speed;                   // Speed of the paraglide. This attribute corresponds to the speed that can be adjusted by the pilot (air force is separated)
    private float t = 0f;                  // Speed interpolation parameter
    private float descend;                 // Paraglide's descend speed 
    
    //CHANGE
    private bool restoreSpeed = false;      // Bool to decide whether to restore paraglide's default speed
    private bool restoreDescend = true;    // Bool to decide whether to restore paraglide's default descend speed
    public float turnY;                    // Paraglide's current rotation around y-axis
    private Rigidbody rb;       

    public GameObject right,               // Right controller
                      left;                // Left controller


    //ENVIRONMENT PHYSICS ATTRIBUTES // 
    private EnvironmentPhysics envPhysics;  
    private Vector3 airForce;              // Force of the air flows to be applied to the paraglide;
    public float airTiltZ = 0f;            // Force of the environment to be applied and tilt the paraglide

    //CHANGE: PRIVATE TO PUBLIC
    public float rightY, leftY, controllerTiltZ, headPosition;
    public bool diff, diff1h, diff2h, hombros, diff1s, diff2s;


    // Use this for initialization
    void Start()
    {
        //CHANGE: ADDED BODYSOURCEMANAGER
        if (MyBodySourceManager == null) return;

        _BodyManager = MyBodySourceManager.GetComponent<MyBodySourceManager>();
        if (_BodyManager == null) return;

        // Initial paraglide's y-axis rotation
        turnY = transform.eulerAngles.y;

        // If no profile was chosen in the menu, default is set
        if (Globals.profile != null)
            pk = Globals.profile;

        // If discrete controls are chosen, divide the attributes used for each controller level
        if (!Globals.continuousRotation) {
            tiltOne = pk.maxTilt / 3f;
            tiltTwo = pk.maxTilt * 2f / 3f;
            tiltThree = pk.maxTilt;

            rotateOne = pk.maxRotateSpeed / 3f;
            rotateTwo = pk.maxRotateSpeed * 2f / 3f;
            rotateThree = pk.maxRotateSpeed;
        }
 
        speed = 200;
        descend = pk.descend;
        rb = GetComponent<Rigidbody>();
        envPhysics = GameObject.Find("Environment Physics").GetComponent<EnvironmentPhysics>();
        airForce = new Vector3(0, 0, 0);
    }
    
    void FixedUpdate()
    {
        rb.velocity = new Vector3(transform.forward.x * speed, descend, transform.forward.z * speed) + airForce;
        if (EnvironmentPhysics.windActive && envPhysics != null)
            envPhysics.ApplyWind();
    }

    // Update is called once per frame
    void Update()
    {
        headPosition = GameObject.Find("PK").transform.Find("XR Rig").transform.Find("Camera Offset").transform.Find("Main Camera").transform.localPosition.y;

        if (!Globals.fixedControllerPosition)
        {        
            // Adjust controller levels according to camera's y-position
            lvlOne = headPosition - 0.4f;
            lvlTwo = headPosition - 0.4f;
            lvlThree = headPosition - 0.6f;
        }

        // Force applied by pilot to tilt the paraglide during rotation
         controllerTiltZ = 0f;

        // Restore paraglide's default forward speed and descend speed if it is not affected by the pilot
        if (restoreSpeed)
            speed = Mathf.Lerp(speed, pk.speed, Time.deltaTime);
        if (restoreDescend)
            descend = Mathf.Lerp(descend, pk.descend, Time.deltaTime);
        HUD.GetComponent<TestHUD>().UpdateHUD();

        // Get controllers' y-position
        //CHANGE: Commented and updates
        //rightY = right.transform.localPosition.y;
        //leftY = left.transform.localPosition.y;
        rightY = _BodyManager.GetBodyJointPos(Windows.Kinect.JointType.WristRight).y;
        leftY = _BodyManager.GetBodyJointPos(Windows.Kinect.JointType.WristLeft).y;

        // Rotate paraglide according to chosen controls
        //Debug.Log("continuousRotation: " + Globals.continuousRotation + ", isFlying: " + Globals.isFlying + ", isPaused: " + Globals.isPaused);
        //CHANGE: ALWAYS TRUE
        Globals.SetFlying(true);
        if (Globals.continuousRotation && Globals.isFlying && !Globals.isPaused)
            RotateContinuous(rightY, leftY, controllerTiltZ, headPosition);
        else if (!Globals.continuousRotation && Globals.isFlying && !Globals.isPaused)
            RotatePK(rightY, leftY, controllerTiltZ);

    }

    /// <summary>
    /// Uses y-position of both controllers to rotate and tilt the paraglide according to
    /// which level are the controllers in
    /// </summary>
    /// <param name="rightY"> y-position of the right controller </param>
    /// <param name="leftY"> y-position of the left controller </param>
    /// <param name="controllerTiltZ"> Force applied by pilot to tilt the paraglide </param>
    public void RotatePK(float rightY, float leftY, float controllerTiltZ)
    {
        Debug.Log("RotatePK Controllers");
        // Right above level 1
        if (rightY > lvlOne)
        {
            // restoreSpeed = true;
            restoreDescend = true;
            if (leftY > lvlOne)                             // Left above level 1
                controllerTiltZ = 0f;
            else if (leftY <= lvlOne && leftY >= lvlTwo)    // Left between levels 1 and 2
            {
                controllerTiltZ = tiltOne;
                turnY -= 1 * rotateOne;
            }
            else if (leftY < lvlTwo && leftY >= lvlThree)   // Left between levels 2 and 3
            {
                controllerTiltZ = tiltTwo;
                turnY -= 1 * rotateTwo;
            }
            else                                            // Left below level 3
            {
                controllerTiltZ = tiltThree;
                turnY -= 1 * rotateThree;
            }
        }
        // Right between levels 1 and 2
        else if (rightY <= lvlOne && rightY >= lvlTwo)     
        {
            restoreDescend = true;
            // restoreSpeed = true;
            if (leftY > lvlOne)                            // Left above level 1
            {
                controllerTiltZ = -1 * tiltOne;
                turnY -= -1 * rotateOne;
                t = 0f;
            }
            else if (leftY <= lvlOne && leftY >= lvlTwo)   // Left between levels 1 and 2
            {
                restoreDescend = false;
                restoreSpeed = false;
                controllerTiltZ = 0f;
                speed = Mathf.Lerp(speed, 8f, t);
                descend = Mathf.Lerp(descend, -2f, t);
                t += 0.05f * Time.deltaTime;
            }
            else if (leftY < lvlTwo && leftY >= lvlThree)  // Left between levels 2 and 3
            {
                controllerTiltZ = tiltOne;
                turnY -= 1 * rotateOne;
                t = 0f;
            }
            else                                           // Left below level 3
            {
                controllerTiltZ = 1 * tiltTwo;
                turnY -= 1 * rotateTwo;
                t = 0f;
            }
        }

        // Right between levels 2 and 3
        else if (rightY < lvlTwo && rightY >= lvlThree)
        {
            // restoreSpeed = true;
            restoreDescend = true;
            if (leftY > lvlOne)                            // Left above level 1
            {
                controllerTiltZ = -1 * tiltTwo;
                turnY -= -1 * rotateTwo;
                t = 0f;
            }
            else if (leftY <= lvlOne && leftY >= lvlTwo)   // Left between levels 1 and 3
            {
                controllerTiltZ = -1 * tiltOne;
                turnY -= -1 * rotateOne;
                t = 0f;
            }
            else if (leftY < lvlTwo && leftY >= lvlThree)  // Left between levels 2 and 3 
            {
                restoreDescend = false;
                restoreSpeed = false;
                controllerTiltZ = 0f;
                speed = Mathf.Lerp(speed, 6f, t);
                descend = Mathf.Lerp(descend, -3f, t);
                t += 0.05f * Time.deltaTime;
            }
            else                                           // Left below level 3
            {
                controllerTiltZ = tiltOne;
                turnY -= 1 * rotateOne;
                t = 0f;
            }
        }
        // right below level 3
        else
        {
            // restoreSpeed = true;
            restoreDescend = true;
            if (leftY > lvlOne)                            // Left above level 1
            {
                controllerTiltZ = -1 * tiltThree;
                turnY -= -1 * rotateThree;
                t = 0f;
            }
            else if (leftY <= lvlOne && leftY >= lvlTwo)   // Left between levels 1 and 2
            {
                controllerTiltZ = -1 * tiltTwo;
                turnY -= -1 * rotateTwo;
                t = 0f;
            }
            else if (leftY < lvlTwo && leftY >= lvlThree)  // Left between levels 2 and 3
            {
                controllerTiltZ = -1 * tiltOne;
                turnY -= -1 * rotateOne;
                t = 0f;
            }
            else                                           // Left below level 3
            {
                restoreDescend = false;
                restoreSpeed = false;
                controllerTiltZ = 0f;
                speed = Mathf.Lerp(speed, 4f, t);
                descend = Mathf.Lerp(descend, -4f, t);
                t += 0.05f * Time.deltaTime;
            }
        }

        float tiltZ = controllerTiltZ + airTiltZ;

        Quaternion rotation = Quaternion.Euler(0f, turnY, tiltZ);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1f);
    }

    private bool calculatePosition_head(Vector3 head, Vector3 point1, Vector3 point2,Vector3 point1_s, Vector3 point2_s){
        // Calcular la diferencia en las coordenadas y respecto a la cabeza
         diff1h = head.y < point1.y;
         diff2h = head.y < point2.y;
        // Aseggurar que los brasos esten abiertos y no solo hacia arriba
         diff1s = point1.z < point1_s.z;
         diff2s = point2.z > point2_s.z;
         // Calculamos la distancia en el eje x entre las muñecas y los hombros
        //float distanciaDerecha = Mathf.Abs(point2.x - point2_s.x);
        //float distanciaIzquierda = Mathf.Abs(point1.x - point1_s.x);

        //float distanciaPromedio = (distanciaDerecha + distanciaIzquierda) / 2f;
         hombros=false;
        // Comprobamos si las muñecas están más alejadas que los hombros en el eje x
        if (diff1s&&diff2s) {
            hombros=true;
        }

        Debug.Log("ACELERA diff1h: " + diff1h + ", diff2h: " + diff2h +", hombros: "+ hombros);

        //Si todas las caracteristicas se cumplen esta acelerando

        return (diff1h&&diff2h&&hombros);
    }

     private bool calculatePosition_foot(Vector3 head, Vector3 point1, Vector3 point2,Vector3 point1_s, Vector3 point2_s){

  diff1h = head.y < point1.y;
         diff2h = head.y < point2.y;
        // Aseggurar que los brasos esten abiertos y no solo hacia arriba
         diff1s = point1.z < point1_s.z;
         diff2s = point2.z > point2_s.z;
          hombros=false;
        // Comprobamos si las muñecas están más alejadas que los hombros en el eje x
        if (diff1s&&diff2s) {
            hombros=true;
        }

        return (diff1h&&diff2h&&hombros);
    }
    
    /// <summary>
    /// Continuous rotation of the paraglide. 
    /// Calculates the force of turning and tilting according to controllers' y-position.
    /// Brake system. Slow down the paraglide and adjust descend speed, when both controllers are below headset and approximately at the same height.
    /// </summary>
    /// <param name="rightY"> y-position of right controller </param>
    /// <param name="leftY"> y-position of left controller </param>
    /// <param name="controllerTiltZ"> Force applied by pilot to tilt the paraglide </param>
    /// <param name="headPosition"> y-position of headset </param>
    public void RotateContinuous(float rightY, float leftY, float controllerTiltZ, float headPosition)
    {
        //Debug.Log("RotateContinuous");
        // +0.0001f in case controllers y-position was 0 (cannot be negative)
        float L = -pk.continuousCoefficient / (leftY + 0.0001f);
        float R = pk.continuousCoefficient / (rightY + 0.0001f);
        float turnDegrees = L + R;
        Debug.Log("L: " + L + ", R: " + R + ", turnDegrees: " + turnDegrees+", speed: "+ speed);


        // If the difference between y-position of the controllers is too big, set it to max value
        if (turnDegrees > pk.turnSharpnes)
            turnDegrees = pk.turnSharpnes;
        if (turnDegrees < -pk.turnSharpnes)
            turnDegrees = -pk.turnSharpnes;

           Sp_up = calculatePosition_head(_BodyManager.GetBodyJointPos(Windows.Kinect.JointType.Head),
                _BodyManager.GetBodyJointPos(Windows.Kinect.JointType.WristLeft),
                _BodyManager.GetBodyJointPos(Windows.Kinect.JointType.WristRight),
                _BodyManager.GetBodyJointPos(Windows.Kinect.JointType.ShoulderLeft),
                _BodyManager.GetBodyJointPos(Windows.Kinect.JointType.ShoulderRight));
        
            bool altura=calculatePosition_foot(_BodyManager.GetBodyJointPos(Windows.Kinect.JointType.Head),
                _BodyManager.GetBodyJointPos(Windows.Kinect.JointType.WristLeft),
                _BodyManager.GetBodyJointPos(Windows.Kinect.JointType.WristRight),
                _BodyManager.GetBodyJointPos(Windows.Kinect.JointType.ShoulderLeft),
                _BodyManager.GetBodyJointPos(Windows.Kinect.JointType.ShoulderRight));
        if(altura){
        //    GameObject.Find("PK").transform.Find("XR Rig").transform.Find("Camera Offset").transform.Find("Main Camera").transform.localPosition.y= headPosition- 1.6f;
           GameObject.Find("PK").transform.localPosition = GameObject.Find("PK").transform.localPosition - new Vector3(0,5,0);

        }
        // If controllers are below headset and approximately in the same height, slow down the paraglide and adjust descend speed
        // and stop trying to restore paraglide's speed, as it's being adjusted by the pilot
        if (Mathf.Abs(turnDegrees) < 0.05 && leftY < headPosition - 0.20f)
        {
            float brakeCoefficient = headPosition - leftY; // 0.20m - 0.30m
            if (brakeCoefficient > 0.3f)
                brakeCoefficient = 0.3f;
            //speed = Mathf.Lerp(speed, pk.speed - brakeCoefficient * 20, Time.deltaTime);
            descend = Mathf.Lerp(descend, pk.descend - 10 * brakeCoefficient, Time.deltaTime);
            HUD.GetComponent<TestHUD>().UpdateHUD();

            restoreSpeed = false;
            restoreDescend = false;
        }
        else if(Mathf.Abs(turnDegrees) < 0.05 && Sp_up)
        {
            speed =Mathf.Lerp(speed, 5+pk.speed, Time.deltaTime);
            restoreSpeed = false;
            restoreDescend = false;
            Debug.Log("ACELERA diff1h: " + diff1h + ", diff2h: " + diff2h +", hombros: "+ hombros);

        }
        else{
             // restoreSpeed = true;
            restoreDescend = true;
        }

        controllerTiltZ = -turnDegrees * 100f;
   
        float tiltZ = airTiltZ + controllerTiltZ;
        turnY += turnDegrees;
        Quaternion rotation = Quaternion.Euler(0f, turnY, tiltZ);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
    }

    // GETTERS //
    public Vector3 GetAirForce()
    {
        return airForce;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetDescend()
    {
        return descend;
    }

    /// <summary>
    /// Applies the force of a certain air flow on the paraglide.
    /// </summary>
    /// <param name="magnitude"> Magnitude of the air flow force to be applied on the paraglide </param>
    public void AddAirForce(float magnitude, Vector3 direction)
    {
        airForce += direction * magnitude;
        HUD.GetComponent<TestHUD>().UpdateHUD();
    }

    public void RemoveAirForce(float magnitude, Vector3 direction)
    {
        airForce -= direction * magnitude;
        HUD.GetComponent<TestHUD>().UpdateHUD();
    }
}
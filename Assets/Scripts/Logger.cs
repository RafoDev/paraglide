using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logger : MonoBehaviour
{
    public List<Vector3> PKPositons;
    public List<Quaternion> PKRotations;
    public List<Vector3> rightControllerPositions;
    public List<Vector3> leftControllerPositions;
    public List<Quaternion> headRotations;
    public List<float> currentSpeeds;
    public List<float> windIntensities;
    public List<int> windDirections;

    public List<Vector3> windCurrentPositions;
    public List<float> windCurrentSpeeds;
    public List<Quaternion> windCurrentRotations;
    public List<Vector3> windCurrentScales;

    [SerializeField]
    private GameObject PK;
    [SerializeField]
    private GameObject terrain;
    [SerializeField]
    private GameObject rightController;
    [SerializeField]
    private GameObject leftController;
    [SerializeField]
    private GameObject VRCamera;
    [SerializeField]
    private GameObject environment;

    private DateTime startTime;
    private float frameTimer;

    // Start is called before the first frame update
    /// <summary>
    /// Start method adds wind current info to appropriate lists
    /// They will be used in saving the replay
    /// </summary>
    void Start()
    {
        EventManager.manager.OnFlightEnded += SaveOnFlightEnded;
        // Marks the start of the recording
        startTime = DateTime.Now;

        foreach (Transform w in environment.transform)
        {
            if (w != environment.transform)
            {
                windCurrentPositions.Add(w.transform.position);
                windCurrentSpeeds.Add(w.GetComponent<AirFlow>().force);
                windCurrentRotations.Add(w.transform.rotation);
                windCurrentScales.Add(w.transform.localScale);
            }

        }
        frameTimer = 0f;
    }

    // Update is called once per frame
    /// <summary>
    /// Adds passed time between frames to frameTimer
    /// Check if the value of frametimer is higher than one period of sampling
    /// If yes, all the flight info needed will be added to the lists and resets frameTimer to 0
    /// The lists will be used in saving the replay
    /// </summary>
    void LateUpdate()
    {
        if (Globals.isLive)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= 1f / Globals.samplingFrequency)
            {
                PKPositons.Add(PK.transform.position);
                PKRotations.Add(PK.transform.rotation);
                rightControllerPositions.Add(rightController.transform.position);
                leftControllerPositions.Add(leftController.transform.position);
                windDirections.Add(EnvironmentPhysics.windDirection);
                windIntensities.Add(EnvironmentPhysics.windIntensity);
                headRotations.Add(VRCamera.transform.rotation);
                currentSpeeds.Add(PK.GetComponent<Movement>().GetSpeed());
                frameTimer = 0f;
            }
        }
    }

    /// <summary>
    /// If Globals.saveReplay is true it calls SaveLog() method to save the file
    /// </summary>
    void SaveOnFlightEnded()
    {
        if (Globals.saveReplay)
            SaveLog();
        else
            Debug.Log("Replay not saved");
    }

    /// <summary>
    /// Saves the data, that was logged during the flight into a json object and then to .json file
    /// Name of the file is based on the startTime variable
    /// </summary>
    /// <exception cref="IOException">
    /// Exception is caught if saving the file fails
    /// </exception>
    /// <returns>
    /// true if saving succeeds
    /// false if exception is caught
    /// </returns>
    // Return true if saving is successful, false otherwise
    public bool SaveLog()
    {
        // Globals.SetLive(false);
        // Header header = new Header((DateTime.Now - startTime).TotalSeconds, Globals.samplingFrequency, terrain.name, JsonUtility.ToJson(PK.GetComponent<Movement>().pk));

        // SaveObject saveObject = new SaveObject(header, windCurrentPositions, windCurrentSpeeds, windCurrentScales, windCurrentRotations, PKPositons, PKRotations,
        //                         rightControllerPositions, leftControllerPositions, headRotations, currentSpeeds, windIntensities, windDirections);

        // Debug.Log(saveObject.ToString());
        // string json = JsonUtility.ToJson(saveObject, true);

        // try
        // {
        //     File.WriteAllText(Globals.saveFolder + GenerateSaveFormat(), json);
        // }
        // catch (IOException e)
        // {
        //     Debug.LogError(e.Message.ToString());
        //     return false;
        // }
        return true;
    }

    /// <summary>
    /// Loads the data from .json file and puts them into SaveObject object
    /// </summary>
    /// <param name="path">
    /// Relative path to the file location
    /// </param>
    /// <exception cref="Exception">
    /// Exception is caught if loading of the file fails
    /// Exception is caught if conversion of string to json fails
    /// </exception>
    /// <returns>
    /// SaveObject instance loadObject if loading and converting to SaveObject succeeds
    /// null if exception is caught
    /// null if the file in the inputted path does not exist
    /// </returns>
    public static SaveObject LoadLog(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                string saveString = File.ReadAllText(path);
                SaveObject loadObject = JsonUtility.FromJson<SaveObject>(saveString);
                Debug.Log(loadObject.ToString());
                return loadObject;
            } catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError("Json file is corrupt");
                return null;
            }
        }
        else
        {
            Debug.LogError("File does not exist");
            return null;
        }

    }

    // 
    // returns the name of the file
    /// <summary>
    /// Generates string format of a name of .json file to save data into
    /// The format is based on startTime variable
    /// </summary>
    /// <returns>
    /// string containing the name of the replay
    /// </returns>
    public string GenerateSaveFormat()
    {
        var save = $"{startTime.Year}-{startTime.Month}-{startTime.Day}-{startTime.Hour}-{startTime.Minute}-{startTime.Second}.json";
        return save;
    }

    /// <summary>
    /// Sets the start time to current moment
    /// Currently unused
    /// </summary>
    public void SetStartTime()
    {
        startTime = DateTime.Now;
    }
}

/// <summary>
/// Class representing the header of Saveobject file
/// It contains the duration of recorded flight, it's sample frequency, terrain name and PK profile info
/// </summary>
[Serializable]
public class Header
{
    public double replayTime;
    public int sampleFrequency;
    public string terrainPrefabName;
    public string PKProfile;

    public Header(double replayTime, int sampleFrequency, string terrainPrefabName, string PKProfile)
    {
        this.replayTime = replayTime;
        this.sampleFrequency = sampleFrequency;
        this.terrainPrefabName = terrainPrefabName;
        this.PKProfile = PKProfile;
    }

    /// <summary>
    /// Generates string format of Header object
    /// </summary>
    /// <returns>
    /// string of the object
    /// </returns>
    public override string ToString()
    {
        var output = "Replay Time: " + (int)replayTime;
        output += $"\nTerrain name: {terrainPrefabName}";
        output += $"\nSample Frequency: {sampleFrequency}\n";
        return output;
    }
}

/// <summary>
/// Object holding all the necessary data about the recorded flight, to be replayed again
/// Contains Header object 
/// </summary>
[Serializable]
public class SaveObject
{
    public Header header;
    public List<Vector3> PKPositions;
    public List<Quaternion> PKRotations;
    public List<Vector3> rightControllerPositions;
    public List<Vector3> leftControllerPositions;
    public List<Quaternion> headRotations;
    public List<float> PKCurrentSpeeds;
    public List<Vector3> windCurrentPositions;
    public List<float> windCurrentSpeeds;
    public List<Vector3> windCurrentScales;
    public List<Quaternion> windCurrentRotations;
    public List<float> windIntensities;
    public List<int> windDirections;
    public List<Transform> windCurrentTransforms;

    public SaveObject(Header header, List<Vector3> windCurrentPositions, List<float> windCurrentSpeeds, List<Vector3> windCurrentScales,
        List<Quaternion> windCurrentRotations, List<Vector3> PKPositions, List<Quaternion> PKRotations, List<Vector3> rightControllerPositions,
        List<Vector3> leftControllerPositions, List<Quaternion> headRotations, List<float> currentSpeeds, List<float> windIntensities, List<int> windDirections)
    {
        this.windCurrentPositions = windCurrentPositions;
        this.windCurrentSpeeds = windCurrentSpeeds;
        this.windCurrentScales = windCurrentScales;
        this.windCurrentRotations = windCurrentRotations;
        this.PKPositions = PKPositions;
        this.PKRotations = PKRotations;
        this.rightControllerPositions = rightControllerPositions;
        this.leftControllerPositions = leftControllerPositions;
        this.headRotations = headRotations;
        this.PKCurrentSpeeds = currentSpeeds;
        this.windIntensities = windIntensities;
        this.windDirections = windDirections;
        this.header = header;
    }

    /// <summary>
    /// Generates string format of SaveObject object
    /// </summary>
    /// <returns>
    /// string of the object
    /// </returns>
    public override string ToString()
    {
        var output = header.ToString();

        output += "Wind currents:";
        for (int i = 0; i < windCurrentPositions.Count; i++)
        {
            output += $"\n Position: {windCurrentPositions[i]} | Speed: {windCurrentSpeeds[i]} | Scale: {windCurrentScales[i]} | Rotation: {windCurrentRotations[i]}";
        }
        output += "\n Dynamic data:";
        for (int i = 0; i < PKPositions.Count; i++)
        {
            var line = $"\n {i} | Position: {PKPositions[i]} | Rotation: {PKRotations[i]}" +
                $" | Right controller position: {rightControllerPositions[i]} | Left controller position: {leftControllerPositions[i]}" +
                $" | Head rotation: {headRotations[i]} | Wind intensity: {windIntensities[i]} | Wind direction: {windDirections[i]}";

            output += line;
        }

        return output;
    }
}

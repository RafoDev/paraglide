using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuidelineManager : MonoBehaviour
{
    public GameObject environment;
    public GameObject windCurrentVisualiserPrefab;

    public Vector3[] guideTrajectoryPositions;
    public GameObject[] middleLanePositions;
    public GameObject[] leftLanePositions;
    public GameObject[] rightLanePositions;
    public GameObject[] electricityPoles;

    public Material defaultElectricityPolesMaterial;
    public Material guidelineElectricityPolesMaterial;

    public Material defaultCableMaterial;
    public Material guidelineCableMaterial;

    public LineRenderer guidelinesRenderer;
    public LineRenderer rightLaneRenderer;
    public LineRenderer middleLaneRenderer;
    public LineRenderer leftLaneRenderer;

    private List<GameObject> windCurrentVisualisations;

    // Start is called before the first frame update
    /// <summary>
    /// Start method sets the materials and creates instances of guide visual aid if showGuidelines is true
    /// If showGuidelines is false, sets the material for electricity poles as defaultCableMaterial
    /// Calls RenderCables() method to render cables
    /// </summary>
    void Start()
    {
        windCurrentVisualisations = new List<GameObject>();

        if (Globals.showGuidelines)
        {
            // Set the obstacle material to electric poles
            foreach (GameObject obj in electricityPoles)
            {
                obj.GetComponent<Renderer>().material = guidelineElectricityPolesMaterial;
            }

            // Creates instances of visualisations of wind currents
            foreach (Transform w in environment.transform)
            {
                if (w != environment.transform)
                {
                    var instance = GameObject.Instantiate(windCurrentVisualiserPrefab, w.position, w.rotation);
                    instance.transform.localScale = w.localScale;
                    windCurrentVisualisations.Add(instance);
                    if (w.GetComponent<AirFlow>().force < 0)
                        instance.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("_WindDirection"), 1f);
                }

            }


            // Draws guide trajectory line
            TrajectoryDrawer.DrawLineTrajectory(guidelinesRenderer, guideTrajectoryPositions);

        } else
        {
            // Set the default pole material to electric poles
            foreach (GameObject obj in electricityPoles)
            {
                //obj.GetComponent<Renderer>().material = defaultElectricityPolesMaterial;
            }
        }

        // Renders cables
        RenederCables(Globals.showGuidelines);

    }

    /// <summary>
    /// This method takes positions of the cables and renderer which will render them and calls DrawLineTrajectory() method
    /// </summary>
    /// <param name="positions">
    /// Array of Gamebojects whose positions are added to cablePositions variable and rendered
    /// </param>
    /// <param name="renderer">
    /// Renderer to draw the line
    /// </param>
    public static void RenderCables(GameObject[] positions, LineRenderer renderer)
    {
        var cablePositions = new List<Vector3>();
        foreach (GameObject pos in positions)
        {
            cablePositions.Add(pos.transform.position);
        }
        TrajectoryDrawer.DrawLineTrajectory(renderer, cablePositions.ToArray());
    }

    /// <summary>
    /// This is the default method for rendering cables
    /// It uses DrawLineTrajectory method to render cables
    /// It uses the positions of the cables in lists middleCablePositions, leftCablePositions and rightCablePositions
    /// It uses the renderers middleLaneRenderer, leftLaneRenderer, rightLaneRenderer
    /// It sets the material of cables based on parametere guidelinesOn
    /// </summary>
    /// <param name="guidelinesOn">
    /// This parameter is used to decide the material of the cables
    /// If true, cables will be rendered with guidelineCableMaterial
    /// If false, cables will be rendered with defaultCableMaterial
    /// </param>

    public void RenederCables(bool guidelinesOn)
    {
        var middleCablePositions = new List<Vector3>();
        var leftCablePositions = new List<Vector3>();
        var rightCablePositions = new List<Vector3>();

        for (int i = 0; i < middleLanePositions.Length; i++)
        {
//            middleCablePositions.Add(middleLanePositions[i].transform.position);
//            leftCablePositions.Add(leftLanePositions[i].transform.position);
//            rightCablePositions.Add(rightLanePositions[i].transform.position);
        }

        if (guidelinesOn)
        {
            middleLaneRenderer.material = guidelineCableMaterial;
            leftLaneRenderer.material = guidelineCableMaterial;
            rightLaneRenderer.material = guidelineCableMaterial;
        } else
        {
            middleLaneRenderer.material = defaultCableMaterial;
            leftLaneRenderer.material = defaultCableMaterial;
            rightLaneRenderer.material = defaultCableMaterial;
        }

        TrajectoryDrawer.DrawLineTrajectory(middleLaneRenderer, middleCablePositions.ToArray());
        TrajectoryDrawer.DrawLineTrajectory(rightLaneRenderer, leftCablePositions.ToArray());
        TrajectoryDrawer.DrawLineTrajectory(leftLaneRenderer, rightCablePositions.ToArray());
    }

    /// <summary>
    /// Sets the visual currents inactive if wind currents are disabled
    /// </summary>
    public void EliminateVisualCurrents()
    {
        foreach (GameObject w in windCurrentVisualisations)
        {
            w.SetActive(false);
        }
    }

}

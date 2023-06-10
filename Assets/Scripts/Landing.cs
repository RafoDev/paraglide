using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landing : MonoBehaviour
{
    [SerializeField]
    private Collider pilot;
    [SerializeField]
    private InGameMenu menu;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Ends the flight after pilot collides with a terrain and shows evaluation screen
    /// </summary>
    /// <param name="other"> Collider of an other object </param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Me choqueeee");
        if (other.CompareTag("Good Landing"))
        {
            menu.ShowLandingEvalution(0);
            EventManager.manager.FlightEnded();
        } 
        else if(other.CompareTag("Ok Landing")) 
        {
            menu.ShowLandingEvalution(1);
            EventManager.manager.FlightEnded();
        }
        else if(other.CompareTag("Bad Landing"))
        {
            menu.ShowLandingEvalution(2);
            EventManager.manager.FlightEnded();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Me choqueeee");

        if (collision.gameObject.CompareTag("Bad Landing"))
        {
            menu.ShowLandingEvalution(2);
            EventManager.manager.FlightEnded();
        }
    }
}

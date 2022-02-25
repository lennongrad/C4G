using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleController : MonoBehaviour
{
    /// <summary>
    /// The scriptable object that manages the players available resources
    /// </summary>
    public PlayerResourceManager playerResourceManager;

    /// <summary>
    /// Event that is invoked at the beginning of a cycle
    /// </summary>
    public GameEvent cycleBegin;
    /// <summary>
    /// Event that is invoked at the end of a cycle
    /// </summary>
    public GameEvent cycleEnd;

    /// <summary>
    /// How many game ticks between the beginning and end of each cycle
    /// </summary>
    public int cycleDuration = 300;
    int cycleTimer;

    public GameObject cycleProgressDisplay;

    void Start()
    {
        cycleTimer = cycleDuration;
        cycleProgressDisplay.GetComponent<ProgressBarController>().Maximum = cycleDuration;
    }

    void FixedUpdate()
    {
        cycleTimer -= 1;
        if (cycleTimer < 0)
        {
            cycleTimer = cycleDuration;
            
            // end cycle 
            cycleEnd.Raise();
            playerResourceManager.Reset();

            // begin cycle
            cycleBegin.Raise();
        }
        cycleProgressDisplay.GetComponent<ProgressBarController>().Amount = cycleDuration - cycleTimer;
    }
}
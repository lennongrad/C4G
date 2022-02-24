using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class for behaviours that can be attached to towers to handle individual elements.
/// May not be the best structure for this but seems fine for now.
/// </summary>
abstract public class TowerBehaviour : MonoBehaviour
{
    /// <summary>
    /// Whether this behaviour should be active
    /// </summary>
    public bool performBehaviour = false;

    /// <summary>
    /// Called every FixedUpdate() if the tower is enabled
    /// </summary>
    protected abstract void Behave();

    /// <summary>
    /// Call on Start()
    /// </summary>
    protected abstract void Initiate();

    /// <summary>
    /// The main TowerController script of the tower this is attached to
    /// </summary>
    protected TowerController mainController;

    void Start()
    {
        mainController = GetComponent<TowerController>();
        Initiate();
    }

    void FixedUpdate()
    {
        if (performBehaviour && mainController.PerformBehaviours)
            Behave();
    }
}

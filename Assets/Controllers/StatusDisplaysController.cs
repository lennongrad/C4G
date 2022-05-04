using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDisplaysController : MonoBehaviour
{
    public ParticleSystem fireParticles;
    public ParticleSystem iceParticles;
    public GameObject arrowDisplay;

    public EnemyController enemy;
    public TowerController tower;

    void FixedUpdate()
    {

        if (HasStatus(Card.Status.Burn))
        {
            if (fireParticles != null && !fireParticles.isEmitting)
                fireParticles.Play();
        }
        else
        {
            if (fireParticles != null)
                fireParticles.Stop(true);
        }

        if (HasStatus(Card.Status.Frozen))
        {
            if (iceParticles != null && !iceParticles.isEmitting)
                iceParticles.Play();
        }
        else
        {
            if (iceParticles != null)
            {
                iceParticles.Stop(true);
            }
        }

        arrowDisplay.SetActive(IsHovered());
    }

    bool HasStatus(Card.Status status)
    {
        if (tower != null && tower.HasStatus(status))
            return true;
        if (enemy != null && enemy.HasStatus(status))
            return true;
        return false;
    }

    bool IsHovered()
    {
        if (tower != null && (tower.IsHovered || tower.IsPreview))
            return true;
        if (enemy != null && enemy.IsHovered)
            return true;
        return false;
    }
}

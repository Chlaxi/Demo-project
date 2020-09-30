using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool isActive;
    [SerializeField] private Light2D lantern;
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isActive)
        {
            Debug.Log("Checkpoint reached");
            OnActivate();
        }
    }

    public Transform GetRespawnPoint()
    {
        return respawnPoint;
    }

    private void OnActivate()
    {
        
        isActive = true;
        GameManager.instance.SetCheckpoint(this);
        lantern.intensity = 1;
        //Turn on the lamp
        //Turn on particles
    }

    public void Deactivate()
    {
        isActive = false;
        lantern.intensity = 0;
        //Turn off lamp
        //Turn off particles
    }
}

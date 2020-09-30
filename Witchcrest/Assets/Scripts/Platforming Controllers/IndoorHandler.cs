using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IndoorHandler : MonoBehaviour
{
    public Tilemap tilemap;

    private Color outsideColour = new Color(255, 255, 255, 1);
    private Color insideColour = new Color(0, 0, 0, 0.1f);
    [SerializeField] private Color desiredColour;

    private void Start()
    {
        desiredColour = outsideColour;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Make sure it's only the circle collider from the player that is leaving, since the box vanishes when crouching, making it Exit
        if (other.tag == "Player" && other.GetType() == typeof(CircleCollider2D))
            desiredColour = outsideColour; 
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        //To balance out the Exit
        if (other.tag == "Player" && other.GetType() == typeof(CircleCollider2D))
            desiredColour = insideColour;
    }

    private void Update()
    {
        
        tilemap.color = Color.Lerp(tilemap.color, desiredColour, 0.2f);
    }
}

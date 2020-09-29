using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private int gemGoal;
    [SerializeField] private GameObject endGameMessage;

    private void Start()
    {
        endGameMessage.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player" && collision.GetType() == typeof(CircleCollider2D))
        {
            if(GameManager.instance.GetGems() >= gemGoal)
            {
                endGameMessage.SetActive(true);
            }
            else
            {
                GameManager.instance.ShowDialogue("You need "+ gemGoal +" gems to enter!");
            }
        }
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    private Checkpoint currentCheckpoint;
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private HealthSO playerHealth;
    public HealthbarHandler healthbar;

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        if(currentCheckpoint != null)
            currentCheckpoint.Deactivate();
        
        currentCheckpoint = checkpoint;
    }

    public Checkpoint GetCheckpoint()
    {
        return currentCheckpoint;
    }

    /// <summary>
    /// Respawns the player at the last checkpoint with full health
    /// </summary>
    public void Respawn()
    {
        player.Teleport(currentCheckpoint.GetRespawnPoint());
        playerHealth.ResetHealth();
        //Lives --?
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private int gems;
    [SerializeField] private Text gemCount;
    [SerializeField] private Dialogue dialogue;

    public void Start()
    {
        UpdateGemCount();
        playerHealth.ResetHealth();
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        if(currentCheckpoint != null)
            currentCheckpoint.Deactivate();
        
        currentCheckpoint = checkpoint;
        ShowDialogue("Checkpoint Reached!");
    }

    public Checkpoint GetCheckpoint()
    {
        return currentCheckpoint;
    }

    public void AddGems(int gems)
    {
        this.gems += gems;
        UpdateGemCount();
    }

    public int GetGems()
    {
        return gems;
    }

    private void UpdateGemCount()
    {
        gemCount.text = GetGems().ToString();
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

    public void ShowDialogue(string message)
    {
        dialogue.ShowMessage(message);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}

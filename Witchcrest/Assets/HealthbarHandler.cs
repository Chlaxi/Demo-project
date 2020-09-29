using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarHandler : MonoBehaviour
{
    [SerializeField] private HealthSO playerHealth;
    [SerializeField] private GameObject HealthSlotPrefab;
    [SerializeField] private List<Image> healthbarList = new List<Image>();
    [SerializeField]
    private Sprite activeHealthslot;
    [SerializeField] private Sprite inactiveHealthSlot;

    private void Start()
    {
        UpdateMaxHealth();
    }

    public void UpdateMaxHealth()
    {
        if (playerHealth.GetMaxHealth() == healthbarList.Count)
        {
            return;
        }

        //We have too many healthslots - Remove them!
        if (healthbarList.Count > playerHealth.GetMaxHealth())
        {
                Destroy(healthbarList[healthbarList.Count-1].gameObject);
                healthbarList.RemoveAt(healthbarList.Count-1);
            
        }
        else //We have too few healthslots - Add some more
        {
            int remainder = playerHealth.GetMaxHealth() - healthbarList.Count;
            for (int i = 0; i < remainder; i++)
            {
                healthbarList.Add(Instantiate(HealthSlotPrefab, transform).GetComponent<Image>());
            }
        }
    }

    private void Update()
    {
        if(playerHealth.GetMaxHealth() != healthbarList.Count)
        {
            Debug.Log("The amount of slots doesn't fit with the health");
            UpdateMaxHealth();
        }

        
        for (int i = 0; i < healthbarList.Count; i++)
        {
            if (healthbarList[i] == null)
                break;

            if (i < playerHealth.GetHealth())
                healthbarList[i].sprite = activeHealthslot;
            else
                healthbarList[i].sprite = inactiveHealthSlot;
        }

    }
}

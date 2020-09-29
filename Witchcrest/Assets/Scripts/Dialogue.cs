using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private Text textBox;
    [SerializeField] private Animator animator;

    public void ShowMessage(string message)
    {
        textBox.text = message;
        animator.SetTrigger("Show");
    }    
}

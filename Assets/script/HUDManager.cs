using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;


    public GameObject DialogueHolder, ContinueButton;
    public TextMeshProUGUI NameDisplay, TextDisplay;


    private void Awake()
    {
    instance = this;
    }
}
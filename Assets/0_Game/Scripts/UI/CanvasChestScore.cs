using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasChestScore : MonoBehaviour
{
    public Text txtScore;

    public void OnEnable()
    {
        txtScore.text = "1.0";
    }
}

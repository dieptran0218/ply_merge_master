using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{
    private Text txtLoading;

    private void Awake()
    {
        txtLoading = GetComponent<Text>();
        StartCoroutine(ie_ShowText(0.4f));
    }

    IEnumerator ie_ShowText(float timeDelay)
    {
        txtLoading.text = "Loading.";
        while(true)
        {
            txtLoading.text = "Loading.";
            yield return Yielders.Get(timeDelay);
            txtLoading.text = "Loading..";
            yield return Yielders.Get(timeDelay);
            txtLoading.text = "Loading...";
            yield return Yielders.Get(timeDelay);
        }
    }
}

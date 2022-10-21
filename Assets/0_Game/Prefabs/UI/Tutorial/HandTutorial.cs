using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandTutorial : SingletonMonoBehaviour<HandTutorial>
{
    public Image hand;
    public List<Sprite> listSpr;

    private bool isPause;
    private void OnEnable()
    {
        isPause = false;
        StartCoroutine(ie_Anim());
    }

    IEnumerator ie_Anim()
    {
        var id = 0;
        while(true)
        {
            if(!isPause) hand.sprite = listSpr[id++ % listSpr.Count];
            yield return Yielders.Get(0.1f);
        }
    }

    public void OnPause(bool b)
    {
        isPause = b;
    }
}

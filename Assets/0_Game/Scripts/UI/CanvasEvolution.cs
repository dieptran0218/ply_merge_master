using UnityEngine;
using UnityEngine.UI;

public class CanvasEvolution : MonoBehaviour
{
    public Image imgFill;
    public int EvoMax = 30;

    private float fill;
    private int maxEvoLevel;
    private int curEvoLevel;

    public void OnOpen()
    {
        maxEvoLevel = 1;
        curEvoLevel = 1;

        imgFill.fillAmount = 0;
        gameObject.SetActive(true);
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.ins != null && GameManager.ins.isGamePlaying)
        {
            fill = (float)PlayerController.ins.evolutionCollected / EvoMax;
            if (fill < 0) fill = 0;
            if (fill > 1) fill = 1;
            imgFill.fillAmount = fill;

            if (fill < 0.25f)
            {
                curEvoLevel = 1;
            }
            else if (fill < 0.5f)
            {
                curEvoLevel = 2;
            }
            else if (fill < 0.75f)
            {
                curEvoLevel = 3;
            }
            else
            {
                curEvoLevel = 3;
            }

            if(curEvoLevel != maxEvoLevel)
            {
                PlayerController.ins.LevelUp(curEvoLevel - maxEvoLevel);
                maxEvoLevel = curEvoLevel;
            }
        }
    }
}

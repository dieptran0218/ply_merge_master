using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheat : MonoBehaviour
{
    public void OnNextLv() {
        GameManager.ins.data.level++;
        GameManager.ins.ReLoadGame();
    }

    public void OnPrevLv()
    {
        if(GameManager.ins.data.level > 0)
            GameManager.ins.data.level--;
        GameManager.ins.ReLoadGame();
    }

    public int skinId = 0;
    public void NextChar()
    {
        skinId++;
        GameManager.ins.data.charUsed = (skinId % 11 + 1).ToEnum<CharacterType>();
        PlayerController.ins.LoadCharacter();
    }

    public void PreChar()
    {
        skinId--;
        skinId += 11;
        GameManager.ins.data.charUsed = (skinId % 11 + 1).ToEnum<CharacterType>();
        PlayerController.ins.LoadCharacter();
    }
}

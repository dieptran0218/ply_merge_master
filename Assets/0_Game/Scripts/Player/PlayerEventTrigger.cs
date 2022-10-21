using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventTrigger : MonoBehaviour
{
    public void Event_FootStep()
    {
        SoundController.PlaySoundOneShot(SoundController.ins.footstep);
    }
}

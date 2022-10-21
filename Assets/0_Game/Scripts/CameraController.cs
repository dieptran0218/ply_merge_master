using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonMonoBehaviour<CameraController>
{
    public Animation animHome;
    public Camera Cam;

    public CameraPos posHome;
    public CameraPos posShop;
    public CameraPos posGamePlay;
    public CameraPos posEndGame;
    public CameraPos posEndGame_AttackBoss_0;
    public CameraPos posEndGame_AttackBoss;
    public CameraPos posEndGame_AttackBoss_2;
    public CameraPos posEndGame_AttackBoss_3;
    public CameraPos posEndGame_Pre_Boss_Piuu;
    public CameraPos posEndGame_CameraChest;

    public CameraPos posEndGame_3_Step_1;
    public CameraPos posEndGame_3_Step_2;
    public CameraPos posEndGame_3_Step_3;

    public float _ratioPos;
    public float _ratioRot;
    #region old game
    public void SetCameraHome()
    {
        transform.DOLocalMove(posHome.pos, posHome.timeSwitch)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                animHome.Play();
            });
        transform.DOLocalRotate(posHome.rot, posHome.timeSwitch)
            .SetEase(Ease.Linear);

        Cam.DOFieldOfView(52, posEndGame.timeSwitch);
    }

    public void SetCameraShop()
    {
        animHome.Stop();
        PlayerController.ins.shopLight.SetActive(true);
        RotateInPlace(PlayerController.ins.trans, Vector3.up * -180);
        transform.DOLocalMoveY(1.52f, posShop.timeSwitch)
            .SetEase(Ease.Linear);
        Cam.DOFieldOfView(40, posShop.timeSwitch);
    }

    public void EndCameraShop()
    {
        animHome.Play();
        CanvasManager.ins.canvasHome.OnOpen();

        //PlayerController.ins.shopLight.SetActive(false);
        //RotateInPlace(PlayerController.ins.trans, Vector3.up * 180);
        //transform.DOLocalMoveY(transform.localPosition.y + 0.75f, posShop.timeSwitch)
        //    .SetEase(Ease.Linear)
        //    .OnComplete(() =>
        //    {
                
        //    });

        //Cam.DOFieldOfView(52, posShop.timeSwitch);
    }

    public void RotateInPlace(Transform trans, Vector3 rot, Action c = null)
    {
        var o = new GameObject();
        o.transform.SetParent(trans.parent);
        o.transform.position = trans.position;
        o.transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.SetParent(o.transform);

        o.transform.DOLocalRotate(rot, posShop.timeSwitch)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.SetParent(trans.parent);
                c?.Invoke();
                Destroy(o);
            });
    }

    public void SetCameraGamePlay()
    {
        animHome.enabled = false;
        transform.DOLocalMove(posGamePlay.pos, posGamePlay.timeSwitch)
            .SetEase(Ease.Linear);
        transform.DOLocalRotate(posGamePlay.rot, posGamePlay.timeSwitch)
            .SetEase(Ease.Linear);
        Cam.DOFieldOfView(52, posEndGame.timeSwitch);
    }

    public void SetCameraEndgame()
    {
        transform.DOKill();
        Cam.DOKill();

        animHome.enabled = false;
        transform.DOLocalMove(posEndGame.pos, posEndGame.timeSwitch)
            .SetEase(Ease.Linear);
        transform.DOLocalRotate(posEndGame.rot, posEndGame.timeSwitch)
            .SetEase(Ease.Linear);

        Cam.DOFieldOfView(65, posEndGame.timeSwitch);
    }

    public void SetCameraEndGame_AttackBoss_0()
    {
        transform.DOKill();
        Cam.DOKill();

        animHome.enabled = false;
        transform.DOLocalMove(posEndGame_AttackBoss_0.pos, posEndGame_AttackBoss_0.timeSwitch)
            .SetEase(Ease.Linear);
        transform.DOLocalRotate(posEndGame_AttackBoss_0.rot, posEndGame_AttackBoss_0.timeSwitch)
            .SetEase(Ease.Linear);
        //var t = transFog.transform.position;
        //t.y = posEndGame_AttackBoss_0.fogPos.y;
        //transFog.transform.position = t;
        Cam.DOFieldOfView(62, posEndGame_AttackBoss_0.timeSwitch);
    }

    public void SetCameraEndGame_AttackBoss()
    {
        transform.DOKill();
        Cam.DOKill();

        animHome.enabled = false;
        transform.DOLocalMove(posEndGame_AttackBoss.pos, posEndGame_AttackBoss.timeSwitch)
            .SetEase(Ease.Linear);
        transform.DOLocalRotate(posEndGame_AttackBoss.rot, posEndGame_AttackBoss.timeSwitch)
            .SetEase(Ease.Linear);
        Cam.DOFieldOfView(62, posEndGame_AttackBoss.timeSwitch);
    }

    public void SetCameraEndGame_AttackBoss_2()
    {
        transform.DOKill();
        Cam.DOKill();

        animHome.enabled = false;
        transform.DOLocalMove(posEndGame_AttackBoss_2.pos, posEndGame_AttackBoss_2.timeSwitch)
            .SetEase(Ease.Linear);
        transform.DOLocalRotate(posEndGame_AttackBoss_2.rot, posEndGame_AttackBoss_2.timeSwitch)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                SetCameraEndGame_AttackBoss_3();
            });
        //transFog.gameObject.SetActive(false);
        //var t = transFog.localPosition;
        //t.y = 0f;
        //transFog.localPosition = t;
        Cam.DOFieldOfView(32, 0.2f);
    }

    public void SetCameraEndGame_AttackBoss_3()
    {
        transform.DOKill();
        Cam.DOKill();

        animHome.enabled = false;
        transform.DOLocalMove(posEndGame_AttackBoss_3.pos, posEndGame_AttackBoss_3.timeSwitch)
            .SetEase(Ease.Linear);
        transform.DOLocalRotate(posEndGame_AttackBoss_3.rot, posEndGame_AttackBoss_3.timeSwitch)
            .SetEase(Ease.Linear);
        //var t = transFog.localPosition;
        //t.y = 0f;
        //transFog.localPosition = t;
        Cam.DOFieldOfView(32, posEndGame_AttackBoss_3.timeSwitch);
    }

    public void SetCameraEndGame_Pre_Boss_Piuu()
    {
        transform.DOKill();
        Cam.DOKill();

        animHome.enabled = false;
        transform.DOLocalMove(posEndGame_Pre_Boss_Piuu.pos, posEndGame_Pre_Boss_Piuu.timeSwitch)
            .SetEase(Ease.Linear);
        transform.DOLocalRotate(posEndGame_Pre_Boss_Piuu.rot, posEndGame_Pre_Boss_Piuu.timeSwitch)
            .SetEase(Ease.Linear);
        Cam.DOFieldOfView(32, posEndGame_Pre_Boss_Piuu.timeSwitch);
    }

    public void SetCameraEndGame_CameraChest()
    {
        transform.DOKill();
        Cam.DOKill();

        animHome.enabled = false;
        transform.DOLocalMove(posEndGame_CameraChest.pos, posEndGame_CameraChest.timeSwitch)
            .SetEase(Ease.Linear);
        transform.DOLocalRotate(posEndGame_CameraChest.rot, posEndGame_CameraChest.timeSwitch)
            .SetEase(Ease.Linear);
        Cam.DOFieldOfView(70, posEndGame_CameraChest.timeSwitch);
    }
    #endregion

    #region endgame 3
    public void SetCameraEndGame_3_Step_1()
    {
        transform.DOKill();
        Cam.DOKill();

        var cam = posEndGame_3_Step_1;
        var fieldOfView = 70;

        SetCam(cam, fieldOfView);
    }

    public void SetCameraEndGame_3_Step_2()
    {
        transform.DOKill();
        Cam.DOKill();

        var cam = posEndGame_3_Step_2;
        var fieldOfView = 83;

        SetCam(cam, fieldOfView, OnUpdate);
    }

    public void SetCameraEndGame_3_Step_3()
    {
        transform.DOKill();
        Cam.DOKill();
        StopAllCoroutines();

        var cam = posEndGame_3_Step_3;
        var fieldOfView = 83;

        SetCam(cam, fieldOfView);
    }

    private Vector3 originCamPos;
    private Vector3 firstCenter;
    private float clampX = 3.5f;
    private float clampZ = 3f;
    private float camSpeed;

    private void SetFightBossCamPos()
    {
        if (PlayerController.ins.listPokemon.Count == 0) return;

        var bounds = new Bounds(PlayerController.ins.listPokemon[0].transform.position, Vector3.zero);
        for (int i = 0; i < PlayerController.ins.listPokemon.Count; i++)
        {
            bounds.Encapsulate(PlayerController.ins.listPokemon[i].transform.position);
        }
        for (int i = 0; i < PlayerController.ins._endgame._lstPokemonBoss.Count; i++)
        {
            bounds.Encapsulate(PlayerController.ins._endgame._lstPokemonBoss[i].transform.position);
        }

        Vector3 centerPoint = bounds.center;
        Vector3 offset = centerPoint - firstCenter;
        float xPos = Mathf.Clamp(originCamPos.x + offset.x, originCamPos.x - clampX, originCamPos.x + clampX);
        float zPos = Mathf.Clamp(originCamPos.z + offset.z, originCamPos.z - clampZ, originCamPos.z + clampZ);
        Vector3 newPos = new Vector3(xPos, transform.position.y, zPos);

        //transform.position = newPos;
        float maxSpeed = 0.125f;
        if (camSpeed < maxSpeed)
        {
            camSpeed += Time.deltaTime * 0.1f;
        }

        transform.position = Vector3.Lerp(transform.position, newPos, camSpeed);

        if (Vector3.Distance(transform.position, newPos) < 0.05f)
        {
            camSpeed = 0;
        }
    }

    float smallestX = 1000;
    float highestX = -1000;
    float smallestY = 1000;
    float highestY = -1000;
    float smallestZ = 1000;
    float highestZ = -1000;

    private void SetFightBossFOV()
    {
        for (int i = 0; i < PlayerController.ins.listPokemon.Count; i++)
        {
            if (PlayerController.ins.listPokemon[i].transform.localPosition.x < smallestX)
            {
                smallestX = PlayerController.ins.listPokemon[i].transform.localPosition.x;
            }
            if (PlayerController.ins.listPokemon[i].transform.localPosition.x > highestX)
            {
                highestX = PlayerController.ins.listPokemon[i].transform.localPosition.x;
            }
            if (PlayerController.ins.listPokemon[i].transform.localPosition.y < smallestY)
            {
                smallestY = PlayerController.ins.listPokemon[i].transform.localPosition.y;
            }
            if (PlayerController.ins.listPokemon[i].transform.localPosition.y > highestY)
            {
                highestY = PlayerController.ins.listPokemon[i].transform.localPosition.y;
            }
            if (PlayerController.ins.listPokemon[i].transform.localPosition.z < smallestZ)
            {
                smallestZ = PlayerController.ins.listPokemon[i].transform.localPosition.z;
            }
            if (PlayerController.ins.listPokemon[i].transform.localPosition.z > highestZ)
            {
                smallestZ = PlayerController.ins.listPokemon[i].transform.localPosition.z;
            }
        }
        for (int i = 0; i < PlayerController.ins._endgame._lstPokemonBoss.Count; i++)
        {
            if (PlayerController.ins._endgame._lstPokemonBoss[i].transform.localPosition.x < smallestX)
            {
                smallestX = PlayerController.ins._endgame._lstPokemonBoss[i].transform.localPosition.x;
            }
            if (PlayerController.ins._endgame._lstPokemonBoss[i].transform.localPosition.x > highestX)
            {
                highestX = PlayerController.ins._endgame._lstPokemonBoss[i].transform.localPosition.x;
            }
            if (PlayerController.ins._endgame._lstPokemonBoss[i].transform.localPosition.y < smallestY)
            {
                smallestY = PlayerController.ins._endgame._lstPokemonBoss[i].transform.localPosition.y;
            }
            if (PlayerController.ins._endgame._lstPokemonBoss[i].transform.localPosition.y > highestY)
            {
                highestY = PlayerController.ins._endgame._lstPokemonBoss[i].transform.localPosition.y;
            }
            if (PlayerController.ins._endgame._lstPokemonBoss[i].transform.localPosition.z < smallestZ)
            {
                smallestZ = PlayerController.ins._endgame._lstPokemonBoss[i].transform.localPosition.z;
            }
            if (PlayerController.ins._endgame._lstPokemonBoss[i].transform.localPosition.z > highestZ)
            {
                smallestZ = PlayerController.ins._endgame._lstPokemonBoss[i].transform.localPosition.z;
            }
        }

        float disX = highestX - smallestX;
        float dixY = highestY - smallestY;
        float disZ = highestZ - smallestZ;

        float maxDis = Mathf.Max(disX, dixY, disZ);
        float targetFOV = CalculateFOV(maxDis);

        Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, targetFOV, 0.05f);
    }

    private float CalculateFOV(float dis)
    {
        float t = dis / 10;
        t = Mathf.Max(t, 0.2f);
        t = Mathf.Min(t, 1f);
        t = Mathf.Sqrt(t);
        return t * 13 + 70;
    }

    [ContextMenu("test cam")]
    public void OnUpdate()
    {
        originCamPos = Cam.transform.position;
        var bounds = new Bounds(PlayerController.ins.listPokemon[0].transform.position, Vector3.zero);
        for (int i = 0; i < PlayerController.ins.listPokemon.Count; i++)
        {
            bounds.Encapsulate(PlayerController.ins.listPokemon[i].transform.position);
        }
        for (int i = 0; i < PlayerController.ins._endgame._lstPokemonBoss.Count; i++)
        {
            bounds.Encapsulate(PlayerController.ins._endgame._lstPokemonBoss[i].transform.position);
        }
        firstCenter = bounds.center;

        StartCoroutine(ie_Update());
    }

    IEnumerator ie_Update()
    {
        while (true)
        {
            //Tinh trong tam
            SetFightBossCamPos();
            SetFightBossFOV();
            yield return Yielders.FixedUpdate;
        }
    }

    #endregion

    public void SetCam(CameraPos cam, int fieldOfView, Action c = null)
    {
        animHome.enabled = false;
        transform.DOLocalMove(cam.pos, cam.timeSwitch)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (c != null) c.Invoke();
            });

        transform.DOLocalRotate(cam.rot, cam.timeSwitch)
            .SetEase(Ease.Linear);

        Cam.DOFieldOfView(fieldOfView, cam.timeSwitch)
            .SetEase(Ease.Linear);

    }
}

public enum CameraStage
{
    None,
    Home,
    Shop,
    GamePlay,
    EndGame_Fight,
    EndGame_BossLost
}

[System.Serializable]
public class CameraPos
{
    public Vector3 pos;
    public Vector3 rot;
    public float timeSwitch = 1f;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class RoadItem : MonoBehaviour
{
    public List<Transform> lstPoint;
    public GameObject connectTo;
    public RoadType thisType;
    private float currentYRot;

    //[ContextMenu("create path")]
    //public void Createpath()
    //{
    //    var dtw = GetComponent<DOTweenPath>();
    //    dtw.wps.Clear();
    //    var tmp = CreatePath();
    //    foreach (var t in tmp)
    //    {
    //        dtw.wps.Add(t);
    //    }
    //}

    [Header("Create Road")]
    public RoadType chosenRoad;
    [ContextMenu("Create Road")]
    public void CreateRoad()
    {
        if (chosenRoad == RoadType.None) return;
        GameObject nextPiece = Instantiate(EnumToPrefab(chosenRoad));
        nextPiece.GetComponent<RoadItem>().SetUp(chosenRoad);
        nextPiece.transform.position = lstPoint[lstPoint.Count - 1].transform.position;
        nextPiece.transform.SetParent(transform.parent);
        //Selection.activeGameObject = nextPiece;
        float xzRot;
        float yRot = 0;

        //Set xz rotation
        if (chosenRoad == RoadType.congPhai30 || chosenRoad == RoadType.congPhai60 || chosenRoad == RoadType.congTrai30 || chosenRoad == RoadType.congTrai60)
        {
            xzRot = 0;
        }
        else
        {
            xzRot = -90;
        }

        //set y rotation
        if (thisType == RoadType.congPhai30)
        {
            yRot = 30;
        }
        else if (thisType == RoadType.congPhai60) {
            yRot = 60;
        }
        else if (thisType == RoadType.congTrai30)
        {
            yRot = -30;
        }
        else if (thisType == RoadType.congTrai60)
        {
            yRot = -60;
        }

        float targetYRot = currentYRot + yRot;
        nextPiece.GetComponent<RoadItem>().SetupRot(targetYRot);

        Debug.Log("xzRot:" + xzRot + " ==== targetRotY:" + targetYRot + " ==== yRot:" + yRot);

        nextPiece.transform.localRotation = Quaternion.Euler(xzRot, targetYRot , xzRot);
        //nextPiece.transform.eulerAngles = new Vector3(xzRot, targetYRot + yRot, xzRot);
    }

    [ContextMenu("Connect Road")]
    public void ConnectRoad() {
        if (connectTo == null) return;
        if (connectTo.GetComponent<RoadItem>().thisType == RoadType.None) return;
        RoadType thisNextType;
        thisNextType = connectTo.GetComponent<RoadItem>().thisType;
        connectTo.transform.position = lstPoint[lstPoint.Count - 1].transform.position;
        connectTo.transform.SetParent(transform.parent);
        //Selection.activeGameObject = connectTo;
        float xzRot;
        float yRot = 0;

        //Set xz rotation
        if (thisNextType == RoadType.congPhai30 || thisNextType == RoadType.congPhai60 || thisNextType == RoadType.congTrai30 || thisNextType == RoadType.congTrai60)
        {
            xzRot = 0;
        }
        else
        {
            xzRot = -90;
        }

        //set y rotation
        if (this.thisType == RoadType.congPhai30)
        {
            yRot = 30;
        }
        else if (this.thisType == RoadType.congPhai60)
        {
            yRot = 60;
        }
        else if (this.thisType == RoadType.congTrai30)
        {
            yRot = -30;
        }
        else if (this.thisType == RoadType.congTrai60)
        {
            yRot = -60;
        }

        float targetYRot = currentYRot + yRot;
        connectTo.GetComponent<RoadItem>().SetupRot(targetYRot);

        Debug.Log("xzRot:" + xzRot + " ==== targetRotY:" + targetYRot + " ==== yRot:" + yRot);

        connectTo.transform.localRotation = Quaternion.Euler(xzRot, targetYRot, xzRot);
    }
    public List<Vector3> CreatePath()
    {
        var path = new List<Vector3>();
        foreach (var t in lstPoint)
        {
            path.Add(t.position);
        }
        return path;
    }

    public void SetUp(RoadType type) {
        thisType = type;
    }
    public void SetupRot(float passYRot) {
        currentYRot = passYRot;
    }

    public GameObject EnumToPrefab(RoadType type)
    {
        if (type == RoadType.endGame)
        {
            return transform.parent.parent.GetComponent<MapSetup>().endGame;
        }
        else if (type == RoadType.roadBeo)
        {
            return transform.parent.parent.GetComponent<MapSetup>().roadBeo;
        }
        else if (type == RoadType.congPhai30)
        {
            return transform.parent.parent.GetComponent<MapSetup>().congPhai30;
        }
        else if (type == RoadType.congTrai30)
        {
            return transform.parent.parent.GetComponent<MapSetup>().congTrai30;
        }
        else if (type == RoadType.congPhai60)
        {
            return transform.parent.parent.GetComponent<MapSetup>().congPhai60;
        }
        else if (type == RoadType.congTrai60)
        {
            return transform.parent.parent.GetComponent<MapSetup>().congTrai60;
        }
        else if (type == RoadType.docLenPhai)
        {
            return transform.parent.parent.GetComponent<MapSetup>().docLenPhai;
        }
        else if (type == RoadType.docLenTrai)
        {
            return transform.parent.parent.GetComponent<MapSetup>().docLenTrai;
        }
        else if (type == RoadType.thangPhai)
        {
            return transform.parent.parent.GetComponent<MapSetup>().thangPhai;
        }
        else if (type == RoadType.thangTrai)
        {
            return transform.parent.parent.GetComponent<MapSetup>().thangTrai;
        }
        else if (type == RoadType.xuongPhai)
        {
            return transform.parent.parent.GetComponent<MapSetup>().xuongPhai;
        }
        else if (type == RoadType.xuongTrai)
        {
            return transform.parent.parent.GetComponent<MapSetup>().xuongTrai;
        }
        else if (type == RoadType.roadThang)
        {
            return transform.parent.parent.GetComponent<MapSetup>().roadThang;
        }
        else
        {
            return null;
        }
    }

    public enum RoadType
    {
        None,
        endGame,
        roadBeo,
        congPhai30,
        congTrai30,
        congPhai60,
        congTrai60,
        docLenPhai,
        docLenTrai,
        thangPhai,
        thangTrai,
        xuongPhai,
        xuongTrai,
        roadThang
    }
}

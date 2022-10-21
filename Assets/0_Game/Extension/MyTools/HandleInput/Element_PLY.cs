using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element_PLY : MonoBehaviour
{
    #region Member Variables
    [HideInInspector] public Transform mTrans;
    public bool isInteractive;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        mTrans = GetComponent<Transform>();
    }
    private void Update()
    {

    }
    #endregion

    #region Public Methods
    public virtual void Action() { }
    public virtual void Init() { }
    public virtual bool CheckInteractive()
    {
        return isInteractive;
    }
    #endregion


    public Endgame_3_Ceil Press()
    {
        Action();
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleInput : MonoBehaviour
{
    #region Inspector Variables
    public InputManager manager;
    public LayerMask mask;
    public LayerMask maskEnd;
    #endregion

    #region Member Variables
    bool isActive = false;
    #endregion

    #region Unity Methods
    private void Start()
    {
        //Debug.Log("Input start");
        manager.OnTouch += HandleTouch;
        manager.OnStartDrag += HandleStartDrag;
        manager.OnDraging += HandleDraging;
        manager.OnFinishDrag += HandleFinishDrag;
        manager.OnTap += HandleTap;
        Active();
    }

    private void OnDestroy()
    {
        //Debug.Log("Input destroy");
        manager.OnTouch -= HandleTouch;
        manager.OnStartDrag -= HandleStartDrag;
        manager.OnDraging -= HandleDraging;
        manager.OnFinishDrag -= HandleFinishDrag;
        manager.OnTap -= HandleTap;
    }
    #endregion

    #region Public Methods
    public void Active()
    {
        isActive = true;
    }

    public void Deactive()
    {
        isActive = false;
        if (drag)
        {
            drag = false;
        }
    }
    #endregion

    #region Private Methods
    /*
     * Kiểm tra ấn vào vật thể (3D) được gắn tag "Element" và layer "Element"
     */
    private Endgame_3_Ceil _objTrigger;
    RaycastHit hit = new RaycastHit();
    Ray ray;
    Camera cam;
    private void HandleTouch(Vector3 pos)
    {        
        if (!isActive)
            return;
        //Debug.Log("Touched");
        ray = Camera.main.ScreenPointToRay(pos);
        Physics.Raycast(ray, out hit, 100, mask);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Element"))
            {
                IElement element = hit.collider.gameObject.GetComponent<IElement>();
                if (element != null)
                {
                    cam = Camera.main;
                    _objTrigger = element.Press();
                }
                    
            }
        }
    }

    private bool drag = false;

    private void HandleStartDrag()
    {
        if (!isActive)
            return;
        drag = true;
        //Debug.Log("Start drag");
    }

    private void HandleDraging(Vector3 pos)
    {
        if (!isActive)
            return;

        if (_objTrigger != null)
        {
            ray = cam.ScreenPointToRay(pos);
            Physics.Raycast(ray, out hit, 100, maskEnd);
            if(hit.collider != null)
            {
                _objTrigger.MoveTo(hit.point);
            }
            else
            {
                //drop action
                _objTrigger.Drop();
                _objTrigger = null;
            }
        }
        
    }

    private void HandleFinishDrag(Vector3 pos)
    {
        if (!isActive)
            return;
        if (drag)
        {
            //Debug.Log("Finish drag");
            drag = false;
            if (_objTrigger != null)
            {
                _objTrigger.Drop();
                _objTrigger = null;
            }
        }

    }

    private void HandleTap(Vector3 pos)
    {

    }


    #endregion
}

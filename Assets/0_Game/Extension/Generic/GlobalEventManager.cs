using UnityEngine;

public class GlobalEventManager : SingletonMonoBehaviour<GlobalEventManager>
{
    #region Events
    public ActionEvent.TwoParam GlobalEvent;
    public ActionEvent.NoParam UpdateProperties;
    #endregion

    #region Inspector Variables
    #endregion

    #region Methods
    public void OnSessionStart()
    {
        GlobalEvent?.Invoke(name, null);
    }

    public void OnSessionEnd()
    {
        GlobalEvent?.Invoke(name, null);
    }

    public void OnUpdateProperties()
    {
        UpdateProperties?.Invoke();
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        OnSessionStart();
    }
    private void OnDestroy()
    {
        OnUpdateProperties();
        OnSessionEnd();
        GlobalEvent = null;
    }
    #endregion
}

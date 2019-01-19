using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    #region Events
    public delegate void Demagnetised();
    public static Demagnetised OnDemagnetised;
    #endregion

    #region Methods
    public static void FireDemagnetisedEvent()
    {
        if (OnDemagnetised != null)
            OnDemagnetised();
    }
    #endregion
}

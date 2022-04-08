using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
public class UGSAnalytics : MonoBehaviour
{
    

    public void RecordEvent(string name, Dictionary<string, object> parameters)
    {
        if (parameters == null )
        {
            parameters = new Dictionary<string, object>();
        }

        Events.CustomData(name, parameters);
        Debug.Log($"Recorded Event {name}"); 
    }

}

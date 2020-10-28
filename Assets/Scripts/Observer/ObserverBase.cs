using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ObserverBase
{
    void OnNotify(ObserverEvent oe_event);
}

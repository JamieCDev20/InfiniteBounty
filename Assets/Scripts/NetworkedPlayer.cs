using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPlayer : MonoBehaviour
{
    private int i_playerID;
    public int PlayerID{ get{ return i_playerID; } set { i_playerID = value; } }
}

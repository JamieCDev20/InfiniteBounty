using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopdogMover : MoverBase
{
    
    /*
     *  f_moveSpeed
     *  v_drag
     *  b_canMove
     *  hit
     *  v_groundNormal
     *  rb
     */

    public void Launch()
    {
        b_canMove = false;
        Invoke(nameof(SetCanMove), 2);

    }

}

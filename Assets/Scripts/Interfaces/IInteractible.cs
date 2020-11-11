using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractible
{

    void Interacted();

    void Interacted(Transform interactor);

}

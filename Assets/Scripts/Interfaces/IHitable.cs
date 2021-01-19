using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitable
{

    void TakeDamage(int damage, bool activatesThunder);

    bool IsDead();

    void Die();

    void TakeDamage(int damage, bool activatesThunder, float delay);

}

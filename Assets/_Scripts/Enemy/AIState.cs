using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : ScriptableObject
{
    public virtual void Movement(EnemyMovement ai)
    {
        Debug.Log("Not overriden yet");
    }

}

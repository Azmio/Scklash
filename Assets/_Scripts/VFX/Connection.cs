using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    LineRenderer line;

    private void Awake()
    {
        line = this.GetComponent<LineRenderer>();
    }


    public void MakeConnection(Vector3 startPos, Vector3 endPos)
    {
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
    }
}


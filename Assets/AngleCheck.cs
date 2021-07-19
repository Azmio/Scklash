using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleCheck : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 lookDir;
    Vector3 otherPointDir1;
    Vector3 otherPointDir2;

    void Start()
    {
        // transform.LookAt(GameController.instance.Player.transform.position);

        
    }


    // Update is called once per frame
    void Update()
    {
        lookDir = GameController.instance.Player.transform.position - this.transform.position;
        lookDir.y = 0;
        this.transform.rotation = Quaternion.LookRotation(lookDir);
        //this.transform.rotation = GameController.instance.Player.transform.rotation;
        Debug.DrawRay(this.transform.position, this.transform.forward*5);
        otherPointDir1= Quaternion.AngleAxis(30, Vector3.up)* this.transform.forward;
        Debug.DrawRay(this.transform.position, otherPointDir1);
        otherPointDir2 = Quaternion.AngleAxis(-30, Vector3.up) * this.transform.forward;
        Debug.DrawRay(this.transform.position, otherPointDir2);
    }

}

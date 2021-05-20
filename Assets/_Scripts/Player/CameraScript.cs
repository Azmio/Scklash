using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform placeHolder; //Parent Gameobject

    public float camDistance = 15f;

    void Update()
    {
        placeHolder.transform.position = PlayerMovement.playerTransform.position;//Follow player without being attached as child

        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public Camera mCamera;
    // Start is called before the first frame update
    void Start()
    {
        mCamera= Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = mCamera.transform.rotation;
    }
}

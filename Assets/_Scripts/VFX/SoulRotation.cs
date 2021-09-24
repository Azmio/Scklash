using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulRotation : MonoBehaviour
{
    public Vector3 rotationDirection;
    public float smoothTime;
    private float convertedTime = 200;
    private float smooth;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        smooth = Time.deltaTime * smoothTime * convertedTime;
        transform.Rotate(rotationDirection * smooth);
    }

    IEnumerator RandomDelay()
    {
        float random = Random.Range(0.1f, 0.9f);

        yield return new WaitForSeconds(random);
    }
}

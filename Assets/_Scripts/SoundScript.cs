using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour
{
    void footstep()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/footstep",  transform.position);
    }
    void swoosh()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/swoosh",  transform.position); 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    public EnemyAI parentObject;

    public GameObject target;
    //wire mesh and material
    public GameObject wireRef;
    Material growMaterial;

    public ParticleSystem succParticles;

    public float minGrowValue=0f;
    public float maxGrowValue=1f;
    public float timeToGrow=1f;
    public float refreshRate = 0.01f;


    public Connection connection;
    public GameObject connectionPrefab;

    public Transform offset;

    public float timeRequiredToAbsorb=4;

    //If true then it succesfully absorbed the soul, otherwise it was used by the player or destroyed by other means
    public bool doneAbsorbing = false;
    public bool isBusy = true;


    public void Start()
    {
        growMaterial = wireRef.GetComponent<MeshRenderer>().material;
        growMaterial.SetFloat("Grow_", minGrowValue);
        succParticles.Stop();
        isBusy = true;
        StartCoroutine(WireAndSucc());

    }
   
    public IEnumerator WireAndSucc()
    {
        //grow the wire
        float growValue = growMaterial.GetFloat("Grow_");
        while ( growValue< maxGrowValue)
        {
            growValue += 1 / (timeToGrow / refreshRate);
            growMaterial.SetFloat("Grow_", growValue);

            yield return new WaitForSeconds(refreshRate);
        }

        //create connection
        float startTime = Time.time;
        float endTime = startTime + timeRequiredToAbsorb;
        succParticles.Play();
        while (startTime < endTime)
        {
            if (target == null||!parentObject.isActive)
            {
                doneAbsorbing = false;
                break;
            }
            CreateConnection();
            UpdateLinkAndDirection(target.transform.position);
            startTime += refreshRate;
            doneAbsorbing = true;
            yield return new WaitForSeconds(refreshRate);
        }
        Destroy(connection.gameObject);
        succParticles.Stop();
        if(doneAbsorbing)
        {
            
            target.GetComponent<EnemyAI>().DestroyUtility();
        }    
            


        //ungrow the wire :p
        yield return new WaitForSeconds(0.1f);
        growValue= growMaterial.GetFloat("Grow_");
        while (growValue >= minGrowValue)
        {
            growValue -= 1 / (timeToGrow / refreshRate);
            growMaterial.SetFloat("Grow_", growValue);

            yield return new WaitForSeconds(refreshRate);
        }

        isBusy = false;

    }

    public void UpdateLinkAndDirection(Vector3 target)
    {
        //using this method so that if we need to end the connection when the distance increases we can do it ez
        Vector3 lookPos = target - transform.position;
        lookPos.y = 0;

        //Look direction take's the player's position in order to change the rotation of the enemies to make it seem more natural and to get more control over the rotation
        Quaternion lookDirection;
        lookDirection = Quaternion.LookRotation(lookPos);
        Debug.DrawRay(transform.position, transform.forward * 4, Color.red);
        // Spherical lerp to get a smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, Time.deltaTime * 10);

    }

    public void CreateConnection()
    {
        if (connection == null)
        {
            Debug.Log("Connection created");
            connection = Instantiate(connectionPrefab).GetComponent<Connection>();
        }

        connection.MakeConnection(offset.position, target.transform.position);
    }

    public void UpdateConnection()
    {
        connection.MakeConnection(offset.position, target.transform.position);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class EnemyHealthBar : MonoBehaviour
{
    public Camera mCamera;
    public HealthScript healthScript; // instance of enemy healthScript for the health value
    public GameObject healthBar;    // needed in order to adjust the rotation of health bar in order to display it properly
    public Image healthBarColor; // the actual health bar image used to adjust the fill setting
    // Start is called before the first frame update
    void Start()
    {
        mCamera= Camera.main;
        healthScript = this.gameObject.GetComponent<HealthScript>();
        healthBar = this.transform.Find("Canvas").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
    }
    void UpdateHealthBar()
    {
        healthBar.transform.rotation = mCamera.transform.rotation;
        healthBarColor.fillAmount = (healthScript.currentHealth / 100f);
        healthBarColor.color = Color.Lerp(Color.red, Color.green, healthScript.currentHealth / 100f);
    }
}

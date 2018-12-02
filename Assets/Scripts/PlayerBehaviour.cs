using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerBehaviour : MonoBehaviour
{
    public string curPassword = "12345";
    public string input;
    public bool onTrigger;
    public bool keypadScreen;
    public GameObject door;
    public bool doorOpen;

    [Header("Health Settings")]
    public GameObject healthSlider;
    public float health = 100;
    public float healthMax = 100;
    public float healValue = 5;
    public float secondToHeal = 10;
    private bool inEnemyArea = false;

    [Header("Flashlight Battery Settings")]
    public GameObject Flashlight;
    public GameObject batterySlider;
    public float battery = 100;
    public float batteryMax = 100;
    public float removeBatteryValue = 0.05f;
    public float secondToRemoveBaterry = 5f;

    [Header("Audio Settings")]
    public AudioClip Noise;
    public AudioClip scarecrowNoise;


    [Header("UI Settings")]
    public GameObject inGameMenuUI;
    public GameObject pickUpUI;
    public GameObject finishedGameUI;
    public bool paused;

    void Start()
    {
        // set initial health values
        health = healthMax;
        battery = batteryMax;

        healthSlider.GetComponent<Slider>().maxValue = healthMax;
        healthSlider.GetComponent<Slider>().value = healthMax;

        // set initial battery values
        batterySlider.GetComponent<Slider>().maxValue = batteryMax;
        batterySlider.GetComponent<Slider>().value = batteryMax;

        // start consume flashlight battery
        StartCoroutine(RemoveBaterryCharge(removeBatteryValue, secondToRemoveBaterry));
    }

    void Update()
    {
        if(inEnemyArea == true)
        {
            health -= 1;
        }
        // update player health slider
        healthSlider.GetComponent<Slider>().value = health;

        // update baterry slider
        batterySlider.GetComponent<Slider>().value = battery;

        // if health is low than 0
        if (health / healthMax * 100 <= 0)
        {
            SceneManager.LoadScene("GameOverScreen");
            health = 0.0f;
        }

        // if battery is low 50%
        if (battery / batteryMax * 100 <= 50)
        {
            Debug.Log("Flashlight is running out of battery.");
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 2.85f;
        }

        // if battery is low 25%
        if (battery / batteryMax * 100 <= 25)
        {
            Debug.Log("Flashlight is almost without battery.");
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 2.0f;
        }

        // if battery is low 10%
        if (battery / batteryMax * 100 <= 10)
        {
            Debug.Log("You will be out of light.");
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 1.35f;
        }

        // if battery out%
        if (battery / batteryMax * 100 <= 0)
        {
            battery = 0.00f;
            Debug.Log("The flashlight battery is out and you are out of the light.");
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 0.0f;
        }


        //animations
        if (Input.GetKey(KeyCode.LeftShift))
            this.gameObject.GetComponent<Animation>().CrossFade("Run", 1);
        else
            this.gameObject.GetComponent<Animation>().CrossFade("Idle", 1);


        // makes mouse visible when near keypad
        if (onTrigger == true)
        {
            Cursor.visible = true;
        }

        if (input == curPassword)
        {
            doorOpen = true;
            door = GameObject.Find("Door");
            Destroy(door);
        }
    }

    public IEnumerator RemoveBaterryCharge(float value, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            Debug.Log("Removing baterry value: " + value);

            if (battery > 0)
                battery -= value;
            else
                Debug.Log("The flashlight battery is out");
        }
    }

    public IEnumerator RemovePlayerHealth(float value, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            Debug.Log("Removing player health value: " + value);

            if (health > 0)
                health -= value;
            else
            {
                SceneManager.LoadScene("GameOverScreen");
            }
        }
    }

    // function to heal player
    public IEnumerator StartHealPlayer(float value, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            Debug.Log("Healling player value: " + value);

            if (health > 0 && health < healthMax)
                health += value;
            else
                health = healthMax;
        }
    }

  
    private void OnTriggerEnter(Collider collider)
    {
        // start noise when reach slender
        if (collider.gameObject.transform.tag == "Scarecrow")
        {
            if (health > 0 && paused == false)
            {
                this.GetComponent<AudioSource>().PlayOneShot(scarecrowNoise);
                inEnemyArea = true;
                
            }
        }

        if (collider.gameObject.transform.tag == "Keypad")
        {
            onTrigger = true;
            Cursor.visible = true;

        }

    }
    private void OnTriggerExit(Collider collider)
    {
        // remove noise sound
        if (collider.gameObject.transform.tag == "Scarecrow")
        {
            if (health > 0 && paused == false)
            {
                this.GetComponent<AudioSource>().clip = null;
                this.GetComponent<AudioSource>().loop = false;
                inEnemyArea = false;
            }
        }

        if (collider.gameObject.transform.tag == "Keypad")
        {
            onTrigger = false;
            keypadScreen = false;
            input = "";
        }

        // disable UI
        if (collider.gameObject.transform.tag == "Page")
            pickUpUI.SetActive(false);
    }

    void OnGUI()
    {
        if (!doorOpen)
        {
            if (onTrigger)
            {
                GUI.Box(new Rect(0, 0, 200, 25), "Press 'E' to open keypad");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    keypadScreen = true;
                    //onTrigger = false;
                }
            }

            if (keypadScreen)
            {
                GUI.Box(new Rect(0, 0, 320, 455), "");
                GUI.Box(new Rect(5, 5, 310, 25), input);

                if (GUI.Button(new Rect(5, 35, 100, 100), "1"))
                {
                    input = input + "1";
                }

                if (GUI.Button(new Rect(110, 35, 100, 100), "2"))
                {
                    input = input + "2";
                }

                if (GUI.Button(new Rect(215, 35, 100, 100), "3"))
                {
                    input = input + "3";
                }

                if (GUI.Button(new Rect(5, 140, 100, 100), "4"))
                {
                    input = input + "4";
                }

                if (GUI.Button(new Rect(110, 140, 100, 100), "5"))
                {
                    input = input + "5";
                }

                if (GUI.Button(new Rect(215, 140, 100, 100), "6"))
                {
                    input = input + "6";
                }

                if (GUI.Button(new Rect(5, 245, 100, 100), "7"))
                {
                    input = input + "7";
                }

                if (GUI.Button(new Rect(110, 245, 100, 100), "8"))
                {
                    input = input + "8";
                }

                if (GUI.Button(new Rect(215, 245, 100, 100), "9"))
                {
                    input = input + "9";
                }

            }
        }
    }
}

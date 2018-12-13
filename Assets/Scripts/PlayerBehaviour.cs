using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using Pathfinding;

public class PlayerBehaviour : MonoBehaviour
{
    public string curPassword = "12345";
    public string input;
    public bool onTrigger;
    public bool keypadScreen;
    public GameObject door;
    public bool doorOpen;
    public GameObject lightJumpScare;
    public FirstPersonController firstPersonController;

    public GameObject zombie;
    public GameObject scarecrow; 

    //Crawler Information
    public GameObject crawlerAI;
    public GameObject crawler;

    [Header("Stamina Settings")]
    public GameObject staminaSlider;
    public float stamina = 100;
    public float staminaMax = 100;

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
    public AudioClip zombieNoise;
    public AudioClip crawlerNoise;
    public bool isPlaying = false;


    [Header("UI Settings")]
    public GameObject inGameMenuUI;
    public GameObject pickUpUI;
    public GameObject finishedGameUI;
    public bool paused;

    private int idols;
    public GameObject endGate1;
    public GameObject endGate2;
    public GameObject ending;

    public GameObject objectiveText;
    public GameObject goalText;

    void Start()
    {
        idols = 0;
        ending.SetActive(false);

        objectiveText.SetActive(false);
        goalText.SetActive(false);

        // set initial health values
        health = healthMax;
        battery = batteryMax;
        stamina = staminaMax;

        staminaSlider.GetComponent<Slider>().maxValue = staminaMax;
        staminaSlider.GetComponent<Slider>().value = staminaMax;

        healthSlider.GetComponent<Slider>().maxValue = healthMax;
        healthSlider.GetComponent<Slider>().value = healthMax;

        // set initial battery values
        batterySlider.GetComponent<Slider>().maxValue = batteryMax;
        batterySlider.GetComponent<Slider>().value = batteryMax;

        // start consume flashlight battery
        StartCoroutine(RemoveBaterryCharge(removeBatteryValue, secondToRemoveBaterry));

        firstPersonController = GetComponent<FirstPersonController>();
    }

    void Update()
    {
        if (!firstPersonController.m_IsWalking)
        {
            stamina -= 0.2f;
        }
        else if (firstPersonController.tired == false)
        {
            stamina += 0.1f;
        }

        if ( stamina > staminaMax)
            stamina = staminaMax;
            

        if (inEnemyArea == true)
        {
            health -= 0.1f;
        }

        // update player health slider
        healthSlider.GetComponent<Slider>().value = health;

        // update player stamina slider
        staminaSlider.GetComponent<Slider>().value = stamina;

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
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 6.0f;
        }

        // if battery is low 25%
        if (battery / batteryMax * 100 <= 25)
        {
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 2.0f;
        }

        // if battery is low 10%
        if (battery / batteryMax * 100 <= 10)
        {
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 1.35f;
        }

        // if battery out%
        if (battery / batteryMax * 100 <= 0)
        {
            battery = 0.00f;
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 0.0f;
        }

        // if out of stamina
        if (stamina / staminaMax * 100 <= 0)
        {
            if (stamina <= 0.00f) stamina = 0.01f;
            firstPersonController.tired = true;
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
            input = "";
            objectiveText.SetActive(true);
            Invoke("textHider", 5f);
        }

        if (idols == 6)
        {
            Destroy(endGate1);
            Destroy(endGate2);
            ending.SetActive(true);
            objectiveText.SetActive(true);
            idols = 0;
            Invoke("textHider", 5f);
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
                                                             
    public IEnumerator waiter()
    {
        yield return new WaitForSeconds(10);
        this.GetComponent<AudioSource>().Stop();
        isPlaying = false;
        crawlerAI.GetComponent<Patrol>().enabled = true;
        crawlerAI.GetComponent<AIDestinationSetter>().enabled = false;
        crawlerAI.GetComponent<AIPath>().maxSpeed = 8;
        crawler.GetComponent<Animator>().SetBool("Aware", false);
    }

    public void idolAdder()
    {
        idols += 1;
    }

    public void textHider()
    {
        if (objectiveText.activeSelf == true)
        {
            objectiveText.SetActive(false);
        }
        if (goalText.activeSelf == true)
        {
            goalText.SetActive(false);
        }
    }


        public IEnumerator wait(float temp)
    {
        yield return new WaitForSeconds(3);
        Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = temp;
        scarecrow.SetActive(false);
    }

        private void OnTriggerEnter(Collider collider)
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (collider.gameObject.transform.tag == "Zombie")
        {
            if (health > 0 && paused == false)
            {
                this.GetComponent<AudioSource>().PlayOneShot(zombieNoise);
                inEnemyArea = true;

                zombie.GetComponent<Animator>().SetBool("Attack", true);
                zombie.GetComponent<Animator>().SetBool("Walk", false);

            }
        }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (collider.gameObject.transform.tag == "JumpScare")
        {
            this.GetComponent<AudioSource>().PlayOneShot(Noise);
            lightJumpScare.transform.Find("Area Light 1").gameObject.GetComponent<Light>().intensity = 0.0f;
            lightJumpScare.transform.Find("Area Light 2").gameObject.GetComponent<Light>().intensity = 0.0f;

            //Turn off flashlight
            float temp = Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity;
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 0.0f;
            StartCoroutine(wait(temp));

            //spawn scarecrow
            scarecrow.SetActive(true);
            Destroy(collider);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (collider.gameObject.transform.tag == "Keypad")
        {
            onTrigger = true;
            Cursor.visible = true;

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (collider.gameObject.transform.tag == "Crawler")

        {
            if (collider.gameObject.name == "Death") {
                crawler.GetComponent<Animator>().SetBool("Attack", true);
                inEnemyArea = true;
            }
            //Script AI Changes
            crawlerAI.GetComponent<Patrol>().enabled = false;
            crawlerAI.GetComponent<AIDestinationSetter>().enabled = true;
            crawlerAI.GetComponent<AIPath>().maxSpeed = 11;

            // Music
            if (isPlaying == false)
            {
                this.GetComponent<AudioSource>().PlayOneShot(crawlerNoise);
                isPlaying = true;
            }
            

            // Animation Changes
            crawler.GetComponent<Animator>().SetBool("Aware", true);
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
    }
    private void OnTriggerExit(Collider collider)
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (collider.gameObject.transform.tag == "Crawler")
        {
            if (collider.gameObject.name == "Death")
            {
                crawler.GetComponent<Animator>().SetBool("Attack", false);
                inEnemyArea = false;
            }

            StartCoroutine(waiter());
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (collider.gameObject.transform.tag == "Zombie")
        {
            if (health > 0 && paused == false)
            {
                this.GetComponent<AudioSource>().clip = null;
                this.GetComponent<AudioSource>().loop = false;
                inEnemyArea = false;

                zombie.GetComponent<Animator>().SetBool("Attack", false);
                zombie.GetComponent<Animator>().SetBool("Walk", true);

                this.GetComponent<AudioSource>().Stop();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (collider.gameObject.transform.tag == "Keypad")
        {
            onTrigger = false;
            keypadScreen = false;
            input = "";
        }
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

                if (GUI.Button(new Rect(5, 35, 100, 100), "1") 
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Alpha1 
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Keypad1)
                {
                    input = input + "1";
                }

                if (GUI.Button(new Rect(110, 35, 100, 100), "2") 
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Alpha2
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Keypad2)
                {
                    input = input + "2";
                }

                if (GUI.Button(new Rect(215, 35, 100, 100), "3") 
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Alpha3
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Keypad3)
                {
                    input = input + "3";
                }

                if (GUI.Button(new Rect(5, 140, 100, 100), "4") 
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Alpha4
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Keypad4)
                {
                    input = input + "4";
                }

                if (GUI.Button(new Rect(110, 140, 100, 100), "5") 
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Alpha5
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Keypad5)
                {
                    input = input + "5";
                }

                if (GUI.Button(new Rect(215, 140, 100, 100), "6") 
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Alpha6
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Keypad6)
                {
                    input = input + "6";
                }

                if (GUI.Button(new Rect(5, 245, 100, 100), "7") 
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Alpha7
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Keypad7)
                {
                    input = input + "7";
                }

                if (GUI.Button(new Rect(110, 245, 100, 100), "8") 
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Alpha8
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Keypad8)
                {
                    input = input + "8";
                }

                if (GUI.Button(new Rect(215, 245, 100, 100), "9") 
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Alpha9
                    || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Keypad9)
                {
                    input = input + "9";
                }

            }
        }
    }
}


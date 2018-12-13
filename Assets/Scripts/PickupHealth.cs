using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealth : MonoBehaviour
{
    [Header("Health System Settings")]
    public GameObject Player;
    public float healthValue;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.transform.tag == "Player")
        {
            collider.gameObject.GetComponent<PlayerBehaviour>().pickUpUI.SetActive(true);
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.transform.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("You get this healthpack: " + collider.gameObject.name);

                // disable UI
                collider.gameObject.GetComponent<PlayerBehaviour>().pickUpUI.SetActive(false);

                // add health value                
                AddHealth(collider.gameObject, healthValue);

                // disable game object
                this.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.transform.tag == "Player")
        {
            // disable UI
            collider.gameObject.GetComponent<PlayerBehaviour>().pickUpUI.SetActive(false);
        }
    }

    void AddHealth(GameObject player, float value)
    {
        Debug.LogFormat("Health value: {0}", value);
        if (player.GetComponent<PlayerBehaviour>().health < player.GetComponent<PlayerBehaviour>().healthMax)
            player.GetComponent<PlayerBehaviour>().health += value;
    }

}

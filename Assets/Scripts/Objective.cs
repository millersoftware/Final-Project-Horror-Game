using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    [Header("Objective System Settings")]
    public GameObject Player;

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
                Destroy(gameObject);
                collider.gameObject.GetComponent<PlayerBehaviour>().pickUpUI.SetActive(false);
                collider.GetComponent<PlayerBehaviour>().idolAdder();
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
}
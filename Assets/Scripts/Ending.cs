using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Ending : MonoBehaviour
{

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.transform.tag == "Player")
        {
            SceneManager.LoadScene(4);
        }
    }

}

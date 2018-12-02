using UnityEngine;
using System.Collections;

public class Keypad : MonoBehaviour
{
    public static Keypad instance;

    public static bool doorOpen;

    private void Start()
    {
        instance = this;
    }

    void Update()
    {

        if (doorOpen)
        {
            this.gameObject.transform.rotation = Quaternion.RotateTowards(this.gameObject.transform.rotation, Quaternion.Euler(0.0f, -90.0f, 0.0f), Time.deltaTime * 250);
        }
    }

}

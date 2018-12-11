using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{

    public AudioClip newTrack;
    public AudioClip oldTrack;

    private AudioManager theAM;

    void Start()
    {
        theAM = FindObjectOfType<AudioManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(newTrack != null)
            {
                theAM.ChangeBGM(newTrack);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (newTrack != null)
            {
                theAM.ChangeBGM(oldTrack);
            }
        }
    }

}

using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : MonoBehaviour {
    public GameObject mannequinAI;
    private Renderer render;
    // Use this for initialization
    void Start () {

        render = this.GetComponent<SkinnedMeshRenderer>();
    
}
	
	// Update is called once per frame
	void Update () {
        if (render.isVisible)
        {
            mannequinAI.GetComponent<AILerp>().enabled= false;
        }
        else
        {
            mannequinAI.GetComponent<AILerp>().enabled = true;
        }

    }
}

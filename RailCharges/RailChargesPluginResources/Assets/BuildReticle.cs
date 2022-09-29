using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildReticle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        const int offset = 5;
        const int size = 25;
        
        int[] quads = { 0, 1 };
        var prefab = gameObject.transform.Find("top");
        
        foreach (var x in quads)
        foreach (var y in quads)
        {
            
        }
        
        // gameObject.transform.Find("")
        Debug.Log("Yaya");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    [Header("持续时间")]
    public float keepTime = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, keepTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TankOption : MonoBehaviour
{
    [Header("单人位置")]
    public Transform transformOne;

    [Header("双人位置")]
    public Transform transformTwo;

    // 玩家数量
    [HideInInspector]
    public static int playerCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerCount = 1;
        transform.position = transformOne.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerCount = 1;
            transform.position = transformOne.position;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            playerCount = 2;
            transform.position = transformTwo.position;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (1 == playerCount)
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}

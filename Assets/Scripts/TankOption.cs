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

    [Header("联网对战位置")]
    public Transform transformNetPlatform;

    [Header("0-单人, 1-双人, 2-联网对战")]
    public Transform[] transforms;

    // 玩家数量
    [HideInInspector]
    public static int posIndex;

    // Start is called before the first frame update
    void Start()
    {
        posIndex = 0;
        transform.position = transformOne.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            posIndex = Mathf.Max(0, posIndex - 1);
            GameCreation.playerCount = 1;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            posIndex = Mathf.Min(transforms.Length - 1, posIndex + 1);
            GameCreation.playerCount = 2;
        }

        transform.position = transforms[posIndex].position;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch(posIndex)
            {
                case 0:
                case 1:
                    SceneManager.LoadScene(1);
                    break;
            }
        }
    }
}

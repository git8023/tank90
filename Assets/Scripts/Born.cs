using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Born : MonoBehaviour
{

    [Header("玩家1")]
    public GameObject player1Prefab;

    [Header("特效时长")]
    public float duration = 1.5f;

    [Header("敌人预制体")]
    public GameObject[] enemiesPrefab;

    [Header("玩家or敌人")]
    public bool createPlayer;

    // Start is called before the first frame update
    void Start()
    {
        // 延迟调用
        Invoke("BornTank", duration);
        Destroy(gameObject, duration);
    }

    private void BornTank()
    {
        if (createPlayer)
        {
            Instantiate(player1Prefab, transform.position, Quaternion.identity);
        }
        else
        {
            int index = Random.Range(0, enemiesPrefab.Length);
            Instantiate(enemiesPrefab[index], transform.position, Quaternion.identity);
        }
    }

}

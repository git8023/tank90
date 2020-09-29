using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Born : MonoBehaviour
{
    public enum BornType
    {
        PLAYER_1,
        PLAYER_2
    }

    [Header("玩家1")]
    public GameObject player1Prefab;

    [Header("特效时长")]
    public float duration = 1.5f;

    [Header("敌人预制体")]
    public GameObject[] enemiesPrefab;

    [Header("玩家or敌人")]
    public bool createPlayer;

    [Header("玩家类型")]
    [HideInInspector]
    public BornType playerType = BornType.PLAYER_1;

    // 玩家游戏物体
    private GameObject playerGo;

    // Start is called before the first frame update
    void Start()
    {
        // 延迟调用
        Invoke(nameof(BornTank), duration);
        Destroy(gameObject, duration);
    }

    private void BornTank()
    {
        if (createPlayer)
        {
            playerGo = Instantiate(player1Prefab, transform.position, Quaternion.identity);
            playerGo.GetComponent<Player>().type = playerType;
        }
        else
        {
            int index = Random.Range(0, enemiesPrefab.Length);
            Instantiate(enemiesPrefab[index], transform.position, Quaternion.identity);
        }
    }

    // 设置为玩家类型
    public void SetPlayerType(BornType type)
    {
        playerType = type;
    }
}

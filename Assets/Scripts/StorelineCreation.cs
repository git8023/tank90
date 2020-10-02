using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorelineCreation : MonoBehaviour
{

    [Header("0:总部, 1:墙: 2:障碍, 3:绿地, 4:河流, 5:空气墙, 6:出生效果")]
    public GameObject[] prefabs;

    [Header("墙数量")]
    public int wallTotal = 50;

    [Header("障碍数量")]
    public int barrierTotal = 20;

    [Header("绿地数量")]
    public int grassTotal = 20;

    [Header("河流数量")]
    public int riverTotal = 10;

    [Header("游戏道具预制体")]
    public GameObject gamePropPrefab;

    // 敌人列表
    private readonly List<GameObject> enemies = new List<GameObject>();

    // 敌人出生地
    private readonly List<Vector3> enemyBorns = new List<Vector3>();

    // 玩家列表
    private readonly List<GameObject> players = new List<GameObject>();

    // 其他游戏物体
    private readonly List<GameObject> otherGameObjects = new List<GameObject>();

    // 已使用位置
    private readonly List<Vector3> usedPositions = new List<Vector3>();

    private void Awake()
    {
        MissionCompletion();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 关卡任务完成, 进入下一关
    private void MissionCompletion()
    {

        // 清空所有游戏物体
        ClearAllGameObjects();

        // 外围围墙(空气墙)
        CreateAirBarrier();

        // 创建坦克
        CreateTanks();

        // 创建其他障碍物
        // 墙
        for (int i = 0; i < wallTotal; i++)
        {
            CreateItem(prefabs[1], RandomPosition(), Quaternion.identity);
        }
        // 障碍
        for (int i = 0; i < barrierTotal; i++)
        {
            CreateItem(prefabs[2], RandomPosition(), Quaternion.identity);
        }
        // 绿地
        for (int i = 0; i < grassTotal; i++)
        {
            CreateItem(prefabs[3], RandomPosition(), Quaternion.identity);
        }
        // 河流
        for (int i = 0; i < riverTotal; i++)
        {
            CreateItem(prefabs[4], RandomPosition(), Quaternion.identity);
        }
    }

    // 清空所有游戏物体
    private void ClearAllGameObjects()
    {
        ClearGameObject(enemies);
        ClearGameObject(players);
        ClearGameObject(otherGameObjects);
        usedPositions.Clear();
        enemyBorns.Clear();
    }

    // 销毁游戏物体
    private void ClearGameObject(List<GameObject> gos)
    {
        for (int i = 0; i < gos.Count; i++)
            Destroy(gos[i]);
        gos.Clear();
    }

    // 创建空气墙
    private void CreateAirBarrier()
    {
        // 上下
        for (int x = -11; x < 12; x++)
        {
            CreateItem(prefabs[5], new Vector3(x, 9, 0), Quaternion.identity);
            CreateItem(prefabs[5], new Vector3(x, -9, 0), Quaternion.identity);
        }
        // 左右
        for (int y = -8; y < 9; y++)
        {
            CreateItem(prefabs[5], new Vector3(11, y, 0), Quaternion.identity);
            CreateItem(prefabs[5], new Vector3(-11, y, 0), Quaternion.identity);
        }
    }

    // 随机位置排除最外层, 防止把所有路堵死.
    // 导致敌人没法进攻
    // x[-10,10]
    // y[-8,8]
    public Vector3 RandomPosition()
    {
        while (true)
        {
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-9, 10), UnityEngine.Random.Range(-7, 8), 0);
            if (!usedPositions.Contains(pos))
            {
                return pos;
            }
        }
    }

    // 创建游戏物体
    private GameObject CreateItem(GameObject go, Vector3 position, Quaternion rotation)
    {
        return CreateItem(go, position, rotation, true);
    }

    // 创建游戏物体
    private GameObject CreateItem(GameObject go, Vector3 position, Quaternion rotation, bool appendToOther)
    {
        GameObject item = Instantiate(go, position, rotation);
        item.transform.SetParent(transform);
        usedPositions.Add(position);
        if (appendToOther)
            otherGameObjects.Add(item);
        return item;
    }

    // 创建坦克
    private void CreateTanks()
    {
        CreateEnemy();

        // 玩家出生地
        // 玩家1
        GameObject player1 = CreateItem(prefabs[6], new Vector3(-2, -8, 0), Quaternion.identity);
        player1.GetComponent<Born>().createPlayer = true;
        players.Add(player1);

    }

    // 创建敌人
    private void CreateEnemy()
    {
        // 敌人出生点
        enemyBorns.Add(new Vector3(-10, 8, 0));
        enemyBorns.Add(new Vector3(0, 8, 0));
        enemyBorns.Add(new Vector3(10, 8, 0));
        int index = UnityEngine.Random.Range(0, enemyBorns.Count);
        GameObject enemy = CreateItem(prefabs[6], enemyBorns[index], Quaternion.identity);
        enemy.GetComponent<Born>().createPlayer = false;

    }
}

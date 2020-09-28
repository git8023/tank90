using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCreation : MonoBehaviour
{

    [Header("0:总部, 1:墙: 2:障碍, 3:绿地, 4:河流, 5:空气墙, 6:出生效果")]
    public GameObject[] prefabs;

    [Header("敌人最大数量")]
    public int enemyTotalConfig = 20;

    [Header("场景敌人数量")]
    public int enemyCountInScene = 2;

    [Header("剩余敌人数量")]
    public Text residueEnemyCountText;

    [Header("关卡")]
    public Text missionLevelText;

    // 单例实例
    private static GameCreation instance;

    // 敌人列表
    private List<GameObject> enemies = new List<GameObject>();

    // 已使用位置
    private List<Vector3> usedPositions = new List<Vector3>();

    // 敌人出生地
    private List<Vector3> enemyBorns = new List<Vector3>();

    // 现有敌人(包括正在生成的)数量
    private int currentEnemyTotal = 0;

    // 关卡
    private int missionLevel = 1;

    // 敌人最大数量
    private int enemyTotal;

    // 是否首次创建场景
    private bool isInit = true;

    public static GameCreation Instance { get => instance; }

    private void Awake()
    {
        isInit = true;
        instance = this;
        MissionCompletion();
    }

    private void MissionCompletion()
    {
        enemyTotal = enemyTotalConfig;
        missionLevel++;

        // 总部
        createHQ();

        // 外围围墙(空气墙)
        createAirBarrier();

        // 创建坦克
        createTanks();

        // 创建其他障碍物
        // 墙
        for (int i = 0; i < 50; i++)
        {
            createItem(prefabs[1], RandomPosition(), Quaternion.identity);
        }
        // 障碍
        for (int i = 0; i < 20; i++)
        {
            createItem(prefabs[2], RandomPosition(), Quaternion.identity);
        }
        // 绿地
        for (int i = 0; i < 30; i++)
        {
            createItem(prefabs[3], RandomPosition(), Quaternion.identity);
        }
        // 河流
        for (int i = 0; i < 20; i++)
        {
            createItem(prefabs[4], RandomPosition(), Quaternion.identity);
        }

        isInit = false;
    }

    // 创建坦克
    private void createTanks()
    {
        // 敌人出生点
        enemyBorns.Add(new Vector3(-10, 8, 0));
        enemyBorns.Add(new Vector3(0, 8, 0));
        enemyBorns.Add(new Vector3(10, 8, 0));

        if (isInit)
        {
            // 玩家出生地
            GameObject player1 = createItem(prefabs[6], new Vector3(-2, -8, 0), Quaternion.identity);
            player1.GetComponent<Born>().createPlayer = true;
        }
    }

    private void Update()
    {
        residueEnemyCountText.text = enemyTotal.ToString();
        missionLevelText.text = missionLevel.ToString();

        CreateEnemy();
    }

    // 创建敌人
    private void CreateEnemy()
    {
        if (0 < enemyTotal && currentEnemyTotal < enemyCountInScene)
        {
            int index = Random.Range(0, enemyBorns.Count);
            GameObject enemy = createItem(prefabs[6], enemyBorns[index], Quaternion.identity);
            enemy.GetComponent<Born>().createPlayer = false;
            enemyTotal--;
            currentEnemyTotal++;
        }
    }

    // 添加敌人
    public void addEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    // 删除敌人
    public void removeEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        currentEnemyTotal--;

        // 消灭完所有敌人, 进入下一关
        if (0 == enemyTotal)
        {
            MissionCompletion();
        }
    }

    // 创建总部
    private void createHQ()
    {
        createItem(prefabs[0], new Vector3(0, -8, 0), Quaternion.identity);
        createItem(prefabs[2], new Vector3(1, -8, 0), Quaternion.identity);
        createItem(prefabs[2], new Vector3(-1, -8, 0), Quaternion.identity);
        createItem(prefabs[2], new Vector3(-1, -7, 0), Quaternion.identity);
        createItem(prefabs[2], new Vector3(0, -7, 0), Quaternion.identity);
        createItem(prefabs[2], new Vector3(1, -7, 0), Quaternion.identity);
    }

    // 创建空气墙
    private void createAirBarrier()
    {
        // 上下
        for (int x = -11; x < 12; x++)
        {
            createItem(prefabs[5], new Vector3(x, 9, 0), Quaternion.identity);
            createItem(prefabs[5], new Vector3(x, -9, 0), Quaternion.identity);
        }
        // 左右
        for (int y = -8; y < 9; y++)
        {
            createItem(prefabs[5], new Vector3(11, y, 0), Quaternion.identity);
            createItem(prefabs[5], new Vector3(-11, y, 0), Quaternion.identity);
        }
    }

    // 随机位置排除最外层, 防止把所有路堵死.
    // 导致敌人没法进攻
    // x[-10,10]
    // y[-8,8]
    private Vector3 RandomPosition()
    {
        while (true)
        {
            Vector3 pos = new Vector3(Random.Range(-9, 10), Random.Range(-7, 8), 0);
            if (!usedPositions.Contains(pos))
            {
                return pos;
            }
        }
    }

    // 创建游戏物体
    private GameObject createItem(GameObject go, Vector3 position, Quaternion rotation)
    {
        GameObject item = Instantiate(go, position, rotation);
        item.transform.SetParent(transform);
        usedPositions.Add(position);
        return item;
    }
}

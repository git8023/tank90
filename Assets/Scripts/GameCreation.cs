using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Born;

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

    [Header("游戏道具预制体")]
    public GameObject gamePropPrefab;

    [Header("墙数量")]
    public int wallTotal = 50;

    [Header("障碍数量")]
    public int barrierTotal = 20;

    [Header("绿地数量")]
    public int grassTotal = 20;

    [Header("河流数量")]
    public int riverTotal = 10;

    // 玩家数量
    [HideInInspector]
    public static int playerCount;

    // 单例实例
    private static GameCreation instance;

    // 敌人列表
    private readonly List<GameObject> enemies = new List<GameObject>();

    // 玩家列表
    private readonly List<GameObject> players = new List<GameObject>();

    // 其他游戏物体
    private readonly List<GameObject> otherGameObjects = new List<GameObject>();

    // 已使用位置
    private readonly List<Vector3> usedPositions = new List<Vector3>();

    // 敌人出生地
    private readonly List<Vector3> enemyBorns = new List<Vector3>();

    // 总部防御系统
    private readonly List<GameObject> defenseShields = new List<GameObject>();

    // 现有敌人(包括正在生成的)数量
    private int currentEnemyTotal = 0;

    // 关卡
    private int missionLevel = 0;

    // 敌人最大数量
    private int enemyTotal;

    // 是否需要生成游戏道具
    public bool canGenGameProp;

    public static GameCreation Instance { get => instance; }

    private void Awake()
    {
        instance = this;
        MissionCompletion();
        canGenGameProp = true;
    }

    // 关卡任务完成, 进入下一关
    private void MissionCompletion()
    {
        enemyTotal = enemyTotalConfig;
        missionLevel++;

        // 清空所有游戏物体
        ClearAllGameObjects();

        // 总部
        CreateHQ();

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

    // 暂停所有敌人活动
    public void PauseAllEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
            enemies[i].GetComponent<Enemy>().isPause = true;
    }

    // 清空所有游戏物体
    private void ClearAllGameObjects()
    {
        ClearGameObject(enemies);
        ClearGameObject(players);
        ClearGameObject(otherGameObjects);
        ClearGameObject(defenseShields);
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

    // 创建坦克
    private void CreateTanks()
    {
        // 敌人出生点
        enemyBorns.Add(new Vector3(-10, 8, 0));
        enemyBorns.Add(new Vector3(0, 8, 0));
        enemyBorns.Add(new Vector3(10, 8, 0));
        CreateEnemy();
        CreateEnemy();
        CreateEnemy();

        // 玩家出生地
        // 玩家1
        GameObject player1 = CreateItem(prefabs[6], new Vector3(-2, -8, 0), Quaternion.identity);
        player1.GetComponent<Born>().createPlayer = true;
        players.Add(player1);
        // 玩家2
        if (2 == playerCount)
        {
            GameObject player2 = CreateItem(prefabs[6], new Vector3(2, -8, 0), Quaternion.identity);
            Born born = player2.GetComponent<Born>();
            born.SetPlayerType(BornType.PLAYER_2);
            born.createPlayer = true;
            players.Add(player2);
        }
    }

    private void Update()
    {
        residueEnemyCountText.text = enemyTotal.ToString();
        missionLevelText.text = missionLevel.ToString();

        CreateEnemy();
        GenGameProp();
    }

    // 生成游戏道具
    private void GenGameProp()
    {
        if (canGenGameProp)
        {
            Instantiate(gamePropPrefab, transform.position, Quaternion.identity);
            canGenGameProp = false;
        }
    }

    // 创建敌人
    private void CreateEnemy()
    {
        if (0 < enemyTotal && currentEnemyTotal < enemyCountInScene)
        {
            int index = UnityEngine.Random.Range(0, enemyBorns.Count);
            GameObject enemy = CreateItem(prefabs[6], enemyBorns[index], Quaternion.identity);
            enemy.GetComponent<Born>().createPlayer = false;
            enemyTotal--;
            currentEnemyTotal++;
        }
    }

    // 添加敌人
    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    // 删除敌人
    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        currentEnemyTotal--;

        // 消灭完所有敌人, 进入下一关
        if (0 == enemyTotal)
        {
            MissionCompletion();
        }
    }

    // 添加玩家
    public void AddPlayer(GameObject player)
    {
        players.Add(player);
    }

    // 删除玩家
    public void RemovePlayer(GameObject player)
    {
        players.Remove(player);
    }

    // 创建总部
    private void CreateHQ()
    {
        CreateItem(prefabs[0], new Vector3(0, -8, 0), Quaternion.identity);
        CreateDefenseShild(prefabs[1]);
    }

    // 建造防御工事
    private void CreateDefenseShild(GameObject prefab)
    {
        defenseShields.Add(CreateItem(prefab, new Vector3(1, -8, 0), Quaternion.identity, false));
        defenseShields.Add(CreateItem(prefab, new Vector3(-1, -8, 0), Quaternion.identity, false));
        defenseShields.Add(CreateItem(prefab, new Vector3(-1, -7, 0), Quaternion.identity, false));
        defenseShields.Add(CreateItem(prefab, new Vector3(0, -7, 0), Quaternion.identity, false));
        defenseShields.Add(CreateItem(prefab, new Vector3(1, -7, 0), Quaternion.identity, false));
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

    // 杀死所有敌人
    public void BombAllEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
            enemies[i].GetComponent<Enemy>().Died();
        enemies.Clear();
    }

    // 防御工事升级
    public void UpgradeDefenseShields()
    {
        ClearGameObject(defenseShields);
        CreateDefenseShild(prefabs[2]);
    }
}

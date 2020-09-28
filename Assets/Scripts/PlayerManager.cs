using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;

    [Header("复活次数")]
    public int lifeTimes = 3;

    [Header("玩家出生地")]
    public GameObject bornPrefab;

    // 玩家死亡标记
    [HideInInspector]
    public bool isDead = false;

    // 游戏失败
    [HideInInspector]
    public bool isDefeat;

    [Header("得分Text")]
    public Text scoreText;

    [Header("生命Text")]
    public Text lifeText;

    [Header("游戏结束UI")]
    public GameObject gameOverUI;

    // 玩家得分
    private int score = 0;

    // 单例实例
    public static PlayerManager Instance { get => instance; }

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // 刷新得分和生命值 
        scoreText.text = score.ToString();
        lifeText.text = lifeTimes.ToString();

        if (isDefeat)
        {
            gameOverUI.SetActive(true);
            Invoke("returnToMainScene", 3);
            return;
        }

        // 每一帧中检测玩家是否死亡
        if (isDead)
            Recover();
    }

    // 复活或游戏结束
    private void Recover()
    {

        // 游戏结束
        if (lifeTimes <=0)
        {
            isDefeat = true;
            return;
        }

        // 扣除复活次数并复活
        instance.lifeTimes--;
        GameObject playerGo = Instantiate(bornPrefab, new Vector3(-2, -8, 0),Quaternion.identity);
        playerGo.GetComponent<Born>().createPlayer = true;
        isDead = false;
    }

    // 增加分数
    public void addScore(int score)
    {
        this.score += score;
    }

    // 返回到主界面
    private void returnToMainScene()
    {
        SceneManager.LoadScene(0);
    }

}

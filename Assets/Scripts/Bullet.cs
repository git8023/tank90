using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [Header("移动速度")]
    public float moveSpeed = 10;

    // true-玩家的子弹
    // false-敌人的子弹
    [HideInInspector]
    public bool createdByPlayer;

    // Update is called once per frame
    void Update()
    {
        // 子弹总是向自身方向的前方移动
        // 前方: 2D坐标系的up方向(当前游戏)
        // 并参考世界坐标系
        transform.Translate(transform.up * moveSpeed * Time.deltaTime, Space.World);
    }

    // 触发器事件
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.tag)
        {
            // 击中玩家
            case "Tank":
                if (!createdByPlayer)
                {
                    Player player = collision.GetComponent<Player>();
                    player.Died();
                    Destroy(gameObject);
                }
                break;

            // 击中敌人
            case "Enemy":
                if (createdByPlayer)
                {
                    Enemy enemy = collision.GetComponent<Enemy>();
                    enemy.Died();
                    Destroy(gameObject);
                    PlayerManager.Instance.addScore(enemy.score);
                }
                break;

            // 可销毁的墙体
            case "Wall": 
                Destroy(collision.gameObject);
                Destroy(gameObject);
                break;

            // 不可销毁的障碍
            case "Barrier":
                // 仅玩家子弹有效
                if (createdByPlayer)
                {
                    Barrier barrier = collision.GetComponent<Barrier>();
                    barrier.playHitAudio();
                }
                Destroy(gameObject);
                break;

            // 总部
            case "HQ":
                HQ hq= collision.GetComponent<HQ>();
                Destroy(gameObject);
                hq.Died();
                break;
        }
    }
}

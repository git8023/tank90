using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("移动速度")]
    public float speed = 3;

    [Header("坦克方向图")]
    [Tooltip("上右下左")]
    public Sprite[] sprites;

    [Header("子弹预制体")]
    public GameObject bulletPrefab;

    [Header("攻击CD")]
    public float attackCd = 3;

    [Header("爆炸特效预制体")]
    public GameObject explosionPrefab;

    [Header("移动转向时间")]
    public float changeDirTime = 3;

    [Header("得分")]
    public int score = 1;

    // 精灵渲染器
    private SpriteRenderer spriteRenderer;

    // 子弹自身应该旋转的角度
    private Vector3 bulletEulerAngles;

    // 攻击计时器
    private float attackTimer = 0;

    // 转向计时器
    private float changeDirectionTimer = 0;

    // 上下移动
    float moveVirtical = 0;

    // 左右移动
    float moveHorizontal = 0;

    // 暂停移动/攻击
    public bool isPause;

    // 暂停计时器
    private float pauseTimer;

    // 暂停最大时长
    private const float MAX_PAUSE_TIME = 3;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 敌人出生后直接向下走
        moveVirtical = -1;

        // 把敌人添加到创建管理中
        GameCreation.Instance.AddEnemy(gameObject);
    }

    // Update is called once per frame
    // 不固定帧刷新
    void Update()
    {
        if (isPause)
        {
            pauseTimer += Time.deltaTime;
            isPause = MAX_PAUSE_TIME > pauseTimer;
            return;
        }

        // 攻击CD
        if (attackTimer >= attackCd)
        {
            Attack();
        }
        else
        {
            attackTimer += Time.deltaTime;
        }
    }

    // 固定帧率刷新
    // 刚体碰撞时不会发生抖动现象
    private void FixedUpdate()
    {
        if (isPause)
            return;
        Move();
    }

    // 发射子弹
    private void Attack()
    {

        // 每次攻击后清零攻击cd
        attackTimer = 0;

        // 子弹实例通过脚本设置为自动运动
        // 这里不需要保存实例
        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(transform.eulerAngles + bulletEulerAngles));
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.createdByPlayer = false;
    }

    // 移动
    private void Move()
    {
        // 随机移动
        changeDirectionTimer += Time.fixedDeltaTime;
        if (changeDirTime <= changeDirectionTimer)
        {
            changeDirectionTimer = 0;
            int dirNum = Random.Range(0, 6);
            switch (dirNum)
            {
                case 0:
                    moveVirtical = 0;
                    moveHorizontal = 1;
                    break;
                case 1:
                    moveVirtical = 0;
                    moveHorizontal = -1;
                    break;
                case 2:
                    moveVirtical = 1;
                    moveHorizontal = 0;
                    break;
                default:
                    moveVirtical = -1;
                    moveHorizontal = 0;
                    break;
            }
        }

        // 监听垂直移动
        transform.Translate(Vector3.up * moveVirtical * speed * Time.fixedDeltaTime, Space.World);
        if (0 > moveVirtical)
        {
            spriteRenderer.sprite = sprites[2];
            bulletEulerAngles = new Vector3(0, 0, 180);
        }
        else if (0 < moveVirtical)
        {
            spriteRenderer.sprite = sprites[0];
            bulletEulerAngles = new Vector3(0, 0, 0);
        }

        // 防止斜方向移动
        if (0 != moveVirtical)
        {
            return;
        }

        // 监听水平方向
        transform.Translate(Vector3.right * moveHorizontal * speed * Time.fixedDeltaTime, Space.World);
        if (0 > moveHorizontal)
        {
            spriteRenderer.sprite = sprites[3];

            // 2D与3D在X轴向相反
            // 在2D中UI是从屏幕里面网外看
            // 所有期望向左移动就需要使用(Z轴反向旋转)右朝向
            bulletEulerAngles = new Vector3(0, 0, 90);
        }
        else if (0 < moveHorizontal)
        {
            spriteRenderer.sprite = sprites[1];
            bulletEulerAngles = new Vector3(0, 0, -90);
        }
    }

    // 敌人死亡
    public void Died()
    {
        // 播放爆炸特效
        Instantiate(explosionPrefab, transform.position, transform.rotation);

        // 销毁游戏物体
        Destroy(gameObject);

        // 删除敌人
        GameCreation.Instance.RemoveEnemy(gameObject);
    }

    // 相互碰撞后立即转向, 防止扎堆
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            changeDirectionTimer = changeDirTime;
        }
    }
}

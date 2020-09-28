using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("移动速度")]
    public float speed = 3;

    [Header("坦克方向图")]
    [Tooltip("上右下左")]
    public Sprite[] sprites;

    [Header("子弹预制体")]
    public GameObject bulletPrefab;

    [Header("子弹CD")]
    public float bulletCd = 0.4f;

    [Header("爆炸特效预制体")]
    public GameObject explosionPrefab;

    [Header("无敌时长")]
    public float defendTimeConfig = 3;

    [Header("无敌特效预制体")]
    public GameObject defendEffect;

    [Header("音效播放器")]
    public AudioSource audioSource;

    [Header("音效: 0-闲置, 1-移动中")]
    public AudioClip[] tankAudios;

    // 精灵渲染器
    private SpriteRenderer spriteRenderer;

    // 子弹自身应该旋转的角度
    private Vector3 bulletEulerAngles;

    // 攻击增量CD
    private float attackTimer = 0;

    // 受保护标记
    private bool isDefend = true;

    // 无敌时长
    private float defendTime;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameCreation.Instance.AddPlayer(gameObject);
        StartDefend();
    }

    // Update is called once per frame
    // 不固定帧刷新
    void Update()
    {
        // 受保护cd
        if (isDefend)
        {
            defendTime -= Time.deltaTime;
            isDefend = defendTime > 0;
        }
        defendEffect.SetActive(isDefend);


        // 总部被摧毁, 玩家不可再操作
        if (PlayerManager.Instance.isDefeat)
        {
            return;
        }

        // 攻击CD
        attackTimer += Time.deltaTime;
        if (attackTimer >= bulletCd)
            Attack();
    }

    // 固定帧率刷新
    // 刚体碰撞时不会发生抖动现象
    private void FixedUpdate()
    {

        // 总部被摧毁, 玩家不可再操作
        if (PlayerManager.Instance.isDefeat)
        {
            return;
        }

        Move();

    }

    // 发射子弹
    private void Attack()
    {
        // 按空格发射子弹
        if (Input.GetKeyDown(KeyCode.Space))
        {

            // 每次攻击后清零攻击cd
            attackTimer = 0;

            // 子弹实例通过脚本设置为自动运动
            // 这里不需要保存实例
            GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(transform.eulerAngles + bulletEulerAngles));
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.createdByPlayer = true;

        }
    }

    // 移动
    private void Move()
    {
        // 监听垂直移动
        // Vectory3.up: 上方向
        float v = Input.GetAxisRaw("Vertical");
        transform.Translate(Vector3.up * v * speed * Time.fixedDeltaTime, Space.World);
        if (0 > v)
        {
            spriteRenderer.sprite = sprites[2];
            bulletEulerAngles = new Vector3(0, 0, 180);
        }
        else if (0 < v)
        {
            spriteRenderer.sprite = sprites[0];
            bulletEulerAngles = new Vector3(0, 0, 0);
        }

        // 防止斜方向移动
        float h = 0;
        if (0 == v)
        {
            // 监听水平方向
            // Vector3.right: 右方向
             h = Input.GetAxisRaw("Horizontal");
            transform.Translate(Vector3.right * h * speed * Time.fixedDeltaTime, Space.World);
            if (0 > h)
            {
                spriteRenderer.sprite = sprites[3];

                // 2D与3D在X轴向相反
                // 在2D中UI是从屏幕里面网外看
                // 所有期望向左移动就需要使用(Z轴反向旋转)右朝向
                bulletEulerAngles = new Vector3(0, 0, 90);
            }
            else if (0 < h)
            {
                spriteRenderer.sprite = sprites[1];
                bulletEulerAngles = new Vector3(0, 0, -90);
            }
        }

        // 播放音效
        float bounds = 0.05f;
        if (bounds < Mathf.Abs(v) || bounds < Mathf.Abs(h))
            audioSource.clip = tankAudios[1];
        else
            audioSource.clip = tankAudios[0];
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    // 玩家死亡
    public void Died()
    {
        // 受保护(无敌)状态不做处理
        if (isDefend)
        {
            return;
        }

        // 播放爆炸特效
        Instantiate(explosionPrefab, transform.position, transform.rotation);

        // 销毁游戏物体
        Destroy(gameObject);

        // 通知玩家管理者, 玩家需要复活
        PlayerManager.Instance.isDead = true;

        // 删除玩家引用
        GameCreation.Instance.RemovePlayer(gameObject);
    }

    // 开启无敌
    public void StartDefend()
    {
        defendTime = defendTimeConfig;
        isDefend = true;
    }
}

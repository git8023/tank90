﻿using System;
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
    public float defendTime = 3;

    [Header("无敌特效预制体")]
    public GameObject defendEffect;

    // 精灵渲染器
    private SpriteRenderer spriteRenderer;

    // 子弹自身应该旋转的角度
    private Vector3 bulletEulerAngles;

    // 攻击增量CD
    private float attackTimer = 0;

    // 受保护标记
    private bool isDefend = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {

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

        // 攻击CD
        if (attackTimer >= bulletCd)
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
        if (0 != v)
        {
            return;
        }

        // 监听水平方向
        // Vector3.right: 右方向
        float h = Input.GetAxisRaw("Horizontal");
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
    }
}
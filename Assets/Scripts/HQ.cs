using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQ : MonoBehaviour
{

    [Header("摧毁图片")]
    public Sprite brokenSprite;

    [Header("爆炸特效")]
    public GameObject explosionPrefab;

    // 图片渲染器
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 总部被摧毁, 游戏结束
    public void Died()
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        spriteRenderer.sprite = brokenSprite;
    }

}

using System;
using UnityEngine;

public class Prop : MonoBehaviour
{

    public enum PropType
    {
        /// <summary>
        /// 生命
        /// </summary>
        LIFE,

        /// <summary>
        /// 敌人暂停
        /// </summary>
        PAUSE,

        /// <summary>
        /// 加强总部工事
        /// </summary>
        DEFENSE_HQ,

        /// <summary>
        /// 炸弹
        /// </summary>
        BONUS,

        /// <summary>
        /// 火力升级
        /// </summary>
        FIRE_UPGRADE,

        /// <summary>
        /// 无敌盾牌
        /// </summary>
        SHIELD,

        MAX
    }

    [Header("0-生命, 1-暂停, 2-工事, 3-炸弹, 4-火力升级, 5-无敌盾牌")]
    public Sprite[] sprites;

    // 精灵图渲染器
    private SpriteRenderer spriteRenderer;

    // 道具类型
    private PropType propType;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        RandomPropType();
        transform.position = GameCreation.Instance.RandomPosition();
    }

    // 随机道具类型
    private void RandomPropType()
    {
        propType = (PropType)UnityEngine.Random.Range((int)PropType.LIFE, (int)PropType.MAX);
        spriteRenderer.sprite = sprites[(int)propType];
    }

    // 玩家进入触发器
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tank"))
        {
            Player player = collision.GetComponent<Player>();
            player.PlayGetBonus();
            Comsume(player);
        }
    }

    // 消耗道具
    private void Comsume(Player player)
    {

        switch (propType)
        {
            /// 增加生命值
            case PropType.LIFE:
                PlayerManager.Instance.lifeTimes++;
                break;

            /// 敌人暂停
            case PropType.PAUSE:
                GameCreation.Instance.PauseAllEnemies();
                break;

            /// 加强总部工事
            case PropType.DEFENSE_HQ:
                GameCreation.Instance.UpgradeDefenseShields();
                break;

            /// 炸弹
            case PropType.BONUS:
                GameCreation.Instance.BombAllEnemies();
                break;

            /// 火力升级
            case PropType.FIRE_UPGRADE:
                player.UpgradeFire();
                break;

            /// 无敌盾牌
            case PropType.SHIELD:
                player.StartDefend();
                break;
        }

        Destroy(gameObject);
        GameCreation.Instance.canGenGameProp = true;
    }
}

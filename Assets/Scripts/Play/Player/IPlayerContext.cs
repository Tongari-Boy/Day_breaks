using UnityEngine;

namespace Player
{
    /// <summary>
    /// これを実装したクラスはプレイヤーの体力や移動速度などのデータを保持する
    /// </summary>
    public interface IPlayerContext
    {
        /// <summary>
        /// プレイヤーの体力
        /// </summary>
        float Health { get; set; }

        /// <summary>
        /// プレイヤーの体力（最大）
        /// </summary>
        float MaxHealth { get; set; }

        /// <summary>
        /// 通常移動の速度係数
        /// </summary>
        float MovementSpeed { get; set; }

        /// <summary>
        /// ダッシュ時の速度係数
        /// </summary>
        float SprintSpeed { get; set; }

        /// <summary>
        /// プレイヤーの弾
        /// </summary>
        GameObject PlayerBullet { get; set; }

        /// <summary>
        /// プレイヤーの弾を射撃するGameObject（シューター）
        /// </summary>
        GameObject PlayerShooter { get; set; }

        /// <summary>
        /// プレイヤーの弾がスポーンする発射点からの距離（シューターの位置を基準とする）
        /// </summary>
        float BulletSpawnDistance { get; set; }

        /// <summary>
        /// プレイヤーの弾の速度係数
        /// </summary>
        float BulletSpeed { get; set; }

        /// <summary>
        /// <para>プレイヤーの射撃後のクールダウンの残り秒数</para>
        /// <para>0以下になるまで次の射撃はできない</para>
        /// </summary>
        float ShootingCooldown { get; set; }

        /// <summary>
        /// プレイヤーの射撃後のクールダウンの秒数
        /// </summary>
        float MaxShootingCooldown { get; set; }
    }
}
using Physics;
using Player.Item;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Player
{
    /// <summary>
    /// <para>プレイヤーを管理するクラス</para>
    /// <para>インスタンスを取得するにはPlayerManager.INSTANCEを参照する</para>
    /// </summary>
    public class PlayerManager
    {
        /// <summary>
        /// PlayerManagerクラスのインスタンス
        /// </summary>
        public static readonly PlayerManager INSTANCE = new();

        /// <summary>
        /// <para>プレイヤーとして扱うGameObject</para>
        /// </summary>
        private GameObject playerObject;

        /// <summary>
        /// プレイヤーの位置や速度を保持
        /// </summary>
        private Entity playerEntity;

        /// <summary>
        /// プレイヤーのデータ
        /// </summary>
        private IPlayerContext playerContext;

        /// <summary>
        /// プレイヤーの移動入力
        /// </summary>
        private InputAction moveAction;

        /// <summary>
        /// プレイヤーのダッシュ入力
        /// </summary>
        private InputAction sprintAction;

        /// <summary>
        /// プレイヤーの建築入力
        /// </summary>
        private InputAction useAction;

        /// <summary>
        /// プレイヤーの射撃入力
        /// </summary>
        private InputAction shootAction;

        /// <summary>
        /// プレイヤーのマウスカーソル入力
        /// </summary>
        private InputAction cursorAction;

        private PlayerManager() { }

        /// <summary>
        /// <para>任意のGameObjectをプレイヤーとして登録する</para>
        /// </summary>
        public void SetPlayer(GameObject gameObject)
        {
            // プレイヤーデータの初期化
            this.playerObject = gameObject;
            this.playerEntity = null;
            this.playerContext = null;

            this.GetContext();
        }

        /// <summary>
        /// プレイヤーのコンテキストを取得する
        /// </summary>
        private void GetContext()
        {
            if (this.playerObject)
            {
                MonoBehaviour[] monoBehaviours = this.playerObject.GetComponents<MonoBehaviour>();

                foreach (MonoBehaviour monoBehaviour in monoBehaviours)
                {
                    if (monoBehaviour is null)
                        continue;

                    if (monoBehaviour && monoBehaviour.enabled)
                    {
                        if (monoBehaviour is PlayerBehaviour playerBehaviour)
                        {
                            this.playerEntity = playerBehaviour.Entity;
                            this.playerContext = playerBehaviour;

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// PlayerManagerの初期化
        /// </summary>
        public void Initialize()
        {
            // 入力の初期化
            InputActionMap playerActions = InputSystem.actions.FindActionMap("Player");

            this.moveAction = playerActions.FindAction("Move");
            this.sprintAction = playerActions.FindAction("Sprint");
            this.useAction = playerActions.FindAction("Use");
            this.shootAction = playerActions.FindAction("Shoot");
            this.cursorAction = playerActions.FindAction("Cursor");

            // 物理エンジンの初期化
            PhysicsManager.INSTANCE.Initialize();
        }

        /// <summary>
        /// PlayerManagerの更新
        /// </summary>
        public void OnUpdate()
        {
            // playerObject、playerEntity, playerContextが存在する場合のみ実行
            if (this.playerObject)
            {
                if (this.playerEntity is not null && this.playerContext is not null)
                {
                    // プレイヤーのアクションを実行
                    this.Move();
                    this.Use();
                    this.Shoot();
                }
                else
                {
                    // プレイヤーのコンテキストを再取得
                    this.GetContext();
                }
            }

            // 物理エンジンの更新
            PhysicsManager.INSTANCE.OnUpdate();
        }

        /// <summary>
        /// プレイヤーの移動
        /// </summary>
        private void Move()
        {
            // 移動する方向
            Vector2 direction = this.moveAction.ReadValue<Vector2>();

            // WASD入力がないときは何もしない
            if (direction == Vector2.zero)
                return;

            // 最終的な速度係数を計算
            float finalSpeed = this.playerContext.MovementSpeed;

            if (sprintAction.IsPressed()) // ダッシュ時にSprintSpeedを乗算
                finalSpeed *= this.playerContext.SprintSpeed;

            // 座標の更新
            this.playerEntity.Velocity = finalSpeed * direction;
        }

        /// <summary>
        /// アイテムの使用
        /// </summary>
        private void Use()
        {
            // Spaceが押された時にのみ使用
            if (!this.useAction.WasPressedThisFrame())
                return;

            // プレイヤーのアイテムスロットを取得
            List<PlayerItemState> playerItemSlots = this.playerContext.PlayerItemSlots;

            // 選択中のアイテムスロットの番号を取得
            int slotIndex = this.playerContext.BindingPlayerItemSlot;

            // スロットの番号が範囲外の場合は何もしない
            if (slotIndex < 0 && slotIndex >= playerItemSlots.Count)
                return;

            PlayerItemState playerItemState = playerItemSlots[slotIndex];

            if (playerItemState is not null)
            {
                // アイテムの使用
                playerItemState.Use(this.playerContext);
            }
        }

        /// <summary>
        /// プレイヤーの射撃
        /// </summary>
        private void Shoot()
        {
            // クールダウンが残っていれば減らす。0以下になるまで射撃はできない。
            if (this.playerContext.ShootingCooldown > 0F)
            {
                this.playerContext.ShootingCooldown -= Time.deltaTime;
                return;
            }

            // マウスボタンが押された時にのみ射撃（長押しでの射撃はできない）
            if (!this.shootAction.WasPressedThisFrame())
                return;

            GameObject shooterObject = this.playerContext.PlayerBulletShooter;

            // シューターが設定されていない場合はプレイヤー自身をシューターとする
            if (!shooterObject)
                shooterObject = this.playerObject;

            // マウスカーソルのワールド座標を取得
            Vector2 cursorPos = this.cursorAction.ReadValue<Vector2>();
            Vector3 shooterPos = shooterObject.transform.position;
            Vector3 aimPos = Camera.main ? Camera.main.ScreenToWorldPoint(new Vector3(cursorPos.x, cursorPos.y, 0F)) : shooterPos;

            // プレイヤーからマウスカーソルへの方向を取得
            Vector3 aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y, 0F) - new Vector3(shooterPos.x, shooterPos.y, 0F));

            // プレイヤーの弾をスポーン
            if (this.playerContext.PlayerBullet)
            {
                Vector3 bulletPos = shooterPos + aimDir * this.playerContext.BulletSpawnDistance;

                EntityBehaviour entityBehaviour = UnityEngine.Object.Instantiate(this.playerContext.PlayerBullet, bulletPos, Quaternion.identity).AddComponent<EntityBehaviour>();

                entityBehaviour.Entity.Position = bulletPos;
                entityBehaviour.Entity.Velocity = aimDir * this.playerContext.BulletSpeed;
            }
            else
            {
                Debug.LogWarning("Player BulletにGame Objectが設定されていません！");
            }

            // クールダウンを設定
            this.playerContext.ShootingCooldown = this.playerContext.MaxShootingCooldown;
        }
    }
}
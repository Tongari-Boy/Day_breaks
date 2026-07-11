using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// <para>プレイヤーを管理するクラス</para>
    /// <para>このクラスはシングルトン設計であるため、PlayerManager.INSTANCEからしかインスタンスを取得できない</para>
    /// <para>SetPlayerメソッドを使ってGameObjectを登録でき、登録されたGameObjectはプレイヤーとして扱われる</para>
    /// <para>登録されたGameObject（プレイヤー）は移動、ダッシュ、射撃などアクションの実行が可能となる</para>
    /// <para>IPlayerContextを実装したMonoBehaviourをGameObjectにアタッチすれば体力や移動速度などを制御できる</para>
    /// <para>※動作させるにはInitializeメソッド、OnUpdateメソッドを実行する必要がある</para>
    /// </summary>
    public class PlayerManager
    {
        /// <summary>
        /// PlayerManagerクラスのインスタンス
        /// </summary>
        public static readonly PlayerManager INSTANCE = new();

        /// <summary>
        /// プレイヤーとして扱うGameObject
        /// </summary>
        private GameObject playerObject;

        /// <summary>
        /// プレイヤーのコライダー（存在しない場合はnull）
        /// </summary>
        private Collider2D playerCollider;

        /// <summary>
        /// プレイヤーの体力や移動速度などのデータを保持
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
        private InputAction buildAction;

        /// <summary>
        /// プレイヤーの射撃入力
        /// </summary>
        private InputAction shootAction;

        /// <summary>
        /// プレイヤーのマウスカーソル入力
        /// </summary>
        private InputAction cursorAction;

        /// <summary>
        /// プレイヤーとなるGameObjectに、IPlayerContextを実装したMonoBehaviourがアタッチされていなかった場合に使用
        /// </summary>
        private readonly IPlayerContext defaultContext;

        private PlayerManager() {
            // インスタンスの初期化
            defaultContext = new DefaultContext();
        }

        /// <summary>
        /// <para>GameObjectを登録する</para>
        /// <para>登録されたGameObjectはプレイヤーとして扱われ、移動や射撃などのアクションが可能となる</para>
        /// </summary>
        /// <param name="gameObject">登録するGameObject</param>
        public void SetPlayer(GameObject gameObject)
        {
            // プレイヤーデータの初期化
            playerObject = gameObject;
            playerCollider = null;
            playerContext = defaultContext;

            if (playerObject)
            {
                // Collider2Dを取得
                playerCollider = playerObject.GetComponent<Collider2D>();

                // IPlayerContextを実装したMonoBehaviourを取得
                MonoBehaviour monoBehaviour = playerObject.GetComponent<MonoBehaviour>();

                if (monoBehaviour is IPlayerContext iPlayerContext)
                {
                    playerContext = iPlayerContext;
                }
            }
        }

        /// <summary>
        /// <para>PlayerManagerの初期化</para>
        /// <para>OnUpdateメソッドの実行前に必ず呼び出す必要がある</para>
        /// </summary>
        public void Initialize()
        {
            // 入力の初期化
            InputActionMap playerActions = InputSystem.actions.FindActionMap("Player");

            moveAction = playerActions.FindAction("Move");
            sprintAction = playerActions.FindAction("Sprint");
            buildAction = playerActions.FindAction("Build");
            shootAction = playerActions.FindAction("Shoot");
            cursorAction = playerActions.FindAction("Cursor");
        }

        /// <summary>
        /// PlayerManagerの更新
        /// </summary>
        public void OnUpdate()
        {
            // playerObjectが存在する場合のみ実行
            if (playerObject)
            {
                Move();
                Build();
                Shoot();
            }
        }

        /// <summary>
        /// プレイヤーの移動
        /// </summary>
        private void Move()
        {
            // 移動する方向
            Vector2 direction = moveAction.ReadValue<Vector2>();

            // WASD入力がないときは何もしない
            if (direction == Vector2.zero)
                return;

            // 最終的な速度係数を計算
            float finalSpeed = Time.deltaTime * playerContext.MovementSpeed;

            if (sprintAction.IsPressed()) // ダッシュ時にSprintSpeedを乗算
                finalSpeed *= playerContext.SprintSpeed;

            // 座標の更新
            playerObject.transform.Translate(direction.x * finalSpeed, direction.y * finalSpeed, 0F);
        }

        /// <summary>
        /// プレイヤーの建築（未実装）
        /// </summary>
        private void Build()
        {
            if (buildAction.WasPressedThisFrame())
            {
                Debug.Log("Build");
            }
        }

        /// <summary>
        /// プレイヤーの射撃
        /// </summary>
        private void Shoot()
        {
            // クールダウンが残っていれば減らす。0以下になるまで射撃はできない。
            if (playerContext.ShootingCooldown > 0F)
            {
                playerContext.ShootingCooldown -= Time.deltaTime;
                return;
            }

            // マウスボタンが押された時にのみ射撃（長押しでの射撃はできない）
            if (!shootAction.WasPressedThisFrame())
                return;

            // マウスカーソルの座標を取得
            Vector2 cursorPos = cursorAction.ReadValue<Vector2>();
            Vector3 shooterPos = playerContext.PlayerShooter ? playerObject.transform.position : playerContext.PlayerShooter.transform.position;
            Vector3 aimPos = Camera.main ? Camera.main.ScreenToWorldPoint(new Vector3(cursorPos.x, cursorPos.y, 0F)) : shooterPos;

            // プレイヤーからマウスカーソルへの方向を取得
            Vector3 aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y, 0F) - new Vector3(shooterPos.x, shooterPos.y, 0F));

            // 弾のスポーン
            if (playerContext.PlayerBullet)
            {
                UnityEngine.Object.Instantiate(playerContext.PlayerBullet, shooterPos + aimDir * playerContext.BulletSpawnDistance, Quaternion.identity);
            }

            // クールダウンを設定
            playerContext.ShootingCooldown = playerContext.MaxShootingCooldown;
        }

        private class DefaultContext : IPlayerContext
        {
            public float Health
            {
                get { return 0F; }
                set { }
            }

            public float MaxHealth
            {
                get { return 0F; }
                set { }
            }

            public float MovementSpeed
            {
                get { return 1F; }
                set { }
            }

            public float SprintSpeed
            {
                get { return 1F; }
                set { }
            }

            public GameObject PlayerBullet
            {
                get { return null; }
                set { }
            }

            public GameObject PlayerShooter
            {
                get { return null; }
                set { }
            }

            public float BulletSpawnDistance
            {
                get { return 1F; }
                set { }
            }

            public float BulletSpeed
            {
                get { return 1F; }
                set { }
            }

            public float ShootingCooldown
            {
                get { return 0F; }
                set { }
            }

            public float MaxShootingCooldown
            {
                get { return 0F; }
                set { }
            }
        }
    }
}
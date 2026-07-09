using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Player
{
    public class PlayerBehaviour : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーの速度係数（通常）
        /// </summary>
        [Header("プレイヤーの速度係数（通常）")]
        [SerializeField] private float motionSpeed = 1F;

        /// <summary>
        /// プレイヤーの速度係数（ダッシュ）
        /// </summary>
        [Header("プレイヤーの速度係数（ダッシュ）")]
        [SerializeField] private float sprintSpeed = 3F;

        /// <summary>
        /// プレイヤーの射撃後のクールダウン（秒）
        /// </summary>
        [Header("プレイヤーの射撃後のクールダウン（秒）")]
        [SerializeField] private float shootCooldown = 3F;

        /// <summary>
        /// プレイヤーの弾（デバッグ用）
        /// </summary>
        [Header("プレイヤーの弾（デバッグ用）")]
        [SerializeField] private GameObject debugBullet;

        /// <summary>
        /// プレイヤーの弾がスポーンする距離（デバッグ用）
        /// </summary>
        [Header("プレイヤーの弾がスポーンする距離（デバッグ用）")]
        [SerializeField] private float bulletSpawnDistance = 1F;

        /// <summary>
        /// プレイヤー弾の速度係数（デバッグ用）
        /// </summary>
        [Header("プレイヤー弾の速度係数（デバッグ用）")]
        [SerializeField] private float bulletSpeed = 3F;

        /// <summary>
        /// プレイヤーの移動入力
        /// </summary>
        private InputAction moveAction;

        /// <summary>
        /// プレイヤーのダッシュ入力
        /// </summary>
        private InputAction sprintAction;

        /// <summary>
        /// プレイヤーの射撃入力
        /// </summary>
        private InputAction shootAction;

        /// <summary>
        /// プレイヤーのマウスカーソル
        /// </summary>
        private InputAction cursorAction;

        /// <summary>
        /// プレイヤーの射撃後の残っているクールダウン。この値が0以下のときに発射可能となる。
        /// </summary>
        private float remainingCooldown = 0F;

        public void Initialize()
        {
            InputActionMap playerActions = InputSystem.actions.FindActionMap("Player");

            moveAction = playerActions.FindAction("Move");
            sprintAction = playerActions.FindAction("Sprint");
            shootAction = playerActions.FindAction("Shoot");
            cursorAction = playerActions.FindAction("Cursor");
        }

        public void OnUpdate()
        {
            Move();
            Shoot();
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
            float finalSpeed = Time.deltaTime * motionSpeed;

            if (sprintAction.IsPressed()) // ダッシュ時にsprintSpeedを乗算
                finalSpeed *= sprintSpeed;

            // 座標の更新
            transform.Translate(direction.x * finalSpeed, direction.y * finalSpeed, 0F);
        }

        /// <summary>
        /// プレイヤーの射撃
        /// </summary>
        private void Shoot()
        {
            // クールダウンが残っていれば減らす。0以下になるまで発射はできない。
            if (remainingCooldown > 0F)
            {
                remainingCooldown -= Time.deltaTime;

                return;
            }

            // マウスボタンが押された時にのみ射撃（長押しでの射撃はできない）
            if (!shootAction.WasPressedThisFrame())
                return;

            // マウスカーソルの座標を取得
            Vector3 playerPos = transform.position;
            Vector2 cursorPos = cursorAction.ReadValue<Vector2>();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(cursorPos.x, cursorPos.y, 0F));

            // プレイヤーからマウスカーソルへの方向を取得
            Vector3 aimDirection = Vector3.Normalize(new Vector3(worldPos.x, worldPos.y, 0F) - new Vector3(playerPos.x, playerPos.y, 0F));

            // 弾のスポーン（デバッグ用）
            if (debugBullet)
            {
                GameObject gameObject = Instantiate(debugBullet, playerPos + aimDirection * bulletSpawnDistance, Quaternion.identity);
                Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();

                // 弾に速度を付与
                if (rigidbody2D)
                {
                    rigidbody2D.linearVelocity = aimDirection * bulletSpeed;
                }
            }

            // クールダウンを設定
            remainingCooldown = shootCooldown;
        }

        void Start()
        {
            // デバッグ用
            Initialize();
        }

        void Update()
        {
            // デバッグ用
            OnUpdate();
        }
    }
}
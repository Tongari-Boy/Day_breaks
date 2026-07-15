using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour, IDamageable
{
    [Header("プレイヤーの体力")]
    [SerializeField] private int health = 100;

    [Header("プレイヤーの攻撃力")]
    [SerializeField] private int attackDamage = 50;

    [Header("プレイヤーの速度（通常）")]
    [SerializeField] private float movementSpeed = 1.0F;

    [Header("プレイヤーの速度（ダッシュ時）")]
    [SerializeField] private float sprintSpeed = 3.0F;

    [Header("シューター")]
    [SerializeField] private GameObject shooterObject;

    [Header("弾")]
    [SerializeField] private GameObject bulletObject;

    [Header("弾の速度")]
    [SerializeField] private float bulletSpeed = 5.0F;

    [Header("弾の寿命（秒）")]
    [SerializeField] private float bulletLifespan = 10.0F;

    [Header("弾のスポーン位置までの距離")]
    [SerializeField] private float bulletSpawnDistance = 1.0F;

    [Header("射撃後のクールダウン")]
    [SerializeField] private float shootingCooldown = 1.0F;

    [Header("連射モード")]
    [SerializeField] private bool holdingShootingMode = false;

    private int remainingHealth;
    private float remainingShootingCooldown;

    private InputAction move;
    private InputAction sprint;
    private InputAction use;
    private InputAction shoot;
    private InputAction cursor;

    private Rigidbody2D rigidbody2D;

    public void Start()
    {
        // 入力の取得
        InputActionMap playerActions = this.GetComponent<PlayerInput>().currentActionMap;

        this.move = playerActions.FindAction("Move");
        this.sprint = playerActions.FindAction("Sprint");
        this.use = playerActions.FindAction("Use");
        this.shoot = playerActions.FindAction("Shoot");
        this.cursor = playerActions.FindAction("Cursor");

        // Rigidbody2Dの取得
        this.rigidbody2D = this.GetComponent<Rigidbody2D>();

        // ステータスの初期化
        this.remainingHealth = this.health;
        this.remainingShootingCooldown = this.shootingCooldown;
    }

    public void Update()
    {
        this.Move();
        this.Use();
        this.Shoot();
    }

    private void Move()
    {
        // 入力を確認
        if (this.move == null || !this.move.IsPressed())
            return;

        // Rigidbody2Dを確認
        if (this.rigidbody2D == null)
            return;

        // 速度の計算
        Vector2 direction = this.move.ReadValue<Vector2>();
        float finalSpeed = this.sprint != null && this.sprint.IsPressed() ? this.sprintSpeed : this.movementSpeed;

        // 速度の適用
        this.rigidbody2D.linearVelocity = direction * finalSpeed;
    }

    private void Use()
    {
    }

    private void Shoot()
    {
    }

    /// <summary>
    /// IDamageableより実装
    /// </summary>
    public void OnDamaged(int damageAmount)
    {
        this.remainingHealth = Mathf.Clamp(this.remainingHealth - damageAmount, 0, this.health);
    }
}

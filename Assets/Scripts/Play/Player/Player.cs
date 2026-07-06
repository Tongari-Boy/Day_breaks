using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerBehaviour : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーの速度係数
        /// </summary>
        private float speedFactor = 1.0F;

        /// <summary>
        /// プレイヤーの移動入力
        /// </summary>
        private InputAction moveAction;

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

        public void Initialize()
        {
            InputActionMap playerActions = InputSystem.actions.FindActionMap("Player");

            moveAction = playerActions.FindAction("Move");
        }

        public void OnUpdate()
        {
            MovePlayer();
        }

        /// <summary>
        /// WASD入力によるプレイヤーの移動
        /// </summary>
        private void MovePlayer()
        {
            Vector2 movement = Time.deltaTime * speedFactor * moveAction.ReadValue<Vector2>();

            transform.Translate(movement.x, movement.y, 0F);
        }
    }
}
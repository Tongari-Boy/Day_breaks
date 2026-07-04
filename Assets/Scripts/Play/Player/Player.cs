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
            InputActionMap playerActions = InputSystem.actions.FindActionMap("Player");

            moveAction = playerActions.FindAction("Move");
        }

        void Update()
        {
            Vector2 movement =  Time.deltaTime * speedFactor * moveAction.ReadValue<Vector2>();

            transform.Translate(movement.x, movement.y, 0F);
        }
    }
}
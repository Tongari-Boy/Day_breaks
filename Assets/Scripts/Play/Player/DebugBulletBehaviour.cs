using Enemy;
using UnityEngine;

public class DebugBulletBehaviour : MonoBehaviour
{
    public void OnCollisionStay2D(Collision2D collision)
    {
        EnemyMovement enemyMovement = collision.gameObject.GetComponentInChildren<EnemyMovement>();

        Debug.Log("Enter");

        if (enemyMovement)
        {
            Debug.Log("Hit");
            enemyMovement.OnDamagedByPlayer(50);
        }
    }
}

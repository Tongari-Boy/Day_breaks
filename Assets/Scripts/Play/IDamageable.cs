/// <summary>
/// Enemyからのダメージを受けることができるオブジェクトのインターフェース
/// Castle,DecoyFortess,Playerなどが実装する想定
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// ダメージを受けるオブジェクトのTransform
    /// </summary>
    UnityEngine.Transform transform { get; }

    /// <summary>
    /// ダメージを受けた時の処理
    /// </summary>
    /// <param name="damageAmount">受けたダメージ量</param>
    void OnDamaged(int damageAmount);
    
}
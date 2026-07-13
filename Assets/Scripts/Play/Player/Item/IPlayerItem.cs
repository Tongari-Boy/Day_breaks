namespace Player.Item
{
    /// <summary>
    /// プレイヤーが使用するアイテムを定義する
    /// </summary>
    public interface IPlayerItem
    {
        /// <summary>
        /// <para>アイテムのID</para>
        /// <para>これはアイテムの識別に使用するので、他のアイテムと重複してはならない</para>
        /// </summary>
        string Id { get; }

        /// <summary>
        /// <para>アイテムの名前</para>
        /// <para>UIなどに表示されるのを想定</para>
        /// </summary>
        string Name { get; }

        /// <summary>
        /// <para>アイテムを使用したときに実行されるアクション</para>
        /// <para>PlayerContextによって、プレイヤーの移動速度などの情報を取得、設定できる</para>
        /// </summary>
        void Use(IPlayerContext playerContext);
    }
}

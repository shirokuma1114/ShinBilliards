using ExitGames.Client.Photon;
using Photon.Realtime;

public static class PlayerPropertiesExtensions
{
    private const string HpKey = "HP";
    private const string PrefabNameKey = "PrefabName";
    private const string MessageKey = "Message";

    //private const string AttackKey = "Attack";

    private static readonly Hashtable propsToSet = new Hashtable();

    public static string GetPrefabName(this Player player)
    {
        return (player.CustomProperties[PrefabNameKey] is string prefabName) ? prefabName : null;
    }

    // プレイヤーのHPを取得する
    public static int GetHP(this Player player)
    {
        return (player.CustomProperties[HpKey] is int hp) ? hp : 0;
    }

    // プレイヤーのメッセージを取得する
    public static string GetMessage(this Player player)
    {
        return (player.CustomProperties[MessageKey] is string message) ? message : string.Empty;
    }

    //// プレイヤーのHPを設定する
    //public static void SetHP(this Player player, int hp)
    //{
    //    propsToSet[HpKey] = hp;
    //    player.SetCustomProperties(propsToSet);
    //    propsToSet.Clear();
    //}

    // プレイヤーのスコアを減算する
    public static void Damage(this Player player, int value)
    {
        propsToSet[HpKey] = player.GetHP() - value;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    // プレイヤーのメッセージを設定する
    public static void SetMessage(this Player player, string message)
    {
        propsToSet[MessageKey] = message;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    // プレイヤーのプレハブ名を設定する
    public static void SetPrefabName(this Player player, string prefabName)
    {
        if (!player.IsLocal) return;

        propsToSet[PrefabNameKey] = prefabName;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}
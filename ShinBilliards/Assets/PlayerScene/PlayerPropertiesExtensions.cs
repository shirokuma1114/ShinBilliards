using ExitGames.Client.Photon;
using Photon.Realtime;

public static class PlayerPropertiesExtensions
{
    private const string HpKey = "HP";
    private const string MessageKey = "Message";

    //private const string AttackKey = "Attack";

    private static readonly Hashtable propsToSet = new Hashtable();

    // �v���C���[��HP���擾����
    public static int GetHP(this Player player)
    {
        return (player.CustomProperties[HpKey] is int hp) ? hp : 0;
    }

    // �v���C���[�̃��b�Z�[�W���擾����
    public static string GetMessage(this Player player)
    {
        return (player.CustomProperties[MessageKey] is string message) ? message : string.Empty;
    }

    //// �v���C���[��HP��ݒ肷��
    //public static void SetHP(this Player player, int hp)
    //{
    //    propsToSet[HpKey] = hp;
    //    player.SetCustomProperties(propsToSet);
    //    propsToSet.Clear();
    //}

    // �v���C���[�̃X�R�A�����Z����
    public static void Damage(this Player player, int value)
    {
        propsToSet[HpKey] = player.GetHP() - value;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    // �v���C���[�̃��b�Z�[�W��ݒ肷��
    public static void SetMessage(this Player player, string message)
    {
        propsToSet[MessageKey] = message;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}
using Photon.Pun;

public class CPhotonEffect : MonoBehaviourPunCallbacks
{
    // エフェクト終了時に呼ばれる
    void OnParticleSystemStopped()
    {
        if (!photonView.AmOwner) return;
        PhotonNetwork.Destroy(gameObject);
    }
}

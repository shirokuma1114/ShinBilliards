using Photon.Pun;

public class CPhotonEffect : MonoBehaviourPunCallbacks
{
    // �G�t�F�N�g�I�����ɌĂ΂��
    void OnParticleSystemStopped()
    {
        if (!photonView.AmOwner) return;
        PhotonNetwork.Destroy(gameObject);
    }
}

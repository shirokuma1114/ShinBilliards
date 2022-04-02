
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CPlayerNameUI : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _nameText = null;

    [SerializeField]
    private Text _meIcon = null;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            _nameText.color = Color.yellow;
            _meIcon.color = Color.yellow;
        }
        else
        {
            _nameText.color = Color.white;
            _meIcon.color = Color.white;
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        _nameText.text = photonView.Owner.NickName;
        
    }

    void LateUpdate()
    {
        //Å@ÉJÉÅÉâÇ∆ìØÇ∂å¸Ç´Ç…ê›íË
        transform.rotation = Camera.main.transform.rotation;
    }
}

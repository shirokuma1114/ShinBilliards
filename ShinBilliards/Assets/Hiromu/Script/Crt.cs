using UnityEngine;

[ExecuteAlways]
public class Crt : MonoBehaviour
{
    // �|�X�g�G�t�F�N�g�p�̃V�F�[�_��t�����}�e���A��������
    [SerializeField] private Material m_Material;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, m_Material);
    }
}
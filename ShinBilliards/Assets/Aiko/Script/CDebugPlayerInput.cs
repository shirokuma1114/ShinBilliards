// デバッグ用入力

using UnityEngine;

[System.Serializable]
public class CDebugPlayerInput : IGamePlayerInput
{
    [SerializeField] KeyCode _actionKey = KeyCode.E;

    [SerializeField] KeyCode _lLeftKey = KeyCode.A;
    [SerializeField] KeyCode _lRightKey = KeyCode.D;
    [SerializeField] KeyCode _lUpKey = KeyCode.W;
    [SerializeField] KeyCode _lDownKey = KeyCode.S;

    [SerializeField] KeyCode _rLeftKey = KeyCode.J;
    [SerializeField] KeyCode _rRightKey = KeyCode.L;
    [SerializeField] KeyCode _rUpKey = KeyCode.I;
    [SerializeField] KeyCode _rDownKey = KeyCode.K;

    public override bool Action()
    {
        return Input.GetKeyDown(_actionKey);
    }

    public override Vector2 Move()
    {
        Vector2 dir = Vector2.zero;
        if (Input.GetKey(_lLeftKey))
        {
            dir -= Vector2.right;
        }
        if (Input.GetKey(_lRightKey))
        {
            dir += Vector2.right;
        }
        if (Input.GetKey(_lUpKey))
        {
            dir += Vector2.up;
        }
        if (Input.GetKey(_lDownKey))
        {
            dir -= Vector2.up;
        }
        return dir;
    }

    public override Vector2 DirMove()
    {
        Vector2 dir = Vector2.zero;
        if (Input.GetKey(_rLeftKey))
        {
            dir -= Vector2.right;
        }
        if (Input.GetKey(_rRightKey))
        {
            dir += Vector2.right;
        }
        if (Input.GetKey(_rUpKey))
        {
            dir += Vector2.up;
        }
        if (Input.GetKey(_rDownKey))
        {
            dir -= Vector2.up;
        }
        return dir;
    }
}

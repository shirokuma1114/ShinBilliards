
using UnityEngine;

public abstract class IGamePlayerInput : MonoBehaviour
{
    public abstract bool Action();

    public abstract Vector2 Move();

    public abstract Vector2 DirMove();
}

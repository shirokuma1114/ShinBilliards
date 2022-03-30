using UnityEngine;

public class UIPosition : MonoBehaviour
{

    [SerializeField]
    private Transform targetTfm;

    private RectTransform myRectTfm;
    [SerializeField]
    private Vector3 offset = new Vector3(0, 1.5f, 0);

    void Start()
    {
        myRectTfm = GetComponent<RectTransform>();
    }

    void Update()
    {
        myRectTfm.position
            = RectTransformUtility.WorldToScreenPoint(Camera.main, targetTfm.position + offset);
    }
}
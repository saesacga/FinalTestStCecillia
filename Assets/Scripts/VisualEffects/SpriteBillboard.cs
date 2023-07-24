using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] private bool activateBillboard;
    void LateUpdate()
    {
        if (activateBillboard)
        {
            transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        }
    }
}

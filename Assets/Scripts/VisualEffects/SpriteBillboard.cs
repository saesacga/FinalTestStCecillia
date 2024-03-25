using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] private bool activateBillboard;
    [SerializeField] private bool _activateVerticalBillboard;
    void LateUpdate()
    {
        if (activateBillboard)
        {
            if (_activateVerticalBillboard)
            {
                transform.rotation = Camera.main.transform.rotation;
            }
            else
            {
                transform.LookAt(Camera.main.transform.position, -Vector3.up);
                transform.localEulerAngles = new Vector3(0,transform.localEulerAngles.y,0);
            }
        }
    }
}

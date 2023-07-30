using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] private bool activateBillboard;
    void LateUpdate()
    {
        if (activateBillboard)
        {
            transform.LookAt(Camera.main.transform.position, -Vector3.up);
            transform.localEulerAngles = new Vector3(0,transform.localEulerAngles.y,0);
            
            //transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        }
    }
}

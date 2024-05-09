using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameExpo : MonoBehaviour
{
    private Animator _animator;

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            _animator.SetBool("startGame", true);
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}

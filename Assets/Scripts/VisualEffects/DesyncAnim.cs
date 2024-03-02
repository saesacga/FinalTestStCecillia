using Random = UnityEngine.Random;
using UnityEngine;

public class DesyncAnim : MonoBehaviour
{
    private Animator anim;
    private float frameOffset; 
    [SerializeField] private string stateName;
    private const int ALL_LAYERS = 0;

    private void OnEnable() 
    { 
        frameOffset = Random.Range(0f, 1f);
        anim = GetComponent<Animator>(); 
        anim.Play(stateName, ALL_LAYERS, frameOffset);
    }
}

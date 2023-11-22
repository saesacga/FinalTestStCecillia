using UnityEngine;
using UnityEngine.Playables;

public class LightRay : MonoBehaviour
{
    #region Members

    [SerializeField] private Collider _invisibleWall;
    [SerializeField] private Animator _cinemachineBlend;
    [HideInInspector] public int _starsCount;
    [HideInInspector] public int _starsInPosition;
    private int _nextPosCount;
    public int _starsRequired;
    private bool _starsInPositionHasRun;
    private Animator _lightBeamAnimator;
    private PlayableDirector _lightBeamTimeline;

    #endregion
    
    private void OnEnable()
    {
        _lightBeamAnimator = GetComponent<Animator>();
        _lightBeamTimeline = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if (_starsRequired == _starsInPosition && _starsInPositionHasRun == false)
        {
            _lightBeamAnimator.SetBool("activateRay", true);
            _lightBeamTimeline.Play();
            _invisibleWall.isTrigger = true;
            _starsInPositionHasRun = true;
        }
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            _nextPosCount++;
            _lightBeamAnimator.SetInteger("nextPosition", _nextPosCount);
        }
    }
    
    private void ControlCameraBlends(int moving)
    {
        if (moving == 0) //El rayo se está moviendo
        {
            StartCoroutine(ActionMapReference.ActivateLooking(false));
            _cinemachineBlend.Play("LightBeamCam");
        }
        else //El rayo se dejó de mover
        {
            _cinemachineBlend.Play("POVCam");
            StartCoroutine(ActionMapReference.ActivateLooking(true));
        }
    }
}

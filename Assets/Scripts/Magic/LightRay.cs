using UnityEngine;
using UnityEngine.Playables;

public class LightRay : MonoBehaviour
{
    #region Members

    [SerializeField] private Collider _invisibleWall;
    [SerializeField] private GameObject _personajes;

    [HideInInspector] public int _starsCount;
    [HideInInspector] public int _starsInPosition;
    //private bool _nextPos;
    private int _nextPos;
    private bool _canGoToNextPos;
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
    
    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_canGoToNextPos)
            {
                _canGoToNextPos = false;
                _nextPos++;
                _lightBeamAnimator.SetInteger("nextPosition", _nextPos);
                Debug.Log(_nextPos);
            }
        }
        
    }
    
    private void ControlCameraBlends(int moving)
    {
        if (moving == 0) //El rayo se está moviendo
        {
            //_nextPos = false;
            _canGoToNextPos = false;
            StartCoroutine(ActionMapReference.ActivateLooking(false, "LightBeamCam"));
        }
        else //El rayo se dejó de mover
        {
            _canGoToNextPos = true;
            StartCoroutine(ActionMapReference.ActivateLooking(true, "POVCam"));
        }
    }

    private void ActivateCharacters()
    {
        _personajes.SetActive(true);
    }
}

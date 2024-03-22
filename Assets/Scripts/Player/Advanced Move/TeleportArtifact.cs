using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TrajectoryPredictor))]
public class TeleportArtifact : MonoBehaviour
{
    #region Members

    TrajectoryPredictor trajectoryPredictor;

    [SerializeField] private Rigidbody _objectToThrow;

    [SerializeField, Range(0.0f, 50.0f)] private float _force;

    [SerializeField] private Transform _startPosition;

    [SerializeField] private static GameObject _teleportProjectile;

    [SerializeField] private GameObject _player;

    [SerializeField] private Image _allowedToTeleportUI;

    #endregion

    void OnEnable()
    {
        _distortionMat.SetFloat("_DistortionStrenght", 0f);
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();

        if (_startPosition == null)
            _startPosition = transform;
    }

    void Update()
    {
        if (TeleportProjectile.allowedToTeleport)
        {
            _allowedToTeleportUI.color = Color.green;
        }
        else
        {
            _allowedToTeleportUI.color = Color.red;
        }

        if (ActionMapReference.playerMap.MovimientoAvanzado.Trayectoria.IsPressed())
        {
            Predict();
        }

        if (ActionMapReference.playerMap.MovimientoAvanzado.Trayectoria.WasReleasedThisFrame())
        {
            ThrowObject();
            trajectoryPredictor.hitMarker.gameObject.SetActive(false);
            GetComponent<LineRenderer>().positionCount = 0;
        }

        if (ActionMapReference.playerMap.MovimientoAvanzado.ActivarTeleport.WasPerformedThisFrame() && _allowTP)
        {
            StartCoroutine(TeleportRoutine());
        }
    }

    void Predict()
    {
        trajectoryPredictor.PredictTrajectory(ProjectileData());
    }

    ProjectileProperties ProjectileData()
    {
        ProjectileProperties properties = new ProjectileProperties();
        Rigidbody r = _objectToThrow.GetComponent<Rigidbody>();

        properties.direction = _startPosition.forward;
        properties.initialPosition = _startPosition.position;
        properties.initialSpeed = _force;
        properties.mass = r.mass;
        properties.drag = r.drag;

        return properties;
    }
    
    private void ThrowObject()
    {
        if (trajectoryPredictor.allowThrow)
        {
            if (_teleportProjectile != null)
            {
                Destroy(_teleportProjectile);
                TeleportProjectile.allowedToTeleport = false;
                TeleportProjectile.animatorCanChangeValues = false;
            }

            GetComponentInChildren<Animator>().Play("AmaShoot");
            Rigidbody thrownObject = Instantiate(_objectToThrow, _startPosition.position, Quaternion.identity);
            _teleportProjectile = thrownObject.gameObject;
            thrownObject.AddForce(_startPosition.forward * _force, ForceMode.Impulse);
        }
        else
        {
            Debug.Log("Ya hay un proyectil creado o no estás apuntando a la superficie correcta");
        }
    }

    [SerializeField] private Material _distortionMat;
    [SerializeField] private float _distortionTarget;
    private float _distortionLerp;
    private bool _allowTP = true;
    
    private IEnumerator TeleportRoutine()
    {
        if (_teleportProjectile == null || TeleportProjectile.allowedToTeleport == false)
        {
            yield break;
        } //No hay proyectil o no está preparado para el teleport

        _allowTP = false;
        while (_distortionMat.GetFloat("_DistortionStrenght") < _distortionTarget)
        {
            _distortionLerp = Mathf.MoveTowards(_distortionMat.GetFloat("_DistortionStrenght"), _distortionTarget, 1f * Time.deltaTime);
            _distortionMat.SetFloat("_DistortionStrenght", _distortionLerp);
            yield return null; 
        }
        
        TeleportProjectile.allowedToTeleport = false;
        TeleportProjectile.animatorCanChangeValues = false;
        _player.GetComponent<CharacterController>().enabled = false;
        _player.transform.position = _teleportProjectile.transform.position;
        _player.GetComponent<CharacterController>().enabled = true;
        Destroy(_teleportProjectile);
        
        while (_distortionMat.GetFloat("_DistortionStrenght") > 0f)
        {
            _distortionLerp = Mathf.MoveTowards(_distortionMat.GetFloat("_DistortionStrenght"), 0f, 1f * Time.deltaTime); 
            _distortionMat.SetFloat("_DistortionStrenght", _distortionLerp);
            yield return null; 
        }
        _allowTP = true;
    }
}
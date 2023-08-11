using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    #region Members
    LineRenderer trajectoryLine;
    [SerializeField] private Transform _startPosition;
    [Tooltip("The marker will show where the projectile will hit")]
    public Transform hitMarker;
    [SerializeField, Range(10, 100), Tooltip("The maximum number of points the LineRenderer can have")]
    int maxPoints = 50;
    [SerializeField, Range(0.01f, 0.5f), Tooltip("The time increment used to calculate the trajectory")]
    float increment = 0.025f;
    [SerializeField, Range(1.05f, 2f), Tooltip("The raycast overlap between points in the trajectory, this is a multiplier of the length between points. 2 = twice as long")]
    float rayOverlap = 1.1f;

    [HideInInspector] public bool allowThrow;
    [SerializeField] private Gradient _canThrowColor;
    [SerializeField] private Gradient _cannotThrowColor;

    [SerializeField] private GravityAttractor _gravityAttractor;
    #endregion

    private void Start()
    {
        if (trajectoryLine == null)
            trajectoryLine = GetComponent<LineRenderer>();

        SetTrajectoryVisible(true);
    }

    public void PredictTrajectory(ProjectileProperties projectile)
    {
        Vector3 velocity = projectile.initialSpeed / projectile.mass * projectile.direction;
        Vector3 position = projectile.initialPosition;
        Vector3 nextPosition;
        float overlap;
        
        UpdateLineRender(maxPoints, (0, position)); 
        
        for (int i = 1; i < maxPoints; i++)
        {
            // Estimate velocity and update next predicted position
            velocity = CalculateNewVelocity(velocity, projectile.drag, increment);
            nextPosition = position + velocity * increment;

            // Overlap our rays by small margin to ensure we never miss a surface
            overlap = Vector3.Distance(position, nextPosition) * rayOverlap;

            //When hitting a surface we want to show the surface marker and stop updating our line
            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap))
            {
                UpdateLineRender(i, (i - 1, hit.point));
                MoveHitMarker(hit);
                if (hit.collider.CompareTag("TeleportSurface") && TeleportArtifact._canThrow)
                {
                    trajectoryLine.colorGradient = _canThrowColor;
                    allowThrow = true;
                }
                break;
            }

            //If nothing is hit, continue rendering the arc without a visual marker
            hitMarker.gameObject.SetActive(false);
            position = nextPosition;
            trajectoryLine.colorGradient = _cannotThrowColor;
            allowThrow = false;
            UpdateLineRender(maxPoints, (i, position)); //Unneccesary to set count here, but not harmful
        }
    }
    
    private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos)
    {
        trajectoryLine.positionCount = count; 
        trajectoryLine.SetPosition(pointPos.point, pointPos.pos);
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        //velocity += Physics.gravity * increment; RESTAURAR IF NECESSARY
        Vector3 gravityUp = (_startPosition.position - _gravityAttractor.transform.position).normalized;
        velocity += gravityUp * (_gravityAttractor.gravity * increment);

        velocity *= Mathf.Clamp01(1f - drag * increment);
        return velocity;
    }

    private void MoveHitMarker(RaycastHit hit)
    {
        hitMarker.gameObject.SetActive(true);

        // Offset marker from surface
        float offset = 0.025f;
        hitMarker.position = hit.point + hit.normal * offset;
        hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
    }

    public void SetTrajectoryVisible(bool visible)
    {
        trajectoryLine.enabled = visible;
        hitMarker.gameObject.SetActive(visible);
    }
}
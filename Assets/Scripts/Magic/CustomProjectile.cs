using UnityEngine;

public class CustomProjectile : MonoBehaviour
{
    #region Declarations

    public Rigidbody rigidBody;
    public float maxLifeTime;
    [SerializeField] private GameObject _particle;
    [SerializeField] private AudioClip[] _destroyMagicSounds;

    #endregion
    
    private void Update()
    {
        maxLifeTime -= Time.deltaTime;
        if (maxLifeTime <= 0) { Destroyer(); }
    }
    
    private void Destroyer()
    {
        _particle = Instantiate(_particle, transform.position, Quaternion.identity);
        SoundManager.PlaySoundOneShot(_destroyMagicSounds);
        float totalDuration = _particle.GetComponent<ParticleSystem>().main.duration + _particle.GetComponent<ParticleSystem>().main.startLifetimeMultiplier;
        Destroy(_particle, totalDuration);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
        {
            Destroyer();
        }
    }
}

using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _footstepSFX;
    [SerializeField] private AudioSource _landingSFX;
    [SerializeField] private AudioSource _punchSFX;
    [SerializeField] private AudioSource _glideSFX;
    [SerializeField] private float _footstepCooldown = 0.3f;
    private float _lastFootstepTime = -999f;

    private void PlayFootstepSFX()
    {
        if (Time.time - _lastFootstepTime < _footstepCooldown) return;

        _footstepSFX.volume = Random.Range(0.8f, 1f);
        _footstepSFX.pitch = Random.Range(0.8f, 1.5f);
        _footstepSFX.Play();

        _lastFootstepTime = Time.time;
    }

    private void PlayLandingSFX()
    {
        _landingSFX.Play();
    }

    private void PlayPunchSFX()
    {
        _punchSFX.volume = Random.Range(0.8f, 1f);
        _punchSFX.pitch = Random.Range(0.8f, 1.5f);
        _punchSFX.Play();
    }

    public void PlayGlideSFX()
    {
        _glideSFX.Play();
    }
    
    public void StopGlideSFX()
    {
        _glideSFX.Stop();
    }
}
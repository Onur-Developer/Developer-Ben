using DG.Tweening;
using UnityEngine;


public class CameraShake : MonoBehaviour
{
    private Tween _shakeCamera;
    private float _duration;
    private float _strength;
    private int _vibrato;
    private float _randomless = 90;

    public void Shake(string shakeType)
    {
        if(_shakeCamera!=null)
            return;

        switch (shakeType)
        {
            case "Damage":
                _duration = .5f;
                _strength = 10f;
                _vibrato = 20;
                break;
            case "Dagger":
                _duration = .25f;
                _strength = 1f;
                _vibrato = 1;
                break;
            case "Pistol":
                _duration = .25f;
                _strength = 3f;
                _vibrato = 3;
                break;
            case "Machine Gun":
                _duration = .35f;
                _strength = 2f;
                _vibrato = 2;
                break;
            case "Bow":
                _duration = .25f;
                _strength = 2f;
                _vibrato = 2;
                break;
            case "Mini Gun":
                _duration = .35f;
                _strength = 2f;
                _vibrato = 2;
                break;
            case "Laser Gun":
                _duration = .45f;
                _strength = 2f;
                _vibrato = 3;
                break;
            case "Bomba":
                _duration = 1f;
                _strength = 20f;
                _vibrato = 18;
                break;
            default:
                return;
        }
        _shakeCamera = transform.DOShakeRotation(_duration, _strength, _vibrato,_randomless)
                .OnComplete((() =>
                {
                    _shakeCamera = null;
                }));
    }
}

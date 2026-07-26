using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV : MonoBehaviour
{
    [SerializeField]
    private Material _tvMaterial;

    [SerializeField]
    private Texture2D[] _footageTextures;

    [SerializeField]
    private float _totalDuration = 5f;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _audioClip;

    [Header("Shader Property Names")]
    [SerializeField]
    private string _mainTexturePropertyName = "_MainTex";
    [SerializeField]
    private string _emissionTexturePropertyName = "_EmissionMap";
    [SerializeField]
    private string _emissionKeyword = "_EMISSION";

    [Header("Audio Fade Out Settings")]
    [SerializeField]
    private float _audioFadeDuration = 1f;

    private Coroutine _footageRoutine;
    private float _initialAudioVolume = 1f;

    private void Awake()
    {
        if (_audioSource != null)
        {
            _initialAudioVolume = _audioSource.volume;
        }

        ClearTVMaterial();
    }

    private void OnDisable()
    {
        if (_footageRoutine != null)
        {
            StopCoroutine(_footageRoutine);
            _footageRoutine = null;
        }

        ClearTVMaterial();
    }

    public void ShowFootageAnimation()
    {
        if (_footageRoutine != null)
        {
            StopCoroutine(_footageRoutine);
        }

        _footageRoutine = StartCoroutine(FootageSequenceRoutine());
    }

    private IEnumerator FootageSequenceRoutine()
    {
        if (_audioSource != null && _audioClip != null)
        {
            _audioSource.volume = _initialAudioVolume;
            _audioSource.clip = _audioClip;
            _audioSource.Play();
        }

        if (_footageTextures != null && _footageTextures.Length > 0)
        {
            float timePerImage = _totalDuration / _footageTextures.Length;

            for (int i = 0; i < _footageTextures.Length; i++)
            {
                ApplyTextureToMaterial(_footageTextures[i]);
                yield return new WaitForSeconds(timePerImage);
            }
        }
        else
        {
            yield return new WaitForSeconds(_totalDuration);
        }

        ClearTVMaterial();

        if (_audioSource != null && _audioSource.isPlaying)
        {
            float startVolume = _audioSource.volume;
            float elapsedTime = 0f;

            while (elapsedTime < _audioFadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _audioFadeDuration);
                _audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            _audioSource.Stop();
            _audioSource.volume = _initialAudioVolume;
        }

        _footageRoutine = null;
    }

    private void ApplyTextureToMaterial(Texture2D tex)
    {
        if (_tvMaterial == null) return;

        if (_tvMaterial.HasProperty(_mainTexturePropertyName))
        {
            _tvMaterial.SetTexture(_mainTexturePropertyName, tex);
        }

        if (_tvMaterial.HasProperty(_emissionTexturePropertyName))
        {
            _tvMaterial.SetTexture(_emissionTexturePropertyName, tex);
        }

        _tvMaterial.EnableKeyword(_emissionKeyword);
    }

    private void ClearTVMaterial()
    {
        if (_tvMaterial == null) return;

        if (_tvMaterial.HasProperty(_mainTexturePropertyName))
        {
            _tvMaterial.SetTexture(_mainTexturePropertyName, null);
        }

        if (_tvMaterial.HasProperty(_emissionTexturePropertyName))
        {
            _tvMaterial.SetTexture(_emissionTexturePropertyName, null);
        }

        _tvMaterial.DisableKeyword(_emissionKeyword);
    }
}
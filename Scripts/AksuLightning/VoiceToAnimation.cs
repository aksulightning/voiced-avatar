// voiced-avatar, VoiceToAnimation.cs
// Script written by Aksu Lightning. The license of the project is "MIT".
// Script works in Unity LTS 2021.3.x or higher.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class VoiceToAnimation : MonoBehaviour
{
    // Complex settings
    [Tooltip("Set a avatar to play mouth moving animation, please animate the avatar first.")]
    public GameObject _avatar;
    private Animation anim;
    [Tooltip("Insert here Animation Trigger Name to animate and it need to be a Trigger on Animator.")]
    public string _animationTriggerName;
    [Tooltip("Audio Source")]
    public AudioSource _audioSource;
    [Tooltip("Microphone")]
    public string _selectedDevice;
    [Tooltip("Do not change or set a value.")]
    public float _spectrumCount;
    private float spectrum;
    [Tooltip("Default value is -11. This adjusts the sensitivity of the Spectrum.")]
    [Range(-1, -100)]public float _limitedSpectrum = -11;
    private Animator _anima;
    

    // Start microphone before void Update
    void Start()
    {

        // Get Animator-Component from a avatar and the audio source from microphone

        _anima = _avatar.GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource> ();
        _selectedDevice = Microphone.devices[0].ToString();
        _audioSource.clip = Microphone.Start(_selectedDevice, true, 10, AudioSettings.outputSampleRate);    // 41100 or AudioSettings.outputSampleRate
        _audioSource.loop = true;

        while (!(Microphone.GetPosition(_selectedDevice) > 0)){}
        _audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {

        // Sleep for 1 millisecond
        // System.Threading.Thread.Sleep(1);

        float[] spectrum = new float[256];

        // Analyse and get spectrum data from the audio
        _audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        // Count spectrum length.

        for (int i = 1; i < spectrum.Length - 10; i++)
        {
           // Use for debug purposes
           //Debug.Log("Spectrum count: " + Mathf.Log(spectrum[i]));
           _spectrumCount = Mathf.Log(spectrum[i]);
        }

        // Check spectrum count for playing a animation

        if (_spectrumCount < _limitedSpectrum)
        {
            _anima.ResetTrigger(_animationTriggerName);
        }
        else
        {
            _anima.SetTrigger(_animationTriggerName);
        }


        // Hidden settings for adjusting the sensitivity of the Spectrum

        // Lower negative

        if (Input.GetKeyDown(KeyCode.I))
        {
            _limitedSpectrum = _limitedSpectrum + 1;
            // Block bypass the limits
            if (_limitedSpectrum.ToString() == "0")
            {
            _limitedSpectrum = -99;
            }
        }

        // Go back to negative

        if (Input.GetKeyDown(KeyCode.O))
        {
            _limitedSpectrum = _limitedSpectrum + -1;
            // Block bypass the limits
            if (_limitedSpectrum.ToString() == "-100")
            {
            _limitedSpectrum = -1;
            }
        }

    }
}

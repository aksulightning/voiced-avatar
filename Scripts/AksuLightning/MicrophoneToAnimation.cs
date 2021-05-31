// I recommend use the code in Unity LTS 2018.4.x.
// Don't forget import TextMeshPro.
// The code written by Aksu Lightning.
// The license of the code is "MIT".
// "I want keep the code simple as possible." - Aksu Lightning

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Please import Text Mesh Pro into your project before for using script.

[RequireComponent (typeof (AudioSource))]
public class MicrophoneToAnimation : MonoBehaviour
{
    // Complex settings
    [Tooltip("Set a avatar to play mouth moving animation, please animate the avatar first.")]
    public GameObject _avatar;
    private Animation anim;
    [Tooltip("Insert here Animation Bool Name to animate and it need to be a Bool on Animator.")]
    public string _animationBoolName;
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
    [Tooltip("Make a Text Mesh Pro 3D.")]
    [SerializeField]public TextMeshPro textmeshpro;
    

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
            _anima.SetBool(_animationBoolName, false);
        }
        else
        {
            _anima.SetBool(_animationBoolName, true);
        }


        // Hidden settings for adjusting the sensitivity of the Spectrum

        // Lower negative

        if (Input.GetKeyDown(KeyCode.I))
        {
            _limitedSpectrum = _limitedSpectrum + 1;
            textmeshpro.text = _limitedSpectrum.ToString();
            // Block bypass the limits
            if (_limitedSpectrum.ToString() == "0")
            {
            _limitedSpectrum = -99;
            textmeshpro.text = _limitedSpectrum.ToString();   
            }
        }

        // Go back to negative

        if (Input.GetKeyDown(KeyCode.O))
        {
            _limitedSpectrum = _limitedSpectrum + -1;
            textmeshpro.text = _limitedSpectrum.ToString();
            // Block bypass the limits
            if (_limitedSpectrum.ToString() == "-100")
            {
            _limitedSpectrum = -1;
            textmeshpro.text = _limitedSpectrum.ToString();   
            }
        }

        // Hide Text Mesh Pro

        if (Input.GetKeyDown(KeyCode.P))
        {
            textmeshpro.text = "";
        }

    }
}

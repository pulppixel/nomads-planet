using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarAudio : NetworkBehaviour
    {
        // 이 스크립트는 자동차의 현재 속성을 읽고 그에 따라 사운드를 재생합니다.
        // 엔진 사운드는 루프 및 피치되는 단순한 단일 클립이거나
        // 엔진의 음색을 나타내는 네 개의 클립이 크로스 페이드 블렌딩될 수도 있습니다.
        // 엔진 음색을 나타내는 네 개의 클립을 혼합할 수도 있습니다.

        // 엔진 클립은 모두 상승하거나 하강하지 않고 일정한 피치여야 합니다.

        // 4채널 엔진 크로스페이딩을 사용할 때는 4개의 클립이 있어야 합니다:
        // lowAccelClip : 스로틀이 열려 있는 저회전 엔진(즉, 매우 낮은 속도에서 가속 시작)
        // highAccelClip : 스로틀이 열린 상태에서 높은 회전수의 엔진 (즉, 가속 중이지만 거의 최대 속도에 도달한 상태)
        // lowDecelClip : 엔진이 낮은 회전수에서 스로틀이 최소로 열린 상태 (즉, 매우 낮은 속도에서 공회전 또는 엔진 제동)
        // highDecelClip : 스로틀을 최소화한 상태에서 고회전 엔진 (즉, 매우 빠른 속도에서 엔진 제동)

        // 적절한 크로스 페이딩을 위해 클립 피치는 모두 일치해야 하며, 저음과 고음 사이에 옥타브 오프셋이 있어야 합니다.

        private void Start()
        {
            if (!IsLocalPlayer)
            {
                this.enabled = false;
            }
        }

        public enum EngineAudioOptions // Options for the engine audio
        {
            Simple, // Simple style audio
            FourChannel // four Channel audio
        }

        public EngineAudioOptions
            engineSoundStyle = EngineAudioOptions.FourChannel; // Set the default audio options to be four channel

        public AudioClip lowAccelClip; // Audio clip for low acceleration
        public AudioClip lowDecelClip; // Audio clip for low deceleration
        public AudioClip highAccelClip; // Audio clip for high acceleration
        public AudioClip highDecelClip; // Audio clip for high deceleration
        public float pitchMultiplier = 1f; // Used for altering the pitch of audio clips
        public float lowPitchMin = 1f; // The lowest possible pitch for the low sounds
        public float lowPitchMax = 6f; // The highest possible pitch for the low sounds
        public float highPitchMultiplier = 0.25f; // Used for altering the pitch of high sounds
        public float maxRolloffDistance = 500; // The maximum distance where rollof starts to take place
        public float dopplerLevel = 1; // The mount of doppler effect used in the audio
        public bool useDoppler = true; // Toggle for using doppler

        private AudioSource m_LowAccel; // Source for the low acceleration sounds
        private AudioSource m_LowDecel; // Source for the low deceleration sounds
        private AudioSource m_HighAccel; // Source for the high acceleration sounds
        private AudioSource m_HighDecel; // Source for the high deceleration sounds
        private bool m_StartedSound; // flag for knowing if we have started sounds
        private CarController m_CarController; // Reference to car we are controlling


        private void StartSound()
        {
            // get the carcontroller ( this will not be null as we have require component)
            m_CarController = GetComponent<CarController>();

            // setup the simple audio source
            m_HighAccel = SetUpEngineAudioSource(highAccelClip);

            // if we have four channel audio setup the four audio sources
            if (engineSoundStyle == EngineAudioOptions.FourChannel)
            {
                m_LowAccel = SetUpEngineAudioSource(lowAccelClip);
                m_LowDecel = SetUpEngineAudioSource(lowDecelClip);
                m_HighDecel = SetUpEngineAudioSource(highDecelClip);
            }

            // flag that we have started the sounds playing
            m_StartedSound = true;
        }


        private void StopSound()
        {
            //Destroy all audio sources on this object:
            foreach (var source in GetComponents<AudioSource>())
            {
                Destroy(source);
            }

            m_StartedSound = false;
        }


        // Update is called once per frame
        private void Update()
        {
            // get the distance to main camera
            float camDist = (Camera.main!.transform.position - transform.position).sqrMagnitude;

            // stop sound if the object is beyond the maximum roll off distance
            if (m_StartedSound && camDist > maxRolloffDistance * maxRolloffDistance)
            {
                StopSound();
            }

            // start the sound if not playing and it is nearer than the maximum distance
            if (!m_StartedSound && camDist < maxRolloffDistance * maxRolloffDistance)
            {
                StartSound();
            }

            if (m_StartedSound)
            {
                // The pitch is interpolated between the min and max values, according to the car's revs.
                float pitch = ULerp(lowPitchMin, lowPitchMax, m_CarController.Revs);

                // clamp to minimum pitch (note, not clamped to max for high revs while burning out)
                pitch = Mathf.Min(lowPitchMax, pitch);

                if (engineSoundStyle == EngineAudioOptions.Simple)
                {
                    // for 1 channel engine sound, it's oh so simple:
                    m_HighAccel.pitch = pitch * pitchMultiplier * highPitchMultiplier;
                    m_HighAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
                    m_HighAccel.volume = 1;
                }
                else
                {
                    // for 4 channel engine sound, it's a little more complex:

                    // adjust the pitches based on the multipliers
                    m_LowAccel.pitch = pitch * pitchMultiplier;
                    m_LowDecel.pitch = pitch * pitchMultiplier;
                    m_HighAccel.pitch = pitch * highPitchMultiplier * pitchMultiplier;
                    m_HighDecel.pitch = pitch * highPitchMultiplier * pitchMultiplier;

                    // get values for fading the sounds based on the acceleration
                    float accFade = Mathf.Abs(m_CarController.AccelInput);
                    float decFade = 1 - accFade;

                    // get the high fade value based on the cars revs
                    float highFade = Mathf.InverseLerp(0.2f, 0.8f, m_CarController.Revs);
                    float lowFade = 1 - highFade;

                    // adjust the values to be more realistic
                    highFade = 1 - ((1 - highFade) * (1 - highFade));
                    lowFade = 1 - ((1 - lowFade) * (1 - lowFade));
                    accFade = 1 - ((1 - accFade) * (1 - accFade));
                    decFade = 1 - ((1 - decFade) * (1 - decFade));

                    // adjust the source volumes based on the fade values
                    m_LowAccel.volume = lowFade * accFade;
                    m_LowDecel.volume = lowFade * decFade;
                    m_HighAccel.volume = highFade * accFade;
                    m_HighDecel.volume = highFade * decFade;

                    // adjust the doppler levels
                    m_HighAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
                    m_LowAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
                    m_HighDecel.dopplerLevel = useDoppler ? dopplerLevel : 0;
                    m_LowDecel.dopplerLevel = useDoppler ? dopplerLevel : 0;
                }
            }
        }


        // sets up and adds new audio source to the gane object
        private AudioSource SetUpEngineAudioSource(AudioClip clip)
        {
            // create the new audio source component on the game object and set up its properties
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = 0;
            source.loop = true;

            // start the clip from a random point
            source.time = Random.Range(0f, clip.length);
            source.Play();
            source.minDistance = 5;
            source.maxDistance = maxRolloffDistance;
            source.dopplerLevel = 0;
            return source;
        }


        // unclamped versions of Lerp and Inverse Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value) * from + value * to;
        }
    }
}
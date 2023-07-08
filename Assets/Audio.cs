using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Example : MonoBehaviour {

    [SerializeField] private SoundEffect jumpSound;

    private void Start() {
        jumpSound.Init(gameObject);
    }

    private void Update() {

        jumpSound.Play();
    }
}

[System.Serializable]
public class SoundEffect {

    public AudioClip[] clip;
    [Header("Parameters")]
    public float volume = 1;
    public float minPitch, maxPitch = 1;
    public bool sequential = false;
    public bool overlap = true;

    internal AudioSource source;

    private int clipIndex;

    /// <summary> Ensures host has an AudioSource (Must be called in Start) </summary>
    public void Init(GameObject host) {

        if (clip.Length == 0) {
            Debug.LogError($"Sound Effect on \"{host.name}\" doesn't haven't any audio clips!");
            return;
        }

        source = host.AddComponent<AudioSource>();

        foreach (var s in host.GetComponents<AudioSource>())
            if (s.clip == null) source = s;
        if (source == null) source = host.AddComponent<AudioSource>();

        //source.outputAudioMixerGroup = Audio.i.soundGroup;
    }

    /// <summary> Play the sound effect. </summary>
    public void Play() {

        // return if overlapping matters
        if (!overlap && source.isPlaying) return;

        // get clip
        AudioClip currentClip;
        if (sequential) {
            currentClip = clip[clipIndex % clip.Length];
            clipIndex++;
        } else currentClip = clip[Random.Range(0, clip.Length - 1)];

        // play sound
        source.pitch = Random.Range(minPitch, maxPitch);
        source.PlayOneShot(currentClip);
        source.volume = volume;
    }

    /// <summary> Stop the sound effect. </summary>
    public void Stop() {
        source.Stop();
    }
}

//public class Audio : MonoBehaviour {

//    [System.Serializable]
//    public class SceneMusic {
//        [HideInInspector] public string name;
//        public Object scene;
//        public AudioClip music;
//        public float volume;
//    }

//    [SerializeField] private SceneMusic[] scenes;
//    [SerializeField] internal AudioMixerGroup musicGroup, soundGroup;

//    private void OnValidate() {
//        foreach (SceneMusic m in scenes) m.name = m.scene != null ? m.scene.name : "No Scene :(";
//    }

//    public static Audio i;
//    private AudioSource musicSource;

//    private void Awake() {
//        if (i == null) i = this;
//        //else Destroy(gameObject);
//        //DontDestroyOnLoad(this);
//    }

//    private void Start() {

//        musicSource = gameObject.AddComponent<AudioSource>();
//        musicSource.outputAudioMixerGroup = musicGroup;
//        musicSource.loop = true;

//        UpdateMusic();
//    }

//    private void Update() {

//        float mVol = UIManager.i.musicVolume,
//              sVol = UIManager.i.soundEffectsVolume;
//        var mix = musicGroup.audioMixer;

//        mix.SetFloat("MusicVolume", mVol == 0 ? -80f : Mathf.Log10(mVol) * 20);
//        mix.SetFloat("SoundVolume", sVol == 0 ? -80f : Mathf.Log10(sVol) * 20);
//    }

//    private void UpdateMusic() {
//        string currentScene = SceneManager.GetActiveScene().name;
//        foreach (SceneMusic m in scenes)
//            if (m.scene.name == currentScene) {

//                musicSource.volume = m.volume;
//                if (m.music != musicSource.clip) {
//                    musicSource.clip = m.music;
//                    musicSource.Play();
//                }
//                return;
//            }
//    }
//}

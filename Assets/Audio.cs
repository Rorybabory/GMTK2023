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

        source.outputAudioMixerGroup = Audio.SoundMixerGroup;
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

public class Audio : MonoBehaviour {

    [System.Serializable] public class SceneMusic {

        public string scene;
        public Music[] tracks;

        [System.Serializable] public class Music {
            public AudioClip clip;
            public float volume;
        }
    }   

    [SerializeField] private float musicVolume, soundVolume;
    [SerializeField] private List<SceneMusic> sceneMusic;
    [SerializeField] private AudioMixerGroup musicGroup, soundGroup;

    public static AudioMixerGroup SoundMixerGroup => I.soundGroup;

    private static Audio I;
    private List<AudioSource> musicSources = new();

    public static float MusicVolume {
        get => I.musicVolume;
        set => I.musicVolume = value;
    }
    public static float SoundVolume {
        get => I.soundVolume;
        set => I.soundVolume = value;
    }

    private void Awake() {

        if (I != null) {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(this);
    }

    private void Start() {
        UpdateMusic(SceneManager.GetActiveScene().name);
        SceneManager.activeSceneChanged += (scene1, scene2) => UpdateMusic(scene2.name);
    }

    private void Update() {

        float mVol = musicVolume,
              sVol = soundVolume;
        var mix = musicGroup.audioMixer;

        mix.SetFloat("MusicVolume", mVol == 0 ? -80f : Mathf.Log10(mVol) * 40);
        mix.SetFloat("SoundVolume", sVol == 0 ? -80f : Mathf.Log10(sVol) * 40);
    }

    private void NewMusicSource() {
        var musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.loop = true;
        musicSources.Add(musicSource);
    }

    private void UpdateMusic(string newSceneName) {

        foreach (var source in musicSources) source.Stop();

        var scene = sceneMusic.Find(s => s.scene == newSceneName);
        if (scene == null) return;

        for (int i = 0; i < scene.tracks.Length; i++) {

            if (i == musicSources.Count) NewMusicSource();

            var source = musicSources[i];
            var track = scene.tracks[i];

            source.volume = track.volume;

            if (source.clip == track.clip) continue;
            source.clip = track.clip;
            source.Play();
        }
    }
}
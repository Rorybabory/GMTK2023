using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    [System.Serializable] private class Level {
        public string displayName, sceneName;
    }

    [SerializeField] private SmartCurve menuTransition;
    [SerializeField] private RectTransform menuPosition;
    [SerializeField] private List<Level> levels;
    [SerializeField] private RectTransform levelSelectParent;
    [SerializeField] private GameObject levelSelectButtonPrefab;

    private static List<Level> staticLevels;
    private enum State { main, settings, levelSelect, credits }

    private Dictionary<State, Vector2> menuPositions = new() {
        { State.main, Vector2.zero },
        { State.settings, new(1600, 0) },
        { State.levelSelect, new(-1600, 0) },
        { State.credits, new(0, 900) },
    };

    private static int levelsCompleted;

    private State state = State.main;
    private Vector2 position, start, end;
    
    public static void Load(bool completed) {

        if (completed) {
            int index = staticLevels.FindIndex(l => l.sceneName == SceneManager.GetActiveScene().name);
            if (index != -1 && index + 1 > levelsCompleted)
                levelsCompleted++;
        }

        SceneManager.LoadScene("MainMenu");
    }

    private void Awake() {

        staticLevels = levels;

        foreach (var level in levels.GetRange(0, levelsCompleted + 1)) {
            var button = Instantiate(levelSelectButtonPrefab, levelSelectParent);

            foreach (var textMesh in button.GetComponentsInChildren<TextMeshProUGUI>())
                textMesh.text = level.displayName;

            button.GetComponentInChildren<Button>().onClick.AddListener(() => SceneManager.LoadScene(level.sceneName));
        }
    }

    private void ChangeState(State newState) {
        start = position;
        end = menuPositions[newState];
        state = newState;
        menuTransition.Start();
    }

    private void Update() {
        position = Vector2.LerpUnclamped(start, end, menuTransition.Evaluate());
        menuPosition.localPosition = position;
    }

    public void StartGame() => ChangeState(State.levelSelect);
    public void Settings() => ChangeState(State.settings);
    public void Credits() => ChangeState(State.credits);
    public void Back() => ChangeState(State.main);
    public void MusicVolume(float volume) => Audio.MusicVolume = volume;
    public void SoundVolume(float volume) => Audio.SoundVolume = volume;
}

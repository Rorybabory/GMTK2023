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

    private enum State { main, settings, levelSelect, credits }

    private Dictionary<State, Vector2> menuPositions = new() {
        { State.main, Vector2.zero },
        { State.settings, new(1600, 0) },
        { State.levelSelect, new(-1600, 0) },
        { State.credits, new(0, 900) },
    };

    private State state = State.main;
    private Vector2 position, start, end;

    private void Awake() {

        foreach (var level in levels) {
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

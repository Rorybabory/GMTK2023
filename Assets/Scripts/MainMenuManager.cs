using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {

    [SerializeField] private SmartCurve menuTransition;
    [SerializeField] private RectTransform menuPosition;

    private enum State { main, settings, levelSelect }

    private Dictionary<State, Vector2> menuPositions = new() {
        { State.main, Vector2.zero },
        { State.settings, new(1600, 0) },
        { State.levelSelect, new(-1600, 0) },
    };

    private State state = State.main;
    private Vector2 position, start, end;

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

    public void StartGame() {
        ChangeState(State.levelSelect);
    }

    public void Settings() {
        ChangeState(State.settings);
    }

    public void Back() {
        ChangeState(State.main);
    }

    public void MusicVolume(float volume) => Audio.MusicVolume = volume;
    public void SoundVolume(float volume) => Audio.SoundVolume = volume;
}

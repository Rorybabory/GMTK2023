using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {

    private enum State { main, settings, levelSelect }

    private State state = State.main;

    public void StartGame() {
        state = State.levelSelect;
    }

    public void Settings() {
        state = State.settings;
    }

    public void Back() {
        state = State.main;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class FSM <KeyType>
    {
        protected Dictionary<KeyType, State> States;
        protected State CurrentState;

        public FSM() 
        { 
            States = new();
        }

        public void AddState(KeyType key, State state)
        {
            States.Add(key, state);
        }

        public State GetState(KeyType key)
        {
            return States[key];
        }

        public void SetCurrentState(State state)
        {
            CurrentState?.Exit(); //exit the current state

            CurrentState = state;

            CurrentState?.Enter(); //enter the new state
        }

        public void SetCurrentState(KeyType key)
        {
            SetCurrentState(GetState(key));
        }

        public void Update()
        {
            CurrentState?.Update();
        }
        
        public void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }
    }

    public class State
    {
        public virtual void Enter() { }
        public virtual void Exit() { }

        public virtual void Update() { }
        public virtual void FixedUpdate() { }
    }
}
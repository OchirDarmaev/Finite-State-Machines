using System;
using System.Collections.Generic;
using System.Linq;

namespace FsmTest
{
    public class FsmStack
    {
        private Stack<Action> _stack;

        public FsmStack() => _stack = new Stack<Action>();

        public void Update() => GetCurrentState().Invoke();

        private Action GetCurrentState() => _stack.FirstOrDefault();

        public Action PopState() => _stack.Pop();

        public string GetStateName() => 
            GetCurrentState().Method.Name + " " + _stack.Count.ToString();

        public void PushState(Action state)
        {
            if (GetCurrentState() != state)
                _stack.Push(state);
        }

    }
}
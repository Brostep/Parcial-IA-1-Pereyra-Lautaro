using System;
using System.Collections.Generic;
using UnityEngine;

public class State<Input>
{
	public string Name { get; private set; }
	public event Action OnEnter = delegate { };
	public event Action OnUpdate = delegate { };
	public event Action OnExit = delegate { };

	Dictionary<Input, Transition<Input>> transitions = new Dictionary<Input, Transition<Input>>();

	public State(string name = "NO NAME")
	{
		Name = name;
	}

	public void AddTransition(Input key, Transition<Input> value)
	{
		Debug.Assert(!transitions.ContainsKey(key), "Clave repetida " + key);
		transitions[key] = value;
	}

	public Transition<Input> GetTransition(Input input)
	{
		return transitions[input];
	}

	public bool Feed(Input input, out State<Input> next)
	{
		if (transitions.ContainsKey(input))
		{
			var transition = transitions[input];
			transition.OnTransitionExecute();
			next = transition.TargetState;
			return true;
		}
		next = this;
		return false;
	}

	public void Enter() { OnEnter(); }
	public void Update() { OnUpdate(); }
	public void Exit() { OnExit(); }
}
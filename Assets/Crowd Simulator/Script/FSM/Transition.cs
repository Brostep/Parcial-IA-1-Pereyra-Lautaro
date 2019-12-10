using System;

public class Transition<Input>
{
	public event Action OnTransition = delegate { };
	public State<Input> TargetState { get; private set; }
	public Transition(State<Input> targetState)
	{
		TargetState = targetState;
	}
	public void OnTransitionExecute()
	{
		OnTransition();
	}
}

public class EventFSM<Input>
{
	State<Input> current;

	public EventFSM(State<Input> initial)
	{
		current = initial;
		current.Enter();
	}

	public void Feed(Input input)
	{
		State<Input> newState;

		if (current.Feed(input, out newState))
		{
			current.Exit();
			current = newState;
			newState.Enter();
		}
	}

	public State<Input> Current { get { return current; } }

	public void Update()
	{
		current.Update();
	}
}
public class StateConfigurer<Input>
{
	State<Input> instance;

	public StateConfigurer(State<Input> state)
	{
		instance = state;
	}

	public StateConfigurer<Input> SetTransition(Input input, State<Input> target)
	{
		instance.AddTransition(input, new Transition<Input>(target));
		return this;
	}
}
public static class StateConfigurer
{
	public static StateConfigurer<T> Create<T>(State<T> state)
	{
		return new StateConfigurer<T>(state);
	}
}

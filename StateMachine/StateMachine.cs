public class StateMachine<T> where T : class
{
    private IState<T> currentState;
    private T context;

    public StateMachine(T context)
    {
        this.context = context;
    }

    public void ChangeState(IState<T> newState)
    {
        currentState?.Exit(context);
        currentState = newState;
        currentState.Enter(context);
    }

    public void Update()
    {
        currentState?.Update(context);
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate(context);
    }
}

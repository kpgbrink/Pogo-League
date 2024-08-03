

public interface IState<T>
{
    void Enter(T context);
    void Update(T context);
    void FixedUpdate(T context);
    void Exit(T context);
}
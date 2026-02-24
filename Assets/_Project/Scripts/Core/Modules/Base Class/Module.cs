using System.Collections;
using UnityEngine;

public abstract class Module : MonoBehaviour
{
    public enum State
    {
        Load,
        Attack,
        Used
    }
    public State state = State.Load;
    [SerializeField] protected float attackStateDuration = 10f;
    public void FastForward()
    {
        switch (state)
        {
            case State.Load:
                state = State.Attack;
                StartCoroutine(AttackStateCoroutine());
                break;
            case State.Attack:
                state = State.Used;
                break;
            case State.Used:
                // nothing happens
                break;
        }
        ActByState();
    }
    protected IEnumerator AttackStateCoroutine()
    {
        yield return new WaitForSeconds(attackStateDuration);
        state = State.Used;
        ActByState();
    }
    public void Rewind()
    {
        switch (state)
        {
            case State.Load:
                // nothing happens
                break;
            case State.Attack:
                state = State.Load;
                break;
            case State.Used:
                state = State.Load;
                break;
        }
        ActByState();
    }
    protected abstract void ActByState();
}

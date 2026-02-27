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
    public void FastForward()
    {
        Debug.Log("FastForwad");
        switch (state)
        {
            case State.Load:
                state = State.Attack;
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
    public void Rewind()
    {
        Debug.Log("Rewind");
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
    public void ActByState()
    {
        switch (state)
        {
            case State.Load:
                LoadState();
                break;
            case State.Attack:
                AttackState();
                break;
            case State.Used:
                UsedState();
                break;
        }
    }
    protected abstract void LoadState();
    protected abstract void AttackState();
    protected abstract void UsedState();
}

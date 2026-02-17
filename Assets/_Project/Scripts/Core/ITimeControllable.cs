using UnityEngine;

public interface ITimeControllable
{
    public void FastForward();
    public void CancelFastForward();
    public void Rewind();
    public void CancelRewind();
}

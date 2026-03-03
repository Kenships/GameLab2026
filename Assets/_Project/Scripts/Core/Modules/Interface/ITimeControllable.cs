namespace _Project.Scripts.Core.Modules.Interface
{
    public interface ITimeControllable
    {
        void Rewind();
        void CancelRewind();
        void FastForward();
        void CancelFastForward();
    }
}

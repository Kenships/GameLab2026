namespace _Project.Scripts.GridObjects.Interface
{
    public interface ITimeControllable
    {
        public bool IsWinding { get; set; }
        public void FastForward();
        public void CancelFastForward();
        public void Rewind();
        public void CancelRewind();
    }
}

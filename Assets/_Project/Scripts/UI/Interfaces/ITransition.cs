namespace _Project.Scripts.UI.Interfaces
{
    public interface ITransition
    {
        float TransitionDuration { get; }
        void Show();
        void Hide();
    }
}

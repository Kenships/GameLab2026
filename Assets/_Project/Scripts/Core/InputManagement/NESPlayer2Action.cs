namespace _Project.Scripts.Core.InputManagement
{
    public class NESPlayer2Action : NESActionBase
    {
        public NESPlayer2Action(NESActions action) : base(action)
        {
            action.KeyboardPlayer2.Move.performed += OnMovePerformed;
            action.KeyboardPlayer2.Move.canceled += OnMoveCancelled;
            action.KeyboardPlayer2.Interact.performed += OnInteractPerformed;
            action.KeyboardPlayer2.Interact.canceled += OnInteractCancelled;
            action.KeyboardPlayer2.AltInteract.performed += OnAltInteractPerformed;
            action.KeyboardPlayer2.AltInteract.canceled += OnAltInteractCancelled;
            action.Enable();
        }
    }
}

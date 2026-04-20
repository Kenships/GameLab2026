namespace _Project.Scripts.Core.InputManagement
{
    public class NESPlayer1Action : NESActionBase
    {
        public NESPlayer1Action(NESActions action) : base(action)
        {
            action.KeyboardPlayer1.Move.performed += OnMovePerformed;
            action.KeyboardPlayer1.Move.canceled += OnMoveCancelled;
            action.KeyboardPlayer1.Interact.performed += OnInteractPerformed;
            action.KeyboardPlayer1.Interact.canceled += OnInteractCancelled;
            action.KeyboardPlayer1.AltInteract.performed += OnAltInteractPerformed;
            action.KeyboardPlayer1.AltInteract.canceled += OnAltInteractCancelled;
            action.Enable();
        }
    }
}

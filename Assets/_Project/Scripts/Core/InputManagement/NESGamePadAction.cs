namespace _Project.Scripts.Core.InputManagement
{
    public class NESGamePadAction : NESActionBase
    {

        public NESGamePadAction(NESActions action) : base(action)
        {
            action.Player.Move.performed += OnMovePerformed;
            action.Player.Move.canceled += OnMoveCancelled;
            action.Player.Interact.performed += OnInteractPerformed;
            action.Player.Interact.canceled += OnInteractCancelled;
            action.Player.AltInteract.performed += OnAltInteractPerformed;
            action.Player.AltInteract.canceled += OnAltInteractCancelled;
            action.Enable();
        }
    }
}

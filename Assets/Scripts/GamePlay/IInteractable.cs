namespace AILand.GamePlay
{
    public interface IInteractable
    {
        public void OnFocus();
        public void OnLostFocus();
        public void Interact();
    }
}
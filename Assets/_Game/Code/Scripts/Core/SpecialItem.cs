namespace VinhLB
{
    public class SpecialItem : DraggableItem, IInteractable
    {
        public bool IsInteractable { get; set; } = false;
        
        public void Interact()
        {
            throw new System.NotImplementedException();
        }
    }
}
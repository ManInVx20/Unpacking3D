namespace VinhLB
{
    public interface IDraggable
    {
        bool IsDraggable { get; set; }
        bool IsDragging { get; set; }

        void Pick();
        void Drag();
        void Drop();
    }
}
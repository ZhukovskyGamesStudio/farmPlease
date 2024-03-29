namespace UI{
    public class DraggableSellTablet : DraggableUIElement{
        public void ChangeRects(bool isOpen){
            ChangeRectEventActive(0, !isOpen);
            ChangeRectEventActive(1, isOpen);
        }
    }
}
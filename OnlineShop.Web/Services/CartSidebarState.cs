namespace OnlineShop.Web.Services;

public class CartSidebarState
{
    public bool IsOpen { get; private set; }

    public event Action? OnChange;

    public void Open()
    {
        if (IsOpen) return;
        IsOpen = true;
        OnChange?.Invoke();
    }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        OnChange?.Invoke();
    }

    public void Set(bool isOpen)
    {
        if (IsOpen == isOpen) return;
        IsOpen = isOpen;
        OnChange?.Invoke();
    }
}

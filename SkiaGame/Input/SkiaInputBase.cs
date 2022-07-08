namespace SkiaGame.Input;

public abstract class SkiaInputBase
{
    private bool _isPressed;

    public bool IsPressed
    {
        get => _isPressed;
        set
        {
            switch (_isPressed)
            {
                //Released
                case true when !value:
                    Released.Invoke();
                    break;
                //Pressed
                case false when value:
                    Pressed.Invoke();
                    break;
                //No changes
                default:
                    return;
            }

            LastState = _isPressed;
            _isPressed = value;
        }
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool LastState { get; set; }
    public event Action Released = () => { };
    public event Action Pressed = () => { };
}
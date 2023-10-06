public class SquareWithLabel : SquareOrthogonal
{
    private Label _label;
    public string Text
    { get => _label.Text; set { _label.Text = value; } }

    protected override void Awake()
    {
        base.Awake();
        _label = GetComponentInChildren<Label>();
    }

    public void SetText(string text)
    {
        _label.SetText(text);
    }
}
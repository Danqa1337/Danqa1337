using TMPro;

public class SquareWithInputField : SquareOrthogonal
{
    private TMP_InputField _inputField;

    protected override void Awake()
    {
        base.Awake();
        _inputField = GetComponentInChildren<TMP_InputField>();
    }

    public string Text
    { get => _inputField.text; set { _inputField.text = value; } }
}
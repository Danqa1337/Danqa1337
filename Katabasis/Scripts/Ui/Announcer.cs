using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Announcer : Singleton<Announcer>
{
    public Canvas canvas;
    public Label messageLabel;
    private Vector2 initialSize;
    private RectTransform rectTransform;
    private const int frames = 16;
    private void Start()
    {
        canvas.enabled = false;
        rectTransform = messageLabel.GetComponent<RectTransform>();
        initialSize = rectTransform.sizeDelta;
    }
    public static async void Announce(string message)
    {
        i._Announce(message);
        
    }
    private async void _Announce(string message)
    {
        canvas.enabled = true;
       
        rectTransform.sizeDelta = new Vector2(0, initialSize.y);

        for (int i = 0; i < frames; i++)
        {
            rectTransform.sizeDelta = new Vector2(initialSize.x  /frames * i, initialSize.y);
            await Task.Delay(10);
        }
        messageLabel.SetText(message);

        await Task.Delay(4000);

        messageLabel.SetText(" ");
        for (int i = 0; i < frames; i++)
        {
            rectTransform.sizeDelta = new Vector2(initialSize.x - initialSize.x / frames * i, initialSize.y);
            await Task.Delay(10);
        }



        canvas.enabled = false;

    }
}

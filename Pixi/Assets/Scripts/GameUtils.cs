using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public Color GetColorFromEnum(ManagerView.BubbleColors bubbleColors)
    {
        switch(bubbleColors)
        {
            case ManagerView.BubbleColors.Blue:
                return new Color(0, 127f/255f, 1f);
            case ManagerView.BubbleColors.Green:
                return new Color(0, 1, 27f/255f);
            case ManagerView.BubbleColors.Pink:
                return new Color(1, 0, 227f/255f);
            case ManagerView.BubbleColors.Purple:
                return new Color(112f/255f, 0, 1);
            case ManagerView.BubbleColors.Red:
                return new Color(1, 0, 13f/255f);
            case ManagerView.BubbleColors.Turquize:
                return new Color(0, 1, 166f/255f);
            case ManagerView.BubbleColors.Yellow:
                return new Color(242f/255f, 1, 0);
            case ManagerView.BubbleColors.Black:
                return new Color(0, 0, 0);
            default:
                return new Color(1, 1, 1);
        }
    }


}

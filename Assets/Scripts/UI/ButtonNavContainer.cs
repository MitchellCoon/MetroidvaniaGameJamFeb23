using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Add this class to the parent container of a group
/// of Button components to dynamically assign
/// the navigation at runtime.
/// </summary>
public class ButtonNavContainer : MonoBehaviour
{
    enum Direction
    {
        Vertical,
        Horizontal,
    }

    [SerializeField] Direction navDirection;

    Button[] buttons;

    void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            if (navDirection == Direction.Vertical)
            {
                nav.selectOnDown = GetNext(i);
                nav.selectOnUp = GetPrev(i);
            }
            else
            {
                nav.selectOnRight = GetNext(i);
                nav.selectOnLeft = GetPrev(i);
            }
            buttons[i].navigation = nav;
        }
    }

    Button GetNext(int index)
    {
        if (buttons == null || buttons.Length == 0) return null;
        return buttons[(buttons.Length + index + 1) % buttons.Length];
    }
    Button GetPrev(int index)
    {
        if (buttons == null || buttons.Length == 0) return null;
        return buttons[(buttons.Length + index - 1) % buttons.Length];
    }
}

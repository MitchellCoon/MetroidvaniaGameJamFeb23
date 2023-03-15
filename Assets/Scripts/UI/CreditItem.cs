using UnityEngine;
using TMPro;

public class CreditItem : MonoBehaviour
{
    [SerializeField][TextArea] string role;
    [SerializeField][TextArea] string contributor;
    [Space]
    [Space]
    [SerializeField] TextMeshProUGUI textNodeRole;
    [SerializeField] TextMeshProUGUI textNodeContributor;

    void Awake()
    {
        if (!string.IsNullOrEmpty(role)) textNodeRole.text = role;
        if (!string.IsNullOrEmpty(contributor)) textNodeContributor.text = contributor;
    }
}

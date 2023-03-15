using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextSequence : MonoBehaviour
{
    [SerializeField] List<Message> messages = new List<Message>();
    [SerializeField] List<float> glitchDurations = new List<float>();
    [SerializeField] float waitTimeBetweenMessages = 3f;

    [System.Serializable]
    struct Message
    {
        [SerializeField][TextArea(minLines: 3, maxLines: 5)] public string text;
    }

    TextMeshProUGUI textNode;

    void Awake()
    {
        textNode = GetComponent<TextMeshProUGUI>();
    }

    IEnumerator Start()
    {
        while (true)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                if (i == 0) yield return CGlitch(messages[i].text);
                textNode.text = messages[i].text;
                yield return new WaitForSeconds(waitTimeBetweenMessages);
                yield return CGlitch(messages[i].text);
            }
            yield return null;
        }
    }

    IEnumerator CGlitch(string message)
    {
        for (int i = 0; i < glitchDurations.Count; i++)
        {
            if (i % 2 == 0)
            {
                textNode.text = "";
            }
            else
            {
                textNode.text = message;
            }
            yield return new WaitForSeconds(glitchDurations[i]);
        }
    }
}

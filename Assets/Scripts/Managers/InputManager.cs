using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public enum Input {Direction1, Direction2, Direction3, Direction4, Direction5, Direction6, Direction7, Direction8, Direction9, Attack1, Attack2, Jump, CoyoteJump, Number1, Number2, Number3, Number4, Special};
    public enum Action {Jump, Grounded, Attack}
    public List<Input> inputQueue;
    public List<float> inputTimes;
    public List<Input> previousPressedInputs;
    public List<float> previousPressedTimes;
    public List<Action> previousPerfomedActions;
    public List<float> previousPerformedTimes;
    private List<Input> inputBufferList = new List<Input>{Input.Jump, Input.CoyoteJump, Input.Attack1, Input.Attack2};
    [SerializeField] List<float> inputBufferTimes = new List<float>{0.15f, 0.15f, 0.15f, 0.15f};

    void Update()
    {
        foreach (Input input in inputBufferList)
        {
            int previousPressedIndex = previousPressedInputs.IndexOf(input);
            if (previousPressedIndex != -1 && inputQueue.Contains(input) && (Time.time - previousPressedTimes[previousPressedIndex] >= GetInputBufferTime(input)))
            {
                RemoveInputRequestFromQueue(input);
            }
        }
    }

    public bool GetInputRequested(Input input)
    {
        return inputQueue.Contains(input);
    }

    public float? GetPreviousActionTime(Action action)
    {
        int index = previousPerfomedActions.IndexOf(action);
        return index == -1 ? -1 : previousPerformedTimes[index];
    }

    public void AddInputRequestToQueue(Input input, float time)
    {
        int index = inputQueue.IndexOf(input);
        if (index == -1)
        {
            inputQueue.Add(input);
            inputTimes.Add(time);
        }
    }

    public void RemoveInputRequestFromQueue(Input input)
    {
        int index = inputQueue.IndexOf(input);
        if (index != -1)
        {
            inputQueue.RemoveAt(index);
            inputTimes.RemoveAt(index);
        }

    }

    public void SetPreviousActionTime(Action action, float time)
    {
        int index = previousPerfomedActions.IndexOf(action);
        if (index == -1)
        {
            previousPerfomedActions.Add(action);
            previousPerformedTimes.Add(time);
        }
        else
        {
            previousPerformedTimes[index] = time;
        }
    }

    public void SetPreviousPressedTime(Input input, float time)
    {
        int index = previousPressedInputs.IndexOf(input);
        if (index == -1)
        {
            previousPressedInputs.Add(input);
            previousPressedTimes.Add(time);
        }
        else
        {
            previousPressedTimes[index] = time;
        }
    }

    public float GetInputBufferTime(Input input)
    {
        int inputBufferIndex = inputBufferList.IndexOf(input);
        return inputBufferTimes[inputBufferIndex];
    }
}

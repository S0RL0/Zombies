using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ExpressionSwitcher : MonoBehaviour
{
    private Animator _animator;

    // Define a list of expression names (will be dynamically populated)
    private List<string> _expressionNames = new List<string>();

    private int _currentIndex = 0;

    // Reference to the UI text to display the current emotion
    [SerializeField] private Text _emotionText;

    // Reference to the UI slider to adjust transition time
    [SerializeField] private Slider _transitionTimeSlider;

    private float _transitionTime = 0.2f; // Default transition time

    void Start()
    {
        _animator = GetComponent<Animator>();

        // Ensure the Animator is assigned
        if (_animator == null)
        {
            Debug.LogWarning("No Animator component found on the GameObject.");
            return;
        }

        // Populate the expression names from the Animator states in the specified layer
        PopulateExpressionNames();

        // Ensure the slider is assigned
        if (_transitionTimeSlider != null)
        {
            _transitionTimeSlider.minValue = 0.0f;
            _transitionTimeSlider.maxValue = 1.0f;
            _transitionTimeSlider.value = _transitionTime;

            // Read the initial value from the slider to set the transition time
            UpdateTransitionTime(_transitionTimeSlider.value);

            // Add a listener to handle value changes
            _transitionTimeSlider.onValueChanged.AddListener(UpdateTransitionTime);
        }
    }
    
    private void PopulateExpressionNames()
    {
        RuntimeAnimatorController ac = _animator.runtimeAnimatorController;

        if (ac != null)
        {
            
            foreach (AnimationClip clip in ac.animationClips)
            {
                if (clip.name.Contains("A_FacePose") && !_expressionNames.Contains(clip.name) && !clip.name.Contains("Neutral"))
                {
                    _expressionNames.Add(clip.name);
                }
            }
        }
        else
        {
            Debug.LogError("AnimatorController is not found.");
        }
    }

    public void CycleExpressions()
    {
        if (_animator != null && _expressionNames.Count > 0)
        {
            string layerName = "Emotion_Additive";
            int layerIndex = _animator.GetLayerIndex(layerName);

            if (layerIndex != -1)
            {
                string expressionName = _expressionNames[_currentIndex];

                // Attempt to play the current expression name on the specified layer
                if (_animator.HasState(layerIndex, Animator.StringToHash(expressionName)))
                {
                    _animator.CrossFadeInFixedTime(expressionName, _transitionTime, layerIndex);
                }
                else
                {
                    // If the state is not found, play the 'Neutral' state on the same layer
                    _animator.CrossFadeInFixedTime("Neutral", _transitionTime, layerIndex);
                }

                // Update the emotion text after playing the animation
                UpdateEmotionText();

                // Move to the next expression in the list
                _currentIndex = (_currentIndex + 1) % _expressionNames.Count;
            }
        }
    }

    private void UpdateEmotionText()
    {
        if (_emotionText != null && _expressionNames.Count > 0)
        {
            string expressionName = _expressionNames[_currentIndex];
            int underscoreIndex = expressionName.LastIndexOf('_');
            if (underscoreIndex != -1 && underscoreIndex < expressionName.Length - 1)
            {
                _emotionText.text = expressionName.Substring(underscoreIndex + 1);
            }
            else
            {
                _emotionText.text = expressionName; // Fallback if no underscore found
            }
        }
    }

    private void UpdateTransitionTime(float value)
    {
        _transitionTime = value;
    }
}

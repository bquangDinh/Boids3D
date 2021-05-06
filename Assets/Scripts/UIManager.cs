using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider cohensionSlider;
    public Slider seperationSlider;
    public Slider alignmentSlider;

    public Toggle enableDrawingVelocityComponentsToggle;
    public Toggle enableDrawingVelocitySumToggle;
    public Toggle enableDrawingOnSingleBoidOnlyToggle;

    // Start is called before the first frame update
    void Start()
    {
        cohensionSlider.value = Constants.cohensionFactor;
        seperationSlider.value = Constants.seperationFactor;
        alignmentSlider.value = Constants.alignmentFactor;

        enableDrawingVelocityComponentsToggle.isOn = Constants.drawingVelocityComponents;
        enableDrawingVelocitySumToggle.isOn = Constants.drawingVelocitySum;
        enableDrawingOnSingleBoidOnlyToggle.isOn = Constants.drawingOnSingleBoidOnly;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CohensionFactorCallback(float value)
    {
        Constants.cohensionFactor = value;
    }

    public void SeperationFactorCallback(float value)
    {
        Constants.seperationFactor = value;
    }

    public void AlignmentFactorCallback(float value)
    {
        Constants.alignmentFactor = value;
    }

    public void EnableDrawingVelocityComponents(bool value)
    {
        Constants.drawingVelocityComponents = value;
    }

    public void EnableDrawingVelocitySum(bool value)
    {
        Constants.drawingVelocitySum = value;
    }

    public void EnableDrawingOnSingleBoidOnly(bool value)
    {
        Constants.drawingOnSingleBoidOnly = value;
        if(value == true)
        {
            Constants.singleBoidID = Random.Range(0, Constants.numberOfBoids - 1);
        }
    }
}

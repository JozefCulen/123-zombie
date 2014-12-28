using UnityEngine;
using System.Collections;
using GlobalVariables;

public class WheelAnchor : MonoBehaviour {

	public constants.WheelTypeEnum wheelType;
	public bool powered;
	public float suspensionDampingRatio = 0.2f;
	public float suspensionFrequency = 4f;
}

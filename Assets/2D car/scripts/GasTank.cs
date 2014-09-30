using UnityEngine;
using System.Collections;

public class GasTank : MonoBehaviour {
	float maxFill;
	float currentFill;

	public void setMaxFill(float max)	{
		this.maxFill = max;
		this.fillUp ();
	}

	public bool use(float x)	{
		this.currentFill -= x;
		if (this.currentFill < 0) {
			this.currentFill = 0;
			return false;
		}
		return true;
	}

	public void fillUp()	{
		this.currentFill = this.maxFill;
	}

	public float getMaxFill()	{
		return this.maxFill;
	}

	public float getCurrentFill()	{
		return this.currentFill;
	}

}

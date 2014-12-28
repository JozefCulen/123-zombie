using UnityEngine;
using System.Collections;

public class GasTank {
	private float maxFill;
	private float currentFill;

	public GasTank( float input_maxTankCapacity, float input_defaultTankValue ) {
		this.maxFill = input_maxTankCapacity;
		this.currentFill = input_defaultTankValue;
		gui.gasRemainingValue = 1.0f;
	}

	public void Use(float value)	{
		this.currentFill -= value;

		// hodnota nadrze nemoze byt zaporna
		if (this.currentFill < 0) {
			this.currentFill = 0;
		}

		gui.gasRemainingValue = this.currentFill / this.maxFill;
	}

	public void Fill(float value)	{
		this.currentFill += value;
		
		// hodnota nadrze nemoze byt vacsia ako maximalny objem
		if (this.currentFill > this.maxFill) {
			this.currentFill = this.maxFill;
		}

		gui.gasRemainingValue = this.currentFill / this.maxFill;
	}

	public void FillUp()	{
		this.currentFill = this.maxFill;
	}

	public bool IsEmpty() {
		return this.currentFill == 0 ? true : false;
	}

	public float getMaxFill()	{
		return this.maxFill;
	}

	public float getCurrentFill()	{
		return this.currentFill;
	}

}

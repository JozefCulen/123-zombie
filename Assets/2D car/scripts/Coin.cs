using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
	public int value = 0;
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "car" ||
		    ( other.gameObject.transform.parent != null && other.gameObject.transform.parent.gameObject.tag == "car") ) {
			GameObject.FindGameObjectWithTag("car").GetComponent<car>().addCoins(value);
			Destroy(this.gameObject);
		}
	}
}

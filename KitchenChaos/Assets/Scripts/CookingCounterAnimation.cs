using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingCounterAnimation : MonoBehaviour {
    [SerializeField] GameObject particles;
    [SerializeField] GameObject stoveOn;
    [SerializeField] CookingCounter cookingCounter;


    private void Start() {
        cookingCounter.OnStateChange += CookingCounter_OnStateChange;
    }
    private void CookingCounter_OnStateChange(object sender, CookingCounter.OnStateChangeEventArgs e) {
        bool showVisuals = e.state == CookingCounter.State.Frying || e.state == CookingCounter.State.Fried;

        particles.SetActive(showVisuals);
        stoveOn.SetActive(showVisuals);
    }

}

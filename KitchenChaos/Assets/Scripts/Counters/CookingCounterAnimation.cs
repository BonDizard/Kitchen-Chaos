/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: effects while Cooking 
 */
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

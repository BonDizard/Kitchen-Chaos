/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * removing all static objects since it wont get destroyed
 */

using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour {
    private void Awake() {
        CuttingCounter.ResetStaticData();
        BaseCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
    }
}


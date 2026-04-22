using UnityEngine;

public class CameraUpdate : MonoBehaviour {

    public WorldManager WorldManager = null;

    private void Update() {
        if (WorldManager != null) {
            WorldManager.UpdatePlayerPosition(transform.position);
        }
    }

}
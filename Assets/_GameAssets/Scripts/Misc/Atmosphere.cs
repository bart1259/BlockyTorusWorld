using UnityEngine;
using UnityEngine.InputSystem;

public class Atmosphere : MonoBehaviour
{
    public GameObject atmospherePrefab;
    public int atmosphereLayers;

    public float startDisplacement;
    public float endDisplacement;

    public float startFrenelStrength;
    public float endFrenelStrength;

    public float fadeStartDistance;
    public float fadeEndDistance;
    public float fresnelStrength;
    public float fresnelPower;

    void Start() {
        Setup();
    }

    void Setup() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        int layerDivisor = (atmosphereLayers > 1) ? (atmosphereLayers - 1) : 1;
        for (int i = 0; i < atmosphereLayers; i++) {
            GameObject atmosphereLayer = Instantiate(atmospherePrefab, transform);
            atmosphereLayer.transform.localPosition = Vector3.zero;

            float t = (float)i / layerDivisor;

            float displacement = Mathf.Lerp(startDisplacement, endDisplacement, t);
            atmosphereLayer.GetComponent<MeshRenderer>().material.SetFloat("_DisplacementAmount", displacement);

            float fresnelStrength = Mathf.Lerp(startFrenelStrength, endFrenelStrength, t);
            atmosphereLayer.GetComponent<MeshRenderer>().material.SetFloat("_FresnelStrength", fresnelStrength);

            atmosphereLayer.GetComponent<MeshRenderer>().material.SetFloat("_FadeStartDistance", fadeStartDistance);
            atmosphereLayer.GetComponent<MeshRenderer>().material.SetFloat("_FadeEndDistance", fadeEndDistance);
            atmosphereLayer.GetComponent<MeshRenderer>().material.SetFloat("_FresnelStrength", fresnelStrength);
            atmosphereLayer.GetComponent<MeshRenderer>().material.SetFloat("_FresnelPower", fresnelPower);
        }
    }

    void Update() {
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame) {
            Setup();
        }
    }
}

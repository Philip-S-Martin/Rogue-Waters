using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkyboxAndLighting : MonoBehaviour
{
    Light sunLight, moonLight, ambientLightA, ambientLightB;
    public Color dayTime, nightTime, dawnDusk;
    public GameObject moonLightObject = null;
    public GameObject ambientA = null;
    public GameObject ambientB = null;
    // Start is called before the first frame update
    void Start()
    {
        sunLight = gameObject.GetComponent<Light>();
        moonLight = moonLightObject.GetComponent<Light>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        sunLight.intensity = Mathf.Lerp(0f,1.25f,Mathf.Clamp(gameObject.transform.forward.y * -4f + 0.2f,0,1));
        moonLight.intensity = Mathf.Lerp(0.5f, 0f, Mathf.Clamp(gameObject.transform.forward.y * -4f, 0, 1));

        if (gameObject.transform.forward.y < 0)
            RenderSettings.ambientLight = Color.Lerp(dawnDusk, dayTime, Mathf.Clamp(gameObject.transform.forward.y * -5f, 0, 1));
        else
            RenderSettings.ambientLight = Color.Lerp(dawnDusk, nightTime, Mathf.Clamp(gameObject.transform.forward.y * -5f, -1, 0) * -1);
    }
}

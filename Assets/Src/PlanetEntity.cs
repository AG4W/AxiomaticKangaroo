using UnityEngine;

public class PlanetEntity : MonoBehaviour
{
    public void Initialize(Palette palette, float waterLevel, bool isTerrestrial)
    {
        Transform planet = this.transform.Find("planet");
        planet.GetComponent<Renderer>().material.SetTexture("_palette", palette.land);

        if (isTerrestrial)
        {
            planet.GetComponent<Renderer>().material.SetTexture("_water", palette.water);
            planet.GetComponent<Renderer>().material.SetFloat("_waterLevel", waterLevel);
        }
    }
}

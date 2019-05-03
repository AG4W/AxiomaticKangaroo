using UnityEngine;
using UnityEngine.SceneManagement;

using Random = System.Random;
using System.Collections;

public class PointOfInterest
{
    public string name { get; protected set; }

    public Vector3 position { get; private set; }

    public GameObject model { get; protected set; }
    public GameObject prefab { get; protected set; }
    public PointOfInterestType type { get; protected set; }

    protected Random random { get; private set; }

    public bool isBusy { get; private set; } = false;

    public PointOfInterest(string name, Vector3 position, Random random)
    {
        this.name = name;
        this.position = position;

        this.random = random;

        this.type = PointOfInterestType.Default;
    }

    public virtual GameObject Instantiate()
    {
        if (model != null)
            prefab = Object.Instantiate(model, position, Quaternion.identity, null);

        return prefab;
    }
    public virtual void Deinstantiate()
    {
        if (prefab != null)
            Object.Destroy(prefab);
    }

    public virtual void Move(Vector3 position, float speed = 1f)
    {
        CoroutineSurrogate.getInstance.StartCoroutine(MovePrefabAsync(position, speed));
    }

    public virtual void OnMouseEnter()
    {
        TooltipManager.getInstance.OpenTooltip(GetTooltip(), Input.mousePosition);
        OverworldInputManager.getInstance.HandlePoIMouseCallback(CallbackType.MouseEnter, this);
    }
    public virtual void OnLeftClick()
    {
        OverworldInputManager.getInstance.HandlePoIMouseCallback(CallbackType.LeftDown, this);
    }
    public virtual void OnScrollClick()
    {
        OverworldInputManager.getInstance.HandlePoIMouseCallback(CallbackType.ScrollDown, this);
    }
    public virtual void OnRightClick()
    {
        OverworldInputManager.getInstance.HandlePoIMouseCallback(CallbackType.RightDown, this);
    }
    public virtual void OnMouseExit()
    {
        TooltipManager.getInstance.CloseTooltip();
        OverworldInputManager.getInstance.HandlePoIMouseCallback(CallbackType.MouseExit, this);
    }

    public virtual void Interact()
    {
        LocalMapData lmd = new LocalMapData(this.name, new float[] { 0f, 0f, 0f }, this.position);

        lmd.SetFleets(PlayerData.fleet);

        RuntimeData.SetLocalMapData(lmd);
        SceneManager.LoadScene("LocalMap");
    }

    public virtual string GetTooltip()
    {
        return "";
    }

    IEnumerator MovePrefabAsync(Vector3 e, float speed)
    {
        Vector3 o = position;

        float s = (speed / (o - e).magnitude) * Time.fixedDeltaTime;
        float t = 0;

        isBusy = true;

        while (t <= 1f)
        {
            t += s;
            position = Vector3.Lerp(o, e, t);
            prefab.transform.position = position;

            yield return new WaitForFixedUpdate();
        }

        isBusy = false;
    }
}
public enum PointOfInterestType
{
    Fleet,
    Planet,
    Star,
    Nebula,
    Wormhole,
    Structure,
    Default
}

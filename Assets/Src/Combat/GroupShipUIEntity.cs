using UnityEngine;
using UnityEngine.UI;

public class GroupShipUIEntity : MonoBehaviour
{
    ShipEntity _ship;

    Image _highlight;
    Image _shield;
    Image _hull;

    public void Initialize(Image highlight, Image shield, Image hull, ShipEntity ship)
    {
        _ship = ship;

        _highlight = highlight;
        _shield = shield;
        _hull = hull;

        ship.OnSelected += OnSelected;
        ship.OnDeselected += OnDeselected;
        ship.OnVitalChanged += OnVitalChanged;
        ship.OnDestroyed += OnDestroyed;
    }
    
    void OnSelected()
    {
        _highlight.enabled = true;
    }
    void OnDeselected()
    {
        _highlight.enabled = false;
    }

    void OnVitalChanged(Vital v)
    {
        ShipVital sv = v as ShipVital;

        if (sv.type == VitalType.ShieldPoints)
            _shield.fillAmount = sv.inPercent;
        else if (sv.type == VitalType.HullPoints)
            _hull.fillAmount = sv.inPercent;
    }
    void OnDestroyed(ShipEntity s)
    {
        _ship.OnSelected -= OnSelected;
        _ship.OnDeselected -= OnDeselected;
        _ship.OnVitalChanged -= OnVitalChanged;
        _ship.OnDestroyed -= OnDestroyed;
    }

    public void OnRemoved()
    {
        _ship.OnSelected -= OnSelected;
        _ship.OnDeselected -= OnDeselected;
        _ship.OnVitalChanged -= OnVitalChanged;
        _ship.OnDestroyed -= OnDestroyed;
    }
}

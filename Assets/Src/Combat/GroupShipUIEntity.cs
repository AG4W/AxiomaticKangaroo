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

    void OnVitalChanged(VitalType v, float current)
    {
        if (v == VitalType.ShieldPoints)
            _shield.fillAmount = _ship.GetVital(v).inPercent;
        else if (v == VitalType.HullPoints)
            _hull.fillAmount = _ship.GetVital(v).inPercent;
    }
    void OnDestroyed(ShipEntity s)
    {
        _ship.OnVitalChanged -= OnVitalChanged;
        _ship.OnDestroyed -= OnDestroyed;
    }

    public void OnRemoved()
    {
        _ship.OnVitalChanged -= OnVitalChanged;
        _ship.OnDestroyed -= OnDestroyed;
    }
}

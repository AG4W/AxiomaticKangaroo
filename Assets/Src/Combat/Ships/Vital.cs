public class Vital
{
    float _maxValue;
    float _currentValue;

    public float max { get { return _maxValue; } }
    public float current { get { return _currentValue; } }
    public float inPercent { get { return _currentValue / _maxValue; } }

    public Vital(float maxValue)
    {
        _maxValue = maxValue;
        _currentValue = maxValue;
    }

    public void Update(float amount)
    {
        _currentValue += amount;

        OnCurrentChanged();
    }

    public void Set(float value)
    {
        _currentValue = value;

        OnCurrentChanged();
    }
    public void SetMax(float max, bool updateCurrent)
    {
        _maxValue = max;

        if (updateCurrent)
            _currentValue = _maxValue;

        if (_currentValue >= _maxValue)
            _currentValue = _maxValue;
    }

    void OnCurrentChanged()
    {
        if (_currentValue < 0)
            _currentValue = 0;
        else if (_currentValue > _maxValue)
            _currentValue = _maxValue;

        OnVitalChanged?.Invoke(this);
    }

    public override string ToString()
    {
        return _currentValue.ToString("0.##;-0.##") + " / " + _maxValue.ToString("0.##");
    }

    public delegate void VitalEvent(Vital v);
    public event VitalEvent OnVitalChanged;
}


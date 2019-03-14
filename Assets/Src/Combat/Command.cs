using UnityEngine;

public abstract class Command
{
    ShipEntity _entity;

    protected ShipEntity entity { get { return _entity; } }

    public Command(ShipEntity entity)
    {
        _entity = entity;
    }

    public virtual void Execute()
    {
        _entity = entity;
    }
    public abstract bool Status();
}
public class Move : Command
{
    Vector3? _position;

    public Vector3 position { get { return (Vector3)_position; } }

    public Move(Vector3? position, ShipEntity entity) : base(entity)
    {
        _position = position;
    }

    public override void Execute()
    {
        base.entity.UpdateMovePosition(_position);
    }
    public override bool Status()
    {
        if (_position == null)
            return true;

        return Vector3.Distance((Vector3)_position, base.entity.transform.position) < 20f;
    }
}
public class Target : Command
{
    ShipEntity _target;

    public Target(ShipEntity target, ShipEntity entity) : base(entity)
    {
        _target = target;
    }

    public override void Execute()
    {
        base.entity.UpdateTarget(_target);
    }
    public override bool Status()
    {
        return true;
    }
}

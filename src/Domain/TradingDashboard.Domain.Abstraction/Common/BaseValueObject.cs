namespace TradingDashboard.Domain.Common.Common;

public abstract class BaseValueObject
{
    protected abstract IEnumerable<object?> GetAtomicValues();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (BaseValueObject)obj;
        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }

    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Aggregate(0, (current, value) => HashCode.Combine(current, value));
    }

    public static bool operator ==(BaseValueObject? left, BaseValueObject? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BaseValueObject? left, BaseValueObject? right)
    {
        return !Equals(left, right);
    }
}


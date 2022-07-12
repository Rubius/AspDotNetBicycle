﻿namespace Domain.Abstractions;

public abstract class ValueObject
{
    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }
        return left is null || left.Equals(right!);
    }

    protected static bool NotEqualOperator(ValueObject? left, ValueObject right)
    {
        return !(EqualOperator(left, right));
    }

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? one, ValueObject? two)
    {
        return one?.Equals(two) ?? false;
    }

    public static bool operator !=(ValueObject? one, ValueObject? two)
    {
        return !(one?.Equals(two) ?? false);
    }
}
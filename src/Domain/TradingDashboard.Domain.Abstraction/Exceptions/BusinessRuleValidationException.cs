namespace TradingDashboard.Domain.Common.Exceptions;

public sealed class BusinessRuleValidationException : Exception
{
    public BusinessRuleValidationException(string message) : base(message)
    {
    }
}


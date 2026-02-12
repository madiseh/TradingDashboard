namespace TradingDashboard.Domain.Abstraction.Exceptions;

public sealed class BusinessRuleValidationException : Exception
{
    public BusinessRuleValidationException(string message) : base(message)
    {
    }
}

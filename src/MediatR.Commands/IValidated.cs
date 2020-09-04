namespace MediatR.Commands
{
    using FluentValidation.Results;

    public interface IValidated
    {
        ValidationResult Validate();
    }
}
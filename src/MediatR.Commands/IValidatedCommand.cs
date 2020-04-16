namespace MediatR.Commands
{
    using FluentValidation.Results;

    public interface IValidatedCommand
    {
        ValidationResult Validate();
    }
}
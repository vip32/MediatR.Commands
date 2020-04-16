namespace MediatR.Commands
{
    using FluentValidation.Results;

    public interface IValidateCommand
    {
        ValidationResult Validate();
    }
}
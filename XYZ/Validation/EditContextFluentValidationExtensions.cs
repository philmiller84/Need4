using FluentValidation;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq;
using System.Reflection;

namespace XYZ.Validation
{
    public static class EditContextFluentValidationExtensions
    {
        public static EditContext AddFluentValidation(this EditContext editContext)
        {
            if (editContext == null)
            {
                throw new ArgumentNullException(nameof(editContext));
            }

            ValidationMessageStore messages = new ValidationMessageStore(editContext);

            editContext.OnValidationRequested +=
                (sender, eventArgs) => ValidateModel((EditContext)sender, messages);

            editContext.OnFieldChanged +=
                (sender, eventArgs) => ValidateField(editContext, messages, eventArgs.FieldIdentifier);

            return editContext;
        }

        private static void ValidateModel(EditContext editContext, ValidationMessageStore messages)
        {
            IValidator validator = GetValidatorForModel(editContext.Model);
            FluentValidation.Results.ValidationResult validationResults = validator.Validate(editContext.Model);

            messages.Clear();
            foreach (FluentValidation.Results.ValidationFailure validationResult in validationResults.Errors)
            {
                messages.Add(editContext.Field(validationResult.PropertyName), validationResult.ErrorMessage);
            }

            editContext.NotifyValidationStateChanged();
        }

        private static void ValidateField(EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier)
        {
            string[] properties = new[] { fieldIdentifier.FieldName };
            ValidationContext context = new ValidationContext(fieldIdentifier.Model, new PropertyChain(), new MemberNameValidatorSelector(properties));

            IValidator validator = GetValidatorForModel(fieldIdentifier.Model);
            FluentValidation.Results.ValidationResult validationResults = validator.Validate(context);

            messages.Clear(fieldIdentifier);

            foreach (FluentValidation.Results.ValidationFailure validationResult in validationResults.Errors)
            {
                messages.Add(editContext.Field(validationResult.PropertyName), validationResult.ErrorMessage);
            }

            editContext.NotifyValidationStateChanged();
        }

        private static IValidator GetValidatorForModel(object model)
        {
            Type modelType = model.GetType();
            Type abstractConstructor = typeof(AbstractValidator<>);
            Type abstractValidatorType = abstractConstructor.MakeGenericType(modelType);
            //var executingAssembly = Assembly.GetExecutingAssembly();
            Assembly validatorAssembly = Assembly.Load("API");
            Type[] assemblyTypes = validatorAssembly.GetTypes();
            Type modelValidatorType = assemblyTypes.FirstOrDefault(t => t.IsSubclassOf(abstractValidatorType));
            IValidator modelValidatorInstance = (IValidator)Activator.CreateInstance(modelValidatorType);

            return modelValidatorInstance;
        }
    }
}

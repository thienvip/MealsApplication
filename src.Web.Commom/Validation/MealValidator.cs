using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using src.Core.Domains;
using src.Localization.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Web.Common.Validation
{
    public class MealValidator : AbstractValidator<Meal>
    {
       
        public MealValidator() 
        {
          
            RuleFor(x => x.StudentCode)
            .NotEmpty()
            .WithMessage(SharedResource.EnterStudentCode)
            .MaximumLength(100)
            .WithMessage(SharedResource.Exceed100Characters);

            RuleFor(x => x.StudentName)
                .NotEmpty()
                .WithMessage(SharedResource.EnterStudentName)
                .MaximumLength(100)
                .WithMessage(SharedResource.Exceed100Characters);


            RuleFor(x => x.FromDate)
                .NotEmpty()
                .WithMessage(SharedResource.EnterFromDate)
                .Custom((fromDate, context) =>
                {
                    var selectedDate = fromDate;
                    var currentDate = DateTime.Now;

                    if (selectedDate < currentDate)
                    {
                        context.AddFailure(SharedResource.DateEarlierCurrentDate);
                    }
                }); 

            RuleFor(x => x.ToDate)
                .NotEmpty()
                .WithMessage(SharedResource.EnterToDate)
                .Custom((toDate, context) =>
                {
                    var selectedDate = toDate;
                    var currentDate = DateTime.Now;

                    if (selectedDate < currentDate)
                    {
                        context.AddFailure(SharedResource.DateEarlierCurrentDate);
                    }
                });

            RuleFor(x => x.FromDate)
                .Must((model, fromDate) =>
                {
                    var selectedDate = fromDate;
                    var toDate = model.ToDate;

                    return selectedDate <= toDate;
                })
                .WithMessage(SharedResource.DateEarlierEndDate);

            RuleFor(x => x.MealRegistration)
                .NotEmpty()
                .WithMessage("Meal Registration is required.");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage(SharedResource.EnterReason)
                .MaximumLength(100)
                .WithMessage(SharedResource.Exceed100Characters);

            RuleFor(x => x.TotalNumberofdays)
                .NotEmpty()
                .WithMessage(SharedResource.EnterTotalNumberOfDays);

        }
    }
}

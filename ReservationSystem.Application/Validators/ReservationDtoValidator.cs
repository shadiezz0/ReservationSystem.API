using FluentValidation;

namespace ReservationSystem.Application.Validators
{
      public class ReservationDtoValidator : AbstractValidator<ReservationDto>
      {
            public ReservationDtoValidator()
            {
                  RuleFor(x => x.ReservationDate).NotEmpty();
                  RuleFor(x => x.StartTime).LessThan(x => x.EndTime);
            }
      }
}

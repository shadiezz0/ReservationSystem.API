using FluentValidation;

namespace ReservationSystem.Application.Validators
{
      public class ItemDtoValidator : AbstractValidator<ItemDto>
      {
            public ItemDtoValidator()
            {
                  RuleFor(x => x.Name).NotEmpty();
                  RuleFor(x => x.PricePerHour).GreaterThan(0);
            }
      }
}

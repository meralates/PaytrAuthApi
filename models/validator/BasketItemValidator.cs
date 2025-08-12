using FluentValidation;

public class BasketItemValidator : AbstractValidator<BasketItem>
{
    public BasketItemValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Ürün adı boş olamaz.");
        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Fiyat boş olamaz.")
            .Matches(@"^\d+(\.\d{1,2})?$").WithMessage("Fiyat formatı geçersiz.");
        RuleFor(x => x.Quantity).InclusiveBetween(1, 10).WithMessage("Adet 1 ile 10 arasında olmalı.");
    }
}

public class PaymentRequestModelValidator : AbstractValidator<PaymentRequestModel>
{
    public PaymentRequestModelValidator()
    {
        RuleFor(x => x.UserIp)
            .NotEmpty().WithMessage("Kullanıcı IP adresi boş olamaz.")
            .Matches(@"^(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
                     + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
                     + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
                     + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$")
            .WithMessage("IP adresi geçerli bir IPv4 adresi olmalı.");

        RuleFor(x => x.Amount)
            .InclusiveBetween(1, 10000).WithMessage("Tutar 1 ile 10.000₺ arasında olmalı.");

        RuleFor(x => x.UserBasket)
            .NotEmpty().WithMessage("Sepet boş olamaz.")
            .Must(list => list.Count >= 1 && list.Count <= 10)
            .WithMessage("Sepette 1 ile 10 ürün olmalı.");

        RuleForEach(x => x.UserBasket).SetValidator(new BasketItemValidator());
    }
}
using FluentValidation;

namespace IAE.Microservice.Application.Common
{
    public abstract class PagedListQueryBaseValidator<T> : AbstractValidator<T> where T : PagedListQueryBase
    {
        public PagedListQueryBaseValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(default(int));
            RuleFor(x => x.PageSize).GreaterThan(default(int)).LessThanOrEqualTo(PagedListQueryBase.MaxPageSize);
        }
    }
    
    public abstract class PagedListQueryValidator<T> : AbstractValidator<T> where T : PagedListQuery
    {
        public PagedListQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(default(int));
            RuleFor(x => x.PageSize).GreaterThan(default(int)).LessThanOrEqualTo(PagedListQueryBase.MaxPageSize);
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}

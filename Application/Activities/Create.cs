using MediatR;
using Persistence;
using Domain;
using FluentValidation;

namespace Application.Activities
{
	public class Create
	{
		public class Command : IRequest
		{
			public Activity Activity { get; set; }
		}

		public class CommandValidator : AbstractValidator<Activity>
		{
			public CommandValidator() {
				RuleFor(x=> x.Title).NotEmpty();
			}
		}

		public class Handler : IRequestHandler<Command>
		{
			private readonly DataContext _context;
			public Handler(DataContext context)
			{
				_context = context;
			}

			public async Task Handle(Command request, CancellationToken cancellationToken)
			{
				_context.Activities.Add(request.Activity);
				await _context.SaveChangesAsync();

			}

		}
	}
}

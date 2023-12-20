﻿using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;


namespace Application.Comments
{
	public class Create
	{
		public class Command : IRequest<Result<CommentDto>> 
		{
            public string Body { get; set; }
			public Guid ActivityId { get; set; }
        }

		public class CommandValidator : AbstractValidator<Command>
		{
			public CommandValidator() {
				RuleFor(x => x.Body).NotEmpty();
			}
			
		}

		public class Handler : IRequestHandler<Command, Result<CommentDto>>
		{
			private readonly DataContext _context;
			private readonly IUserAccessor _userAccessor;
			private readonly IMapper _mapper;

			public Handler(DataContext dataContext, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
				_mapper = mapper;
				_context = dataContext;
            }

			public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
			{
				var activity = await _context.Activities.FindAsync(request.ActivityId); 
				if (activity == null) { return null; }

				var user = await _context.Users
					.Include(p=> p.Photos)
					.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

				var comment = new Comment
				{
					Author = user,
					Activity = activity,
					Body = request.Body,
				};

				activity.Comments.Add(comment);

				var result = await _context.SaveChangesAsync() > 0;
				if (!result) { return Result<CommentDto>.Failure("Failed to add comment"); }
				return Result<CommentDto>.Success(_mapper.Map<CommentDto>(comment));
			}
		}
	}
}

﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PlayGen.SUGAR.Common.Authorization;
using PlayGen.SUGAR.Server.Authorization;
using PlayGen.SUGAR.Server.Core.Controllers;

namespace PlayGen.SUGAR.Server.Core.Authorization
{
	public class AuthorizationHandler : AuthorizationHandler<AuthorizationRequirement, int>
	{
		private readonly ActorClaimController _actorClaimDbController;
		private readonly ClaimController _claimDbController;

		public AuthorizationHandler(ActorClaimController actorClaimDbController, ClaimController claimDbController)
		{
			_actorClaimDbController = actorClaimDbController;
			_claimDbController = claimDbController;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement, int entityId)
		{
			return AuthorizationHandlerHelper.HandleRequirements(_claimDbController, _actorClaimDbController, context, requirement, entityId);
		}
	}

	public class AuthorizationHandlerWithoutEntity : AuthorizationHandler<AuthorizationRequirement, ClaimScope>
	{
		private readonly ActorClaimController _actorClaimDbController;
		private readonly ClaimController _claimDbController;

		public AuthorizationHandlerWithoutEntity(ActorClaimController actorClaimDbController,
					ClaimController claimDbController)
		{
			_actorClaimDbController = actorClaimDbController;
			_claimDbController = claimDbController;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement, ClaimScope scope)
		{
			var claim = _claimDbController.Get(requirement.ClaimScope, requirement.Name);
			if (claim != null && claim.ClaimScope == scope)
			{
				var claims = _actorClaimDbController.GetActorClaims(int.Parse(context.User.Identity.Name)).ToList();
				if (claims.Any(c => c.ClaimId == claim.Id))
				{
					context.Succeed(requirement);
				}
			}
			return Task.CompletedTask;
		}
	}
}
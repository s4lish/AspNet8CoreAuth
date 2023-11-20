using Microsoft.AspNetCore.Authorization;

namespace AspNet8Core.Authorization
{
    public class HRManagerProbationRequirment : IAuthorizationRequirement
    {
        public int ProbationMonths { get; }

        public HRManagerProbationRequirment(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }
    }

    public class HRManagerProbationRequirmentHandler : AuthorizationHandler<HRManagerProbationRequirment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirment requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "EmployementDate"))
                return Task.CompletedTask;

            if (DateTime.TryParse(context.User.FindFirst(x => x.Type == "EmployementDate")?.Value, out DateTime employementDate))
            {
                TimeSpan period = DateTime.Now - employementDate;
                if (period.Days > 30 * requirement.ProbationMonths)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}

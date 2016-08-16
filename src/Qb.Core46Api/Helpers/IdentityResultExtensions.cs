using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Qb.Core46Api.Helpers
{
    public static class IdentityResultExtensions
    {
        /// <summary>Returns erros formatted into string `{code}:{description} | {code}:{description} |...`</summary>
        public static string PrettyErrors(this IdentityResult res)
        {
            return string.Join(" | ", res.Errors.Select(e => $"{e.Code}:{e.Description}"));
        }
    }
}
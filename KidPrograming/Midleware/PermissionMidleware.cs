using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using KidPrograming.Core;
using KidPrograming.Core.Base;
using Microsoft.IdentityModel.Tokens;
namespace KidPrograming.Midleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _jwtSettings;
        public PermissionMiddleware(RequestDelegate next, JwtSettings jwtSettings)
        {
            _next = next;
            _jwtSettings = jwtSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }
            if (endpoint.Metadata.Any(meta => meta is Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute))
            {
                await _next(context);
                return;
            }

            var publicEndpoints = new HashSet<string>
                            {
                                "/api/auth/login-google"
                            };

            var path = context.Request.Path.Value?.ToLower();
            if (publicEndpoints.Any(p => path.StartsWith(p)))
            {
                await _next(context);
                return;
            }
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                throw new ErrorException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Token is missing");
            }

            var principal = ValidateToken(token);
            if (principal == null)
            {
                throw new ErrorException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Invalid or expired token");
            }
            var userRole = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (!HasPermission(endpoint, userRole))
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Access denied");
            }
            context.User = principal;
            await _next(context);
        }

        private ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey!);
            try
            {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true, 
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true, 
                ValidAudience = _jwtSettings.Audience, 
                ValidateLifetime = true, 
                ClockSkew = TimeSpan.Zero 
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);

                // Kiểm tra token hết hạn
                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
                    if (expClaim != null && long.TryParse(expClaim, out var exp))
                    {
                        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp);
                        if (expirationTime < DateTimeOffset.UtcNow)
                        {
                            return null; 
                        }
                    }
                }

                return principal;
            }
            catch
            {
                return null; 
            }
        }


        private bool HasPermission(Endpoint endpoint, string userRole)
        {
            // có gì bổ sung sau
            return true; 
        }
    }

}

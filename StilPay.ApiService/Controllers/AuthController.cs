using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.ApiService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SettingDAL _settingDAL = new SettingDAL();
        public AuthController(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        [HttpPost("token")]
        public IActionResult Token()
        {
            try
            {
                // Basic auth kontrolü
                if (HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    var authHeader = AuthenticationHeaderValue.Parse(HttpContext.Request.Headers["Authorization"]);
                    var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                    var username = credentials[0];
                    var password = credentials[1];

                    // Kullanıcı adı ve şifre kontrolü
                    if (ValidateUserCredentials(username, password))
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Name, username)
                            }),
                            Expires = DateTime.UtcNow.AddMinutes(5),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256Signature)
                        };

                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        var tokenString = tokenHandler.WriteToken(token);

                        return Ok(new { token = tokenString, expires_in = 5 });
                    }

                    var response = new ContentResult
                    {
                        StatusCode = 401,
                        Content = "{\"status\": 401, \"error\": \"Invalid username or password\"}",
                        ContentType = "application/json"
                    };

                    return response;
                }

                return BadRequest(new { status = 400, error = "Authorization header not found"});

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        private bool ValidateUserCredentials(string username, string password)
        {
            var basicAuths = _settingDAL.GetList(new List<FieldParameter>()
            {
                new FieldParameter("ParamType", FieldType.NVarChar, "ApiServiceBasicAuth")
            });

            return basicAuths != null && basicAuths.Any(x => x.ParamVal == string.Concat(username + ":" + password));
        }
    }
}

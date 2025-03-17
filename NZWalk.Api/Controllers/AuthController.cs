using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalk.Api.Models.DTO;
using NZWalk.Api.Repositories;

namespace NZWalk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;  //– Manages user creation, authentication, and roles.
        private readonly ITokenRepository tokenRepository;  //Handles JWT token generation.

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }
        //post: api/auth/Register
        [HttpPost]
        [Route("Register")] 
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto) //Accepts: RegisterRequestDto (username, password, roles).
                                                                                                    //Model Binding (FromBody) – Parses JSON request body.
        { //Creates a new user instance. 
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };
            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);   //Calls CreateAsync() to store user in ASP.NET Identity database.  

            if (identityResult.Succeeded)
            {
                //add role to user

                //  Checks if user registration was successful.
                //If roles exist, assigns them using AddToRolesAsync().
                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                    if (identityResult.Succeeded)
                    {
                        return Ok("User was registered! please login.");  //Returns success message if user & roles are created.
                    }
                }

            }
            return BadRequest("Somthing went Wrong");


        }


        //post: api/auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult>Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);
            if (user != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (checkPasswordResult)
                {
                    //get roles for this user

                    var roles = await userManager.GetRolesAsync(user);  //Retrieves assigned roles

                    if (roles != null) 
                    {

                        // create Token
                        //Calls tokenRepository.CreateJwtToken() to generate a JWT token.
                        var jwtToken = tokenRepository.CreateJwtToken(user, roles.ToList());

                        //Creates LoginResponseDto containing the token.
                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };

                        return Ok(response);

                    }               

                }

            }
            return BadRequest("Somthing went Wrong! Wrong Password or username");
        }


    }
}

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NetflixTitles.API.Entities;
using NetflixTitles.API.Models;
using NetflixTitles.API.Services;

namespace NetflixTitles.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IConfiguration _configuration;
        private INetflixTitlesRepository _netflixTitlesRepository;
        private ILogger<AuthenticationController> _logger;
        private IMapper _mapper;

        public AuthenticationController(IConfiguration configuration,
            INetflixTitlesRepository netflixTitlesRepository,
            ILogger<AuthenticationController> logger,
            IMapper mapper)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _netflixTitlesRepository = netflixTitlesRepository ?? throw new ArgumentNullException(nameof(netflixTitlesRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        [HttpPost("authenticate")]

        public async Task<ActionResult<string>> Authenticate(
            AuthenticationRequestBody authenticationRequestBody)
        {
            var user = await ValidateUserCredentials(
                authenticationRequestBody.UserName,
                authenticationRequestBody.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("user_name", user.UserName));
            claimsForToken.Add(new Claim("user_type", user.UserType!));

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);


            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);

        }

        private async Task<User?> ValidateUserCredentials(string? userName, string? password)
        {
            var passwordHasher = new PasswordHasher<User>();
            var validatedUser = await _netflixTitlesRepository.GetUserAsync(userName);

            if (validatedUser != null)
            {
                var result = passwordHasher.VerifyHashedPassword(validatedUser, validatedUser.Password, password);
                if (result == PasswordVerificationResult.Success)
                {
                    return validatedUser;
                }
            }

            return null;
        }

        [HttpPost("register")]

        public async Task<ActionResult> RegisterUser(RegisterUserDto registerUserDto)
        {
            var hasher = new PasswordHasher<RegisterUserDto>();
            if (registerUserDto.Password.Length >= 6 || registerUserDto.Password.Length <= 16)
            {
                var hashedPassword = hasher.HashPassword(registerUserDto, registerUserDto.Password);
                registerUserDto.Password = hashedPassword;
            }
            else
            {
                _logger.LogInformation($"Password must be between 6 and 16 characters");
                return BadRequest();
            }

            var finalUser = _mapper.Map<User>(registerUserDto);

            var userExists = await _netflixTitlesRepository.GetUserAsync(finalUser.UserName);
            if (userExists != null)
            {
                _logger.LogInformation($"user with UserName: {finalUser.UserName} already exists");
                return BadRequest();
            }

            _netflixTitlesRepository.AddUser(finalUser);
            await _netflixTitlesRepository.SaveChangesAsync();

            var createdUser = _mapper.Map<UserDto>(finalUser);

            return CreatedAtRoute("GetUser",
                new 
                {
                    controller = "User",
                    id = createdUser.UserId
                },
                createdUser);
        }
    }
}

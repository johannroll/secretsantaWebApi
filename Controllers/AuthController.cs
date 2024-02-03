using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApi.Dto;
using SecretSantaApi.Interfaces;
using SecretSantaApi.Migrations;
using SecretSantaApi.Models;
using SecretSantaApi.Utilities;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace SecretSantaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly TokenGenerator _TokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IAuthenticateRepository _authenticateRepository;
        private readonly IEmailService _emailService; 
        private readonly IEmailVerificationRepository _emailVerificationRepository;

        public AuthController(IUserRepository userRepository, TokenGenerator TokenGenerator, IMapper mapper, IAuthenticateRepository authenticateRepository,IEmailService emailService, IEmailVerificationRepository emailVerificationRepository)
        {

            _TokenGenerator = TokenGenerator;
            _userRepository = userRepository;
            _mapper = mapper;
            _authenticateRepository = authenticateRepository;
            _emailService = emailService;
            _emailVerificationRepository = emailVerificationRepository;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterUserAsync([FromBody] UserRegisterDto userRegisterDto)
        {
            if (userRegisterDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userRepository.EmailExistsAsync(userRegisterDto.UserEmail))
                return BadRequest("Email already in use.");

            var token = GenerateVerificationToken();
            var emailVerificationToken = new EmailVerificationToken
            {
                Token = token,
                Email = userRegisterDto.UserEmail
            };

            if (!await _emailVerificationRepository.SaveTokenAsync(emailVerificationToken))
            {
                return BadRequest("Unable to save verification token");
            }

            //var verificationLink = Url.Action(nameof(VerifyEmailAsync), "Auth", new { token }, Request.Scheme);
            //var verificationLink = "https://secretsantaapi.azurewebsites.net/api/Auth/Verify-email?token=" + token;
            var verificationLink = "https://orange-pebble-03f6b0910.4.azurestaticapps.net/#!/signup-password?token=" + token; 

            var verifyEmail = new VerifyEmailDto();
            verifyEmail.VerifyLink = verificationLink;
            verifyEmail.To = userRegisterDto.UserEmail;

            await _emailService.SendVerificationEmail(verifyEmail); 

            return Ok("Verification email sent");
        }
        [HttpGet("Verify-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyEmailAsync(string token)
        {
            var verificationToken = await _emailVerificationRepository.GetTokenAsync(token);
            if (verificationToken == null || verificationToken.IsUsed || verificationToken.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest("Invalid or expired token");
            }

            await _emailVerificationRepository.MarkTokenAsUsedAsync(verificationToken);

            return Ok(new { Email = verificationToken.Email });
        }

        [HttpPost("set-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetPasswordAsync(SetPasswordDto setpassworddto)
        {

            var users = await _userRepository.GetUsersAsync();
            if (users.Any(u => u.UserEmail.Trim().ToLower() == setpassworddto.UserEmail.Trim().ToLower()))
            {
                ModelState.AddModelError("", "registration request received");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            var userentity = _mapper.Map<User>(setpassworddto);

            var result = await _authenticateRepository.RegisterUserAsync(userentity, setpassworddto.Password);
            if (!result)
                return BadRequest("registration failed");

             //optionally, generate a jwt token upon successful registration
             //var token = _tokengenerator.GenerateJwtToken(userEntity);

            return Ok("Password set successfully.");
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginUserAsync([FromBody] UserLoginDto userLoginDto)
        {
            if (userLoginDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var userEntity = _mapper.Map<User>(userLoginDto);

            var user = await _authenticateRepository.LoginUserAsync(userEntity.UserEmail, userEntity.Password);
            if (user == null)
                return BadRequest("Invalid email or password");

            var token = _TokenGenerator.GenerateJwtToken(user);
            return Ok(new { UserName = user.UserName, UserId = user.UserId, UserEmail = user.UserEmail, Token = token });
        }

        [HttpPost("update-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePasswordAsync([FromBody] UpdatePasswordDto updatePasswordDto)
        {
            if (updatePasswordDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            // Retrieve the current user (assuming the user ID is stored in the token claims)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserAsync(int.Parse(userId));

            if (user == null)
                return NotFound("User not found");

            bool updateResult = await _userRepository.UpdatePasswordAsync(user, updatePasswordDto.OldPassword, updatePasswordDto.NewPassword);
            if (!updateResult)
                return BadRequest("Invalid old password or failed to update password");

            return Ok("Password updated successfully");
        }

        private string GenerateVerificationToken()
        {
            return Guid.NewGuid().ToString();
        }

        [HttpPost("request-password-reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
               
                return BadRequest("Password reset request failed.");
            }

            var resetToken = _TokenGenerator.GeneratePasswordResetToken();
            var resetTokenExpiry = _TokenGenerator.GenerateTokenExpiryTime();
            
            if (!await _userRepository.SaveUserResetToken(user.UserId, resetToken, resetTokenExpiry))
            {
                return BadRequest("Token could not be saved");
            }

            var resetLink = "https://orange-pebble-03f6b0910.4.azurestaticapps.net/#!/reset-password?token=" + resetToken;

            var resetEmail = new PasswordResetEmailDto();
            resetEmail.To = request.Email;
            resetEmail.ResetLink = resetLink;

            
            await _emailService.PasswordResetEmail(resetEmail);


            return Ok("Password reset link has been sent to your email.");
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto resetDto)
        {
            // Validate the token and find the associated user
            var user = await _userRepository.GetUserByResetTokenAsync(resetDto.Token);
            if (user == null || !IsTokenValid(user, resetDto.Token))
            {
                return BadRequest("Invalid or expired token.");
            }

            // Update the user's password
            var result = await _userRepository.ResetPasswordAsync(user, resetDto.Password);
            if (!result)
            {
                return BadRequest("Failed to reset password.");
            }

            // Mark the token as used or expired
            if (!await _userRepository.UpdateUserPasswordResetTokenStatus(user))
            {
                return BadRequest("Failed to reset password token status.");
            }

            return Ok("Password has been reset successfully.");
        }

        private bool IsTokenValid(User user, string token)
        {
            return user.PasswordResetToken == token && user.PasswordResetTokenExpires.HasValue && user.PasswordResetTokenExpires > DateTime.UtcNow;
        }








    }
}

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Services;
using Entities.Models;
using System.Diagnostics.Metrics;
using AutoMapper;
using DTO;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyShop.Controllers
{

    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserService _userService;
        IMapper _mapper;
        private ILogger<UserController> _logger;

        public UserController(IUserService userService,IMapper mapper, ILogger<UserController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }
        // GET: api/<UserController>
        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/<UserController>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] FullUserDTO userToAdd)
        {
            User user = _mapper.Map<FullUserDTO, User>(userToAdd);
            User newUser = await _userService.AddUser(user);

            if (newUser == null)
            {
                _logger.LogError("userController: error creating user");
                return BadRequest();
            }
            else
            {
                ReturnUserDTO usersDTO = _mapper.Map<User, ReturnUserDTO>(newUser);

                if (usersDTO.UserName == null)
                {
                    return Conflict();
                }
                else
                {
                    _logger.LogInformation($"UserController: created user {user.UserName}");
                    return CreatedAtAction(nameof(Get), new { id = user.UserId }, usersDTO);
                }
            }

        }

        [HttpPost]
        [Route("Password")]
        public ActionResult<int> CheckPassword([FromBody] string password)
        {
            int result = _userService.CheckPassword(password);
            if (result < 0)
            {
                return BadRequest();
            }
            return result;
        }

        // POST api/<UserController>/Login
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Login([FromBody] LoginUserDTO loginUser )
        {
            User user = await _userService.Login(loginUser.Password,loginUser.UserName);
            ReturnUserDTO usersDTO = _mapper.Map<User, ReturnUserDTO>(user);
            if (usersDTO != null)
            {
                // יצירת טוקן JWT
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                    // ניתן להוסיף תביעות נוספות לפי הצורך
                };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(HttpContext.RequestServices
                        .GetRequiredService<IConfiguration>()["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: HttpContext.RequestServices
                        .GetRequiredService<IConfiguration>()["Jwt:Issuer"],
                    audience: null,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // אם לא נוצר טוקן תקין, החזר 401
                if (string.IsNullOrEmpty(tokenString))
                {
                    return Unauthorized();
                }

                // שליחת הטוקן ב-cookie מאובטח
                Response.Cookies.Append("access_token", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // חובה בפרודקשן!
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                _logger.LogInformation($"Login attemped with user {loginUser.UserName} and password {loginUser.Password}");
                return Ok(usersDTO);
            }
            _logger.LogInformation($"Login failed with user {loginUser.UserName} and password {loginUser.Password}" );
            return Unauthorized();
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] FullUserDTO userToUpdate)
        {
            User u = _mapper.Map<FullUserDTO, User>(userToUpdate);
            u.UserId = id;

            User user = await _userService.UpdateUser(id, u);
            if (user == null)
            {
                _logger.LogError($"userController: error updating user {u.UserName} ");
                return BadRequest();
            }
            else
            {
                ReturnUserDTO usersDTO = _mapper.Map<User, ReturnUserDTO>(user);
                if (usersDTO.UserName==null)
                {
                    return Conflict();
                }
                else{
                    _logger.LogInformation($"userController: updating user {u.UserName}");
                    return Ok(usersDTO);
                }
            }
        }
    }
}

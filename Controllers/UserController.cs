using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApi.Dto;
using SecretSantaApi.Interfaces;
using SecretSantaApi.Models;

namespace SecretSantaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IGiftListRepository _listRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository,IGiftListRepository listRepository,IPersonRepository personRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _listRepository = listRepository;
            _personRepository = personRepository;
            _mapper = mapper;
        }

        // DELETE: api/User/{userId}
        [HttpDelete("delete/{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (!await _userRepository.UserExistsAsync(userId))
                return NotFound("No user found");

            var userToDelete = await _userRepository.GetUserAsync(userId);
            if (userToDelete == null)
            {
                return NotFound("User not found");
            }

            var userListsToDelete = await _listRepository.GetListsByUserIdAsync(userId);

            if (userListsToDelete.Any())
            {
                foreach (var list in userListsToDelete)
                {
                    var peopleOnListToDelete = await _personRepository.GetPeopleByListIdAsync(list.ListId);
                    if (peopleOnListToDelete.Any())
                    {
                        var deletedPeople = await _personRepository.DeleteAllPeopleFromAListAsync(peopleOnListToDelete.ToList());
                        if (!deletedPeople)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting people from list");
                        }
                    }
                }

                var deletedLists = await _listRepository.DeleteAllUserListsAsync(userListsToDelete.ToList());
                if (!deletedLists)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting user lists");
                }
            }

            var deletedUser = await _userRepository.DeleteUserAsync(userToDelete);
            if (!deletedUser)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting the user");
            }

            return NoContent();

        }

        // PUT: api/User/{userId}
        [HttpPut("update/{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserDto updatedUser)
        {
            if (updatedUser == null)
                return BadRequest(ModelState);

            if (!await _userRepository.UserExistsAsync(userId))
                return NotFound("User not found");

            if (!ModelState.IsValid)
                return BadRequest();

            updatedUser.UserId = userId;
            var userMap = _mapper.Map<User>(updatedUser);

            if (!await _userRepository.UpdateUserAsync(userMap))
            {
                ModelState.AddModelError("", "Something went wrong updating user");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok("User updated");
        }

        // GET: api/User
        [HttpGet("get-users")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUsers()
        { 
            var users = await _userRepository.GetUsersAsync();
            if (users == null || !users.Any())
            {
                return NotFound("No users found");
            }
            var usersDto = _mapper.Map<List<UserDto>>(users);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(usersDto);
        }

        // GET: api/User/{userId}
        [HttpGet("{userId}", Name = "GetUser")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(int userId)
        {
            if (!await _userRepository.UserExistsAsync(userId))
                return NotFound("User not found");
            
            var user = await _userRepository.GetUserAsync(userId);
            var userDto = _mapper.Map<UserDto>(user);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(userDto);
        }
    }
}

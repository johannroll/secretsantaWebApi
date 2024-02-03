using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApi.Dto;
using SecretSantaApi.Interfaces;
using SecretSantaApi.Models;
using System.Security.Claims;

namespace SecretSantaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftListController : ControllerBase
    {
        private readonly IGiftListRepository _listRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GiftListController(IGiftListRepository listRepository,IPersonRepository personRepository,IUserRepository userRepository, IMapper mapper)
        {
            _listRepository = listRepository;
            _personRepository = personRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // POST: api/GiftList/{userId}/list
        [HttpPost("createlist/{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateList(int userId, [FromBody] GiftListDto listCreate)
        {
            if (listCreate == null)
                return BadRequest(ModelState);

            if (!await _userRepository.UserExistsAsync(userId))
                return NotFound("User not found");

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Int32.Parse(currentUser) != userId)
            {
                ModelState.AddModelError("", "User id does not match");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            var userLists = await _listRepository.GetListsByUserIdAsync(userId);    
            if (userLists.Any(l => l.Title.Trim().ToLower() == listCreate.Title.TrimEnd().ToLower()))
            {
                ModelState.AddModelError("", "List already exists");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            listCreate.UserId = userId;
            var listMap = _mapper.Map<GiftList>(listCreate);
            var created = await _listRepository.CreateListAsync(listMap);

            if (!created)
            {
                ModelState.AddModelError("", "Something went wrong while saving the list");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            var listDto = _mapper.Map<GiftListDto>(listMap);
            return CreatedAtRoute(nameof(GetList), new { listId = listMap.ListId }, listDto);
        }

        // DELETE: api/GiftList/{listId}
        [HttpDelete("deletelist/{listId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteList(int listId)
        {
             
            if (!await _listRepository.ListExistsAsync(listId))
                return NotFound();

            var listToDelete = await _listRepository.GetListAsync(listId);
            var peopleOnListToDelete = await _personRepository.GetPeopleByListIdAsync(listId);

            if (peopleOnListToDelete.Any())
            {
                var deletedPeople = await _personRepository.DeleteAllPeopleFromAListAsync(peopleOnListToDelete.ToList());
                if (!deletedPeople)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting people from list");
                }
            }

            //if (!await _personRepository.DeleteAllPeopleFromAListAsync(peopleOnListToDelete.ToList()))
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting people from list");
            //}

            if (!await _listRepository.DeleteListAsync(listToDelete))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting the list");
            }

            return NoContent();

        }

        // PUT: api/GiftList/{listId}
        [HttpPut("updatelist/{listId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateList(int listId, [FromBody] GiftListDto updatedList)
        {
            if (updatedList == null)
                return BadRequest(ModelState);

            //if (listId != updatedList.ListId)
            //    return BadRequest("List ID mismatch");

            var existingList = await _listRepository.GetListAsync(listId);
            if (existingList == null)
            {
                return NotFound();
            }


            var userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var userLists = await _listRepository.GetListsByUserIdAsync(userId);
            if (userLists.Any(l => l.Title.Trim().ToLower() == updatedList.Title.TrimEnd().ToLower()))
            {
                ModelState.AddModelError("", "List title already in use");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            _mapper.Map(updatedList, existingList);

            existingList.ListId = listId;
            existingList.UserId = userId;
           

            if (!await _listRepository.UpdateListAsync(existingList))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating the list");
            }

            return Ok(_mapper.Map<GiftListDto>(existingList));
        }

        // GET: api/GiftList/{userId}/lists
        [HttpGet("get-user-lists/{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GiftListDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListsOfAUser(int userId)
        {
            if (!await _userRepository.UserExistsAsync(userId)) // Check if user exists
            {
                return NotFound("User not found");
            }

            var lists = await _listRepository.GetListsByUserIdAsync(userId);
            if (lists == null || !lists.Any()) // Check if lists are found
            {
                return NotFound("No lists found for the user");
            }

            var listsDtos = _mapper.Map<List<GiftListDto>>(lists);

            return Ok(listsDtos);
        }

        // GET: api/GiftList/{listId}
        [HttpGet("getlist/{listId}", Name = "GetList")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GiftList))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetList(int listId)
        {
            var list = await _listRepository.GetListAsync(listId);
            if (list == null) // Check if the list exists
            {
                return NotFound("List not found");
            }
            
            var listDto = _mapper.Map<GiftListDto>(list);

            return Ok(listDto);
        }
    }
}

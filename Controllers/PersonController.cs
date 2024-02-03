using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApi.Dto;
using SecretSantaApi.Interfaces;
using SecretSantaApi.Migrations;
using SecretSantaApi.Models;
using System.Collections.Generic;

namespace SecretSantaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonRepository _personRepository;
        private readonly IGiftListRepository _listRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public PersonController(IPersonRepository personRepository,IGiftListRepository listRepository, IMapper mapper, IEmailService emailService)
        {
            _personRepository = personRepository;
            _listRepository = listRepository;
            _mapper = mapper;
            _emailService = emailService;
        }

        // POST: api/Person/{listId}/person
        [HttpPost("create/{listId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreatePerson(int listId, [FromBody] PersonDto personCreate)
        {
            if (personCreate == null)
                return BadRequest(ModelState);

            if (!await _listRepository.ListExistsAsync(listId))
                return NotFound("List not found");

            var person = await _personRepository.GetPeopleByListIdAsync(listId);
            if (person.Any(p => p.Name.Trim().ToLower() == personCreate.Name.TrimEnd().ToLower()))
            {
                ModelState.AddModelError("", "Person already added to list");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            personCreate.ListId = listId;
            var personMap = _mapper.Map<Person>(personCreate);
            var created = await _personRepository.CreatePersonAsync(personMap);
            if (!created)
            {
                ModelState.AddModelError("", "Something went wrong while saving the person");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            var personDto = _mapper.Map<PersonDto>(personMap);
            return CreatedAtRoute(nameof(GetPerson), new { personId = personMap.PersonId }, personDto);
        }

        // PUT: api/Person/{personId}
        [HttpPut("update/{personId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePersonAsync(int personId, [FromBody] PersonUpdateDto updatedPerson)
        {
            if (updatedPerson == null)
                return BadRequest(ModelState);

            //if (personId != updatedPerson.PersonId)
            //    return BadRequest(ModelState);

            if (!await _personRepository.PersonExistsAsync(personId))
                return NotFound("Person not found");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var person = await _personRepository.GetPersonAsync(personId);

            var people = await _personRepository.GetPeopleByListIdAsync(person.ListId);
            foreach (var personGiverGiftee in people)  
            {
                if (personGiverGiftee.giverGiftee.Trim().ToLower() == person.Name.TrimEnd().ToLower())
                {
                    if (personGiverGiftee.giverGiftee.Trim().ToLower() != updatedPerson.Name.TrimEnd().ToLower())
                    {
                        personGiverGiftee.giverGiftee = updatedPerson.Name;
                        var personGiverGifteeMap = _mapper.Map<Person>(personGiverGiftee);
                        if (!await _personRepository.UpdatePersonAsync(personGiverGifteeMap))
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating GiverGiftee");
                        }

                    }
                }
            }

            updatedPerson.PersonId = personId;
            var personMap = _mapper.Map<Person>(updatedPerson);
            if (!await _personRepository.UpdatePersonAsync(personMap))
            { 
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating person");
            }

            return Ok("Person updated");
        }

        // GET: api/Person
        [HttpGet("get-people")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Person>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllPeople()
        {
            var people = await _personRepository.GetAllPeopleAsync();
            if (people == null || !people.Any())
            {
                return NotFound("No people found");
            }
            var peopleDto = _mapper.Map<List<PersonDto>>(people);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(peopleDto);
        }

        // GET: api/Person/{personId}
        [HttpGet("{personId}", Name = "GetPerson")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PersonDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPerson(int personId)
        {
            if (!await _personRepository.PersonExistsAsync(personId))
                return NotFound("Person not found");

            var person = await _personRepository.GetPersonAsync(personId);
            if (person == null)
            {
                return NotFound("No person found");
            }
            var personDto = _mapper.Map<PersonDto>(person);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(personDto);
        }

        // GET: api/Person/{listId}/people
        [HttpGet("{listId}/people-on-list")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PersonDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPeopleFromAList(int listId)
        {
            if (!await _listRepository.ListExistsAsync(listId))
                return NotFound("List not found");
         
            var people = await _personRepository.GetPeopleByListIdAsync(listId);
            if (people == null || !people.Any())
            {
                return NotFound("No people found");
            }

            var peopleDto = _mapper.Map<List<PersonDto>>(people);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(peopleDto);
        }

        [HttpPost("send-secret-santas/{listId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PersonDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendSecretSantaEmail(int listId)
        {
            if (!await _listRepository.ListExistsAsync(listId))
                return NotFound("List not found");

            var people = await _personRepository.GetPeopleByListIdAsync(listId);
            if (people == null || !people.Any())
            {
                return NotFound("No people found");
            }

            var peopleDto = _mapper.Map<List<PersonDto>>(people);

            await _emailService.SendSecretSantasEmail(peopleDto);

            return Ok("Secret santas email sent");
        }

        // DELETE: api/Person/{personId}
        [HttpDelete("delete/{personId}")]
        [Authorize]    
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePerson(int personId)
        {

            if (!await _personRepository.PersonExistsAsync(personId))
                return NotFound("Person Not Found");

            var personToDelete = await _personRepository.GetPersonAsync(personId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _personRepository.DeletePersonAsync(personToDelete))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting person from list");
            }

            return NoContent();

        }


    }
}
   
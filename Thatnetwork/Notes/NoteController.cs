using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Thatnetwork.Middlewares;
using Thatnetwork.Notes.Dtos;
using Thatnetwork.Photos;
using Thatnetwork.Users;

namespace Thatnetwork.Notes
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly ILogger<NoteController> _logger;
        private readonly NoteService _noteService;
        private readonly PhotoService _photoService;

        public NoteController(ILogger<NoteController> logger, NoteService noteService, PhotoService photoService)
        {
            _logger = logger;
            _noteService = noteService;
            _photoService = photoService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NoteDto?>> GetNoteById(int id)
        {
            NoteDto? noteDto = await _noteService.GetNoteByIdAsync(id);
            if (noteDto == null)
            {
                return NotFound();
            }
            
            return Ok(noteDto);
        }

        [HttpGet("comments/{noteId:int}")]
        public async Task<ActionResult<List<NoteDto>>> GetComments(int noteId)
        {
            NoteDto? noteDto = await _noteService.GetNoteByIdAsync(noteId);
            if (noteDto == null)
            {
                return NotFound();
            }

            return Ok(await _noteService.GetCommentsAsync(noteId));
        }

        [HttpPost(), Authorize()]
        public async Task<ActionResult> AddNote([FromBody] AddNoteDto addNoteDto)
        {
            if (addNoteDto.IsEmpty)
            {
                return UnprocessableEntity("Нельзя опубликовать пустой пост");
            }

            if (addNoteDto.ParentNoteId != null)
            {
                NoteDto? noteDto = await _noteService.GetNoteByIdAsync((int)addNoteDto.ParentNoteId);
                if (noteDto == null)
                {
                    return UnprocessableEntity($"Нельзя оставить комментарий, т.к. не найден пост с id: {addNoteDto.ParentNoteId}");
                }
            }

            User currUser = HttpContext.GetCurrentUser();
            foreach (int photoId in addNoteDto.PhotoIds)
            {
                Photo? photo = _photoService.GetPhotoById(photoId);
                if (photo == null)
                {
                    return UnprocessableEntity($"Не найдено фото с id: {photoId}");
                }
                if (photo.Owner.Id != currUser.Id)
                {
                    return UnprocessableEntity("Нельзя добавлять чужие фотографии в пост");
                }
            }

            await _noteService.AddNoteAsync(addNoteDto, currUser);
            return Ok();
        }

        [HttpPost("get-notes")]
        public async Task<ActionResult<GetNotesResultDto>> GetNotes()
        {
            var notes = await _noteService.GetAllNotes();
            var result = new GetNotesResultDto() { Notes = notes };

            return Ok(result);
        }

        [HttpPost("get-note-entities")]
        public async Task<ActionResult<List<Note>>> GetNoteEntities()
        {
            var notes = await _noteService.GetAllNoteEntities();
            return Ok(notes);
        }
    }
}

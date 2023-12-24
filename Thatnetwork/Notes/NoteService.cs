using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.Controllers;
using System.Xml;
using Thatnetwork.Entities;
using Thatnetwork.Notes.Dtos;
using Thatnetwork.Photos;
using Thatnetwork.Users;

namespace Thatnetwork.Notes
{
    public class NoteService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<NoteService> _logger;

        public NoteService(AppDbContext dbContext, IMapper mapper, ILogger<NoteService> logger) {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<NoteDto?> GetNoteByIdAsync(int id)
        {
            var note = await _dbContext.Notes.Include(n => n.Children).SingleOrDefaultAsync(n => n.Id == id);
            if (note == null)
            {
                return null;
            }
            return MapNoteToDto(note);
            //return _mapper.Map<NoteDto>(note);
        }

        public async Task<List<NoteDto>> GetCommentsAsync(int noteId)
        {
            var note = await _dbContext.Notes.Include(n => n.Children).SingleAsync(n => n.Id == noteId);
            _logger.LogInformation($"Get comments async. children count: {note.Children.Count}");
            return note.Children.Select(MapNoteToDto).ToList();
            //return _mapper.Map<List<NoteDto>>(note.Children);
        }

        public NoteDto MapNoteToDto(Note note)
        {
            var noteDto = _mapper.Map<NoteDto>(note);
            noteDto.CommentCount = _CalculateCommentCount(note);
            return noteDto;
        }

        private int _CalculateCommentCount(Note note)
        {
            var comments = _dbContext.Notes
                .Where(n => n.Parent == note)
                .Include(n => n.Children)
                .ToList();
            if (!comments.Any())
            {
                return 0;
            }

            int count = 0;
            foreach (Note childNote in note.Children)
            {
                count += 1;
                count += _CalculateCommentCount(childNote);
            }
            return count;
        }

        //private int _GetNoteWithChildrenCount(Note note)
        //{
        //    int count = 1;
        //    foreach (Note childNote in note.Children)
        //    {
        //        count += _GetNoteWithChildrenCount(childNote);
        //    }
        //    return count;
        //}

        public async Task<List<NoteDto>> GetAllNotes()
        {
            var notes = await _dbContext.Notes
                .Include(n => n.Parent)
                .Where(n => n.Parent == null)
                .ToListAsync();
            notes.Reverse();
            var noteDtos = notes.Select(MapNoteToDto).ToList();
            return noteDtos;
        }

        public async Task<List<Note>> GetAllNoteEntities()
        {
            var notes = await _dbContext.Notes.ToListAsync();
            notes.Reverse();
            return notes;
        }

        public async Task AddNoteAsync(AddNoteDto addNoteDto, User creator)
        {
            List<Photo> photos = await _dbContext.Photos
                .Where(p => addNoteDto.PhotoIds.Contains(p.Id))
                .ToListAsync();

            Note? parent = null;
            if (addNoteDto.ParentNoteId != null)
            {
                parent = _dbContext.Notes.Single(n => n.Id == addNoteDto.ParentNoteId);
            }
            Note note = new Note { Text = addNoteDto.Text, Creator = creator, Photos = photos, Parent = parent };
            await _dbContext.Notes.AddAsync(note);
            await _dbContext.SaveChangesAsync();
        }
    }
}

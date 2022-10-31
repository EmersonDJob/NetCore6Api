using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCore6Api.Models;
using System.Linq.Expressions;

namespace NetCore6Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieesController : ControllerBase
    {
        private readonly MovieContext _dbContext;

        public MovieesController(MovieContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            if(_dbContext.Movies == null)
            {
                return NotFound();
            }
            return await _dbContext.Movies.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            if(_dbContext.Movies == null)
            {
                return NotFound();    
            }
            var movie = await _dbContext.Movies.FindAsync(id);
            if(movie == null)
            {
                return NotFound();
            }
            return movie;
        }

        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            if(movie.Id == 0)
            {
                _dbContext.Movies.Add(movie);
            }
            else
            {
                var movieDb = _dbContext.Movies.Find(movie.Id);
                if (movieDb == null)
                    return new JsonResult(NotFound());
                movieDb = movie;
            }
           
            await _dbContext.SaveChangesAsync();
            return new JsonResult(Ok(movie));
            
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> PutMovie( int id, Movie movie)
        {
            if(id != movie.Id)
            {
                return BadRequest();
            }
            _dbContext.Entry(movie).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!MovieExits(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        private bool MovieExits(long id)
        {
            return (_dbContext.Movies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            if(_dbContext.Movies == null)
            {
                return NotFound();
            }
            var movie = await _dbContext.Movies.FindAsync(id);
            if(movie == null)
            {
                return NoContent();
            }
            _dbContext.Movies.Remove(movie);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamsApi.Models;

namespace TeamsApi.Controllers
{
    [Route("api/Players")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly TeamContext _context;

        public PlayersController(TeamContext context)
        {
            _context = context;
        }

        //QUERY ALL PLAYERS
        // GET: api/Players
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            return await _context.Players.ToListAsync();
        }

        //QUERY PLAYERS BY ID
        // GET: api/Players/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayer(long id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
            {
                return NotFound("Player Not Found");
            }

            return player;
        }

        //QUERY PLAYERS BY LAST NAME
        //GET: api/Players/name
        [HttpGet("name/{lastName}")]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayersByLastName(string lastName)
        {
            List<Player> players = await _context.Players.ToListAsync();
            List<Player> result = new List<Player>();
            foreach (var player in players)
            {
                if(player.LastName.ToLower().Equals(lastName.ToLower()))
                {
                    result.Add(player);
                }
            }
            return Ok(result);
        }

        //QUERY ALL PLAYERS ON A TEAM
        //GET: api/Players/name
        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayersByTeam(long teamId)
        {
            List<Player> players = await _context.Players.ToListAsync();
            List<Player> result = new List<Player>();
            foreach (var player in players)
            {
                if (player.TeamId.Equals(teamId))
                {
                    result.Add(player);
                }
            }
            return Ok(result);
        }

        //REPLACE A PLAYER
        // PUT: api/Players/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayer(long id, Player player)
        {
            if (id != player.Id)
            {
                return BadRequest("Invalid Player Id");
            }

            _context.Entry(player).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(id))
                {
                    return NotFound("Player Not Found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(player.FirstName + " " + player.LastName + " Has Been Edited");
        }

        //CREATE A NEW PLAYER
        // POST: api/Players
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(Player player)
        {
            if(PlayerExists(player.Id))
            {
                return BadRequest("Player Already Exists");
            }
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
        }

        //DELETE A PLAYER
        // DELETE: api/Players/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(long id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound("Player Not Found");
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return Ok(player.FirstName + " " + player.LastName + " Deleted");
        }

        private bool PlayerExists(long id)
        {
            return _context.Players.Any(e => e.Id == id);
        }

    }
}

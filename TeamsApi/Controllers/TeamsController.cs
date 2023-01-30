using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TeamsApi.Models;
using System.Linq;
using System.Numerics;

namespace TeamsApi.Controllers
{
    [Route("api/Teams")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly TeamContext _context;

        public TeamsController(TeamContext context)
        {
            _context = context;
        }

        //QUERY ALL TEAMS
        // GET: api/Teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _context.Teams.ToListAsync();
        }

        //SORT TEAMS
        // GET: api/Teams
        [HttpGet("sort")]
        public async Task<ActionResult<IEnumerable<Team>>> SortTeams(string orderBy = "?")
        {          
            if(orderBy.ToLower().Equals("name"))
            {
                return await _context.Teams.OrderBy(team => team.Name).ToListAsync();
            }
            else if(orderBy.ToLower().Equals("location"))
            {
                return await _context.Teams.OrderBy(team => team.Location).ToListAsync();
            }
            else
            {
                return BadRequest("Invalid Search Query");
            }  
        }

        //QUERY TEAM BY ID
        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(long id)
        {
            Team team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                return NotFound("Team Not Found");
            }

            return team;
        }

        //CREATE A NEW TEAM
        // POST: api/Teams
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(Team team)
        {
            foreach(Team t in _context.Teams)
            {
                if(t.Name== team.Name && t.Location== team.Location)
                {
                    return BadRequest("Team Already Exists");
                }
            }
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }

        //REPLACE A TEAM
        // PUT: api/Teams/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(long id, Team team)
        {
            if (id != team.Id)
            {
                return BadRequest("Team Not Found");
            }
            _context.Entry(team).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(team.Location + " " + team.Name + " Has Been Modified");
        }

        //ADD A PLAYER TO A TEAM
        // PUT: api/Teams/5/player/5/add
        [HttpPut("{teamId}/player/{playerId}/add")]
        public async Task<ActionResult> AddPlayerFromTeam(long teamId, long playerId)
        {
            Team team = await _context.Teams.FindAsync(teamId);
            if (team == null)
            {
                return BadRequest("Team Not Found");
            }
            Player player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                return BadRequest("Player Not Found");
            }       
            if (player.TeamId == team.Id)
            {
                return BadRequest("The Player Is Already On That Team");
            }
            if (player.TeamId != null)
            {
                Team oldTeam = await _context.Teams.FindAsync(player.TeamId);
                oldTeam.numPlayers--;
            }
            if (team.numPlayers == 8)
            {
                return BadRequest("The Team Is Full");
            }
            
            player.TeamId = team.Id;
            team.numPlayers++;
            await _context.SaveChangesAsync();
            return Ok(player.FirstName + " " + player.LastName + " Added To " + team.Location + " " + team.Name);

        }

        //REMOVE A PLAYER FROM A TEAM
        // PUT: api/Teams/5/player/5/remove
        [HttpPut("{teamId}/player/{playerId}/remove")]
        public async Task<ActionResult> RemovePlayerFromTeam(long teamId, long playerId)
        {
            Team team = await _context.Teams.FindAsync(teamId);
            if (team == null)
            {
                return BadRequest("Team Not Found");
            }
            Player player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                return BadRequest("Player Not Found");
            }
            player.TeamId = 0;
            team.numPlayers -= 1;
            await _context.SaveChangesAsync();

            return Ok(player.FirstName + " " + player.LastName + " Removed From " + team.Location + " " + team.Name);

        }
       
        //DELETE A TEAM
        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(long id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound("Team Not Found");
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return Ok(team.Location + " " + team.Name + " Has Been Deleted");
        }

        private bool TeamExists(long id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
        

    }
}

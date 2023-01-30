using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TeamsApi.Models
{
    public class Services
    {
        private readonly TeamContext _context;

        public Services(TeamContext context)
        {
            _context = context;
        }

       
    }
}

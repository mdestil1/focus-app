using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudyApp.Server.Models;
using StudyApp.Server.Data;

namespace StudyApp.Server
{
    public class StudySessionService
    {
        private readonly AppDBContext _context;

        public StudySessionService(AppDBContext context)
        {
            _context = context;
        }

        public StudySession GetStudySession(string date, string userId, string token)
        {
            return _context.StudySessions
                .FirstOrDefault(s =>
                    s.StudyDate == date &&
                    s.UserId == userId &&
                    s.Token == token);
        }
    }
}

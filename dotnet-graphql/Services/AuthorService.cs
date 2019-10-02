using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet_graphql.Data;
using dotnet_graphql.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_graphql.Services
{
    public class AuthorService
    {
        private readonly AppDbContext _appDBContext;
        public AuthorService(AppDbContext context)
        {
            _appDBContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Author>> GetAuthors()
        {
            return await _appDBContext.Authors.ToListAsync();
        }

        public async Task<Author> GetAuthor(int id)
        {
            return await _appDBContext.Authors.SingleAsync(b => b.Id == id);
        }

    }
}

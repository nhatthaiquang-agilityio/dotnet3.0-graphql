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
        private readonly AppDbContext _appDbContext;
        public AuthorService(AppDbContext context)
        {
            _appDbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Get Authors
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Author>> GetAuthors()
        {
            return await _appDbContext.Authors.ToListAsync();
        }

        /// <summary>
        /// Get Author
        /// </summary>
        /// <param name="id">author Id.</param>
        /// <returns></returns>
        public async Task<Author> GetAuthor(int id)
        {
            return await _appDbContext.Authors.SingleAsync(b => b.Id == id);
        }

    }
}

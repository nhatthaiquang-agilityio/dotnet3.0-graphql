using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_graphql.Data;
using dotnet_graphql.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_graphql.Services
{
    public class BookService
    {
        private readonly AppDbContext _appDbContext;
        public BookService(AppDbContext context)
        {
            _appDbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Get all books.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Book>> GetBooks()
        {
            return await _appDbContext.Books.Include(a => a.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(c => c.Category).AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Get a book.
        /// </summary>
        /// <param name="id">book Id.</param>
        /// <returns></returns>
        public async Task<Book> GetBook(int id)
        {
            return await _appDbContext.Books.Include(a => a.Author)
                .Include(c => c.BookCategories)
                    .ThenInclude(c => c.Category).AsNoTracking()
                .SingleAsync(b => b.Id == id);
        }

        /// <summary>
        /// Create the book.
        /// </summary>
        /// <param name="bookViewModel"></param>
        /// <returns></returns>
        public async Task<Book> Create(BookViewModel bookViewModel)
        {
            var book = new Book
            {
                BookName = bookViewModel.BookName,
                Price = bookViewModel.Price,
                AuthorId = bookViewModel.AuthorId
            };

            // has Categories
            if (bookViewModel.BookCategories != null)
            {
                // update bookcategory
                var categories = await _appDbContext.Categories
                    .Where(c => bookViewModel.BookCategories.Contains(c.CategoryName)).ToListAsync();

                book.BookCategories = new List<BookCategory>();

                // and then add new bookcategory
                foreach (var category in categories)
                {
                    book.BookCategories.Add(new BookCategory
                    {
                        CategoryId = category.CategoryId,
                        BookId = book.Id
                    });
                }
            }

            await _appDbContext.Books.AddAsync(book);
            await _appDbContext.SaveChangesAsync();
            return book;
        }

        /// <summary>
        /// Delete Book.
        /// </summary>
        /// <param name="id">Book Id.</param>
        /// <returns></returns>
        public async Task<bool> Delete(int id)
        {
            var book = await _appDbContext.Books.SingleAsync(b => b.Id == id);
            if (book == null)
                return false;

            _appDbContext.Books.Remove(book);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Update Book.
        /// </summary>
        /// <param name="id">Book Id.</param>
        /// <param name="bookViewModel"></param>
        /// <returns></returns>
        public async Task<Book> Update(int id, BookViewModel bookViewModel)
        {
            var bookFromDb = await GetBook(id);

            if (bookFromDb == null)
                return null;

            // update book
            bookFromDb.BookName = bookViewModel.BookName;
            bookFromDb.Price = bookViewModel.Price;
            bookFromDb.AuthorId = bookViewModel.AuthorId;

            // has Categories
            if (bookViewModel.BookCategories != null)
            {
                // update bookcategory
                var categories = await _appDbContext.Categories
                    .Where(c => bookViewModel.BookCategories.Contains(c.CategoryName)).ToListAsync();

                // remove old bookcategory
                var bookCategories = await _appDbContext.BookCategories
                    .Where(b => b.BookId == bookViewModel.Id).ToListAsync();
                _appDbContext.BookCategories.RemoveRange(bookCategories);

                // and then add new bookcategory
                foreach (var category in categories)
                {
                    bookFromDb.BookCategories.Add(new BookCategory
                    {
                        Category = category,
                        BookId = bookFromDb.Id
                    });
                }
            }
            _appDbContext.Books.Update(bookFromDb);

            await _appDbContext.SaveChangesAsync();
            return bookFromDb;
        }

        /// <summary>
        /// Get Book Category.
        /// </summary>
        /// <param name="bookId">Book Id.</param>
        /// <returns></returns>
        public async Task<BookCategory> GetBookCategory(int bookId)
        {
            return await _appDbContext.BookCategories
                .Include(a => a.Category).FirstOrDefaultAsync(i => i.BookId == bookId);
        }

        /// <summary>
        /// Get Category.∂
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<Category> GetCategory(int categoryId)
        {
            return await _appDbContext.Categories.FirstOrDefaultAsync(i => i.CategoryId == categoryId);
        }
    }
}

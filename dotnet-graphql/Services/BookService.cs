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
        private readonly AppDbContext _appDBContext;
        public BookService(AppDbContext context)
        {
            _appDBContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Book>> GetBooks()
        {
            return await _appDBContext.Books.Include(a => a.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(c => c.Category).AsNoTracking()
                .ToListAsync();
        }

        public async Task<Book> GetBook(int id)
        {
            return await _appDBContext.Books.Include(a => a.Author)
                .Include(c => c.BookCategories)
                    .ThenInclude(c => c.Category).AsNoTracking()
                .SingleAsync(b => b.Id == id);
        }

        public async Task<Book> Create(BookViewModel bookViewModel)
        {
            Book book = new Book
            {
                BookName = bookViewModel.BookName,
                Price = bookViewModel.Price,
                AuthorId = bookViewModel.AuthorId
            };

            // has Categories
            if (bookViewModel.BookCategories != null)
            {
                // update bookcategory
                List<Category> categories = await _appDBContext.Categories
                    .Where(c => bookViewModel.BookCategories.Contains(c.CategoryName)).ToListAsync();

                book.BookCategories = new List<BookCategory>();

                // and then add new bookcategory
                foreach (Category category in categories)
                {
                    book.BookCategories.Add(new BookCategory
                    {
                        CategoryId = category.CategoryId,
                        BookId = book.Id
                    });
                }
            }

            await _appDBContext.Books.AddAsync(book);
            await _appDBContext.SaveChangesAsync();
            return book;
        }

        public async Task<bool> Delete(int id)
        {
            Book book = await _appDBContext.Books.SingleAsync(b => b.Id == id);
            if (book == null)
            {
                return false;
            }

            _appDBContext.Books.Remove(book);
            await _appDBContext.SaveChangesAsync();
            return true;
        }

        public async Task<Book> Update(int id, BookViewModel bookViewModel)
        {
            Book bookFromDb = await GetBook(id);

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
                List<Category> categories = await _appDBContext.Categories
                    .Where(c => bookViewModel.BookCategories.Contains(c.CategoryName)).ToListAsync();

                // remove old bookcategory
                List<BookCategory> bookCategories = await _appDBContext.BookCategories
                    .Where(b => b.BookId == bookViewModel.Id).ToListAsync();
                _appDBContext.BookCategories.RemoveRange(bookCategories);

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
            _appDBContext.Books.Update(bookFromDb);

            await _appDBContext.SaveChangesAsync();
            return bookFromDb;
        }

        public async Task<BookCategory> GetBookCategory(int bookId)
        {
            return await _appDBContext.BookCategories
                .Include(a => a.Category).FirstOrDefaultAsync(i => i.BookId == bookId);
        }

        public async Task<Category> GetCategory(int categoryId)
        {
            return await _appDBContext.Categories.FirstOrDefaultAsync(i => i.CategoryId == categoryId);
        }
    }
}

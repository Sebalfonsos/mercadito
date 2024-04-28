using mercadito.LocalDbModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mercadito
{
    public class LocalDbService
    {
        private const string DB_NAME = "mercadito_db.db3";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _connection.CreateTableAsync<Cart>();
        }

        public async Task<List<Cart>> getCart()
        {
            return await _connection.Table<Cart>().ToListAsync();
        }

        public async Task create(Cart item)
        {
            await _connection.InsertAsync(item);
        }

        public async Task delete(Cart item)
        {
            await _connection.DeleteAsync(item);
        }

        public async Task clearCart()
        {
            await _connection.DeleteAllAsync<Cart>();
        }
    }
}

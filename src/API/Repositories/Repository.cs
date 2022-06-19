using API.Data;
using API.Interfaces;
using API.Models;
using API.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace API.Repositories
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        #region Fields
        private MongoClient _client;
        private IMongoDatabase _database;
        //private FunnyAppDatabaseSettings _settings;

        public Repository()
        {
            //_settings = new FunnyAppDatabaseSettings();
            //_client = new MongoClient(_settings.ConnectionString);
            //_database = _client.GetDatabase(_settings.DatabaseName);
            _client = new MongoClient("mongodb://localhost:27017");
            _database = _client.GetDatabase("FunnyApp");
        }
        #endregion

        #region Public Methods

        public async Task<T> GetById(string id, string collection)
        {
            var _collection = _database.GetCollection<T>(collection);
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate, string collection)
        {
            var _collection = _database.GetCollection<T>(collection);
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task Add(T entity, string collection)
        {
            entity.Id = ObjectId.GenerateNewId().ToString();
            entity.CreationDate = DateTime.Now;
            var _collection = _database.GetCollection<T>(collection);
            await _collection.InsertOneAsync(entity);
        }

        public async Task Update(T entity, string id, string collection)
        {
            var _collection = _database.GetCollection<T>(collection);
            await _collection.ReplaceOneAsync(x => x.Id == id, entity);
        }

        public async Task Remove(string id, string collection)
        {
            var _collection = _database.GetCollection<T>(collection);
            await _collection.DeleteOneAsync(x => x.Id == id.ToString());
        }

        public async Task<List<T>> GetAll(string collection)
        {
            var _collection = _database.GetCollection<T>(collection);
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<List<T>> GetWhere(Expression<Func<T, bool>> predicate, string collection)
        {
            var _collection = _database.GetCollection<T>(collection);
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<bool> Exists(string id, string collection)
        {
            var _collection = _database.GetCollection<T>(collection);
            var docs = await _collection.Find(x => x.Id == id).ToListAsync();
            return docs.Any();
        }

        #endregion

    }

}

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB.Simple.Store.Interfaces
{
    
    public interface ISimpleItemDbContext<T> : IDisposable where T : class
    {
        Task<T> GetItemAsync(string id);

        Task<ItemResponse<T>> UpsertItemAsync(T item);

        Task<Document> ReplaceItemAsync(string id, T item);

        Task DeleteItemAsync(string id);
       
        Container Container { get; }
    }
}

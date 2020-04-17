using System.Collections.Generic;

namespace CosmosDB.Simple.Store.Configuration
{
    /// <summary>
    ///     AppSettings CosmosDb Configuration Section.
    /// </summary>
    public class CosmosDbConfiguration<T>
        where T: class
    {
        /// <summary>
        ///     URL EndPoint for CosmosDb Instance.
        /// </summary>
        public string EndPointUrl { get; set; }

        /// <summary>
        ///     Primary Key for CosmosDb Instance.
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        ///     Optional Database Name (overrides default).
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        ///     Optional Database Collection Details (overrides defaults).
        /// </summary>
        //public List<Collection> Collections { get; set; } = new List<Collection>();
        public Collection Collection { get; set; }
    }
}
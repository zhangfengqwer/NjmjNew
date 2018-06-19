using MongoDB.Driver;

namespace ETModel
{
	[ObjectSystem]
	public class DbComponentSystem : AwakeSystem<DBComponent>
	{
		public override void Awake(DBComponent self)
		{
			self.Awake();
		}
	}

	/// <summary>
	/// 连接mongodb
	/// </summary>
	public class DBComponent : Component
	{
		public MongoClient mongoClient;
		public IMongoDatabase database;

		public void Awake()
		{
			DBConfig config = Game.Scene.GetComponent<StartConfigComponent>().StartConfig.GetComponent<DBConfig>();
			string connectionString = config.ConnectionString;
			mongoClient = new MongoClient(connectionString);
			this.database = this.mongoClient.GetDatabase(config.DBName);
		}

		public IMongoCollection<ComponentWithId> GetCollection(string name)
		{
			return this.database.GetCollection<ComponentWithId>(name);
		}

        public IMongoCollection<PlayerBaseInfo> GetPlayerBaseInfoCollection(string name)
        {
            return this.database.GetCollection<PlayerBaseInfo>(name);
        }

        public IMongoCollection<T> GetDBDataCollection<T>(string name)
        {
            return this.database.GetCollection<T>(name);
        }

        public IMongoCollection<EmailInfo> GetEmailInfoCollection(string name)
        {
            return this.database.GetCollection<EmailInfo>(name);
        }
    }
}
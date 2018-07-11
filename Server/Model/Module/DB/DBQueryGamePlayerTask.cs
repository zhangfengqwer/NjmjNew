using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETModel
{
    [ObjectSystem]
    public class DBQueryGamePlayerTaskAwakeSystem : AwakeSystem<DBQueryGamePlayerTask, string, string,
        TaskCompletionSource<List<PlayerBaseInfo>>>
    {
        public override void Awake(DBQueryGamePlayerTask self, string collectionName, string json,
            TaskCompletionSource<List<PlayerBaseInfo>> tcs)
        {
            self.CollectionName = collectionName;
            self.Json = json;
            self.Tcs = tcs;
        }
    }

    public sealed class DBQueryGamePlayerTask : DBTask
    {
        public string CollectionName { get; set; }

        public string Json { get; set; }

        public TaskCompletionSource<List<PlayerBaseInfo>> Tcs { get; set; }

        public override async Task Run()
        {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            try
            {
                // 执行查询数据库任务
                FilterDefinition<PlayerBaseInfo> filterDefinition = new JsonFilterDefinition<PlayerBaseInfo>(this.Json);
                //                List<PlayerBaseInfo> components = await dbComponent.GetPlayerBaseInfoCollection(this.CollectionName).Find(filterDefinition).SortByDescending(a => a.WinGameCount).Limit(30).ToListAsync();
                List<PlayerBaseInfo> components = new List<PlayerBaseInfo>();
                this.Tcs.SetResult(components);
            }
            catch (Exception e)
            {
                this.Tcs.SetException(new Exception($"查询数据库异常! {CollectionName} {this.Json}", e));
            }

        }
    }
}
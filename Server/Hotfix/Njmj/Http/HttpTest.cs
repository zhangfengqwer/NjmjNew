using System;
using System.Net;
using ETModel;
using System.Threading.Tasks;

namespace ETHotfix
{
    [HttpHandler(AppType.Gate, "/")]
    public class HttpTest : AHttpHandler
    {
        [Get] // url-> /Login?name=11&age=1111
        public string Login(string name, int age, HttpListenerRequest req, HttpListenerResponse resp)
        {
            Log.Info(name);
            Log.Info($"{age}");
            return "ok";
        }

        [Get("t")] // url-> /t
        public int Test()
        {
            Log.Info("");
            return 1;
        }

        [Get] // url-> /t
        public string GetClientError(string data)
        {
            Log.Error("客户端日志:" + data);
            return "ok";
        }

        [Post] // url-> /Test1
        public int Test1(HttpListenerRequest req)
        {
            return 1;
        }

        [Get] // url-> /Test2
        public int Test2(HttpListenerResponse resp)
        {
            return 1;
        }

        [Get] // url-> /GetRechargeRecord
        public async Task<HttpResult> GetRechargeRecord(long id)
        {
            // var db = Game.Scene.GetComponent<DBProxyComponent>();

            // var info = await db.Query<RechargeRecord>(id);

            await Task.Delay(1000); // 用于测试

            object info = null;
            if (info != null)
            {
                return Ok(data: info);
            }
            else
            {
                return Error("ID不存在！");
            }
        }

        [Get] // url-> /Login?name=11&age=1111
        public async Task<HttpResult> BuyYuanBao(int orderId, long userId, int goodsId, int goodsNum, float price, string account, string password)
        {
            Log.Info($"web请求发元宝,orderId:{orderId},userId:{userId},goodsId:{goodsId},price:{price}");
            try
            {
                if (!"admin".Equals(account) || !"jinyou123".Equals(password))
                {
                    return Error("账号错误");
                }

                ShopConfig config = ShopData.getInstance().GetDataByShopId(goodsId);

                if (config == null)
                {
                    return Error("goodsId错误");
                }

                if (price != config.Price)
                {
                    return Error("支付的价格不正确");
                }

                await DBCommonUtil.UserRecharge(orderId, userId, goodsId, goodsNum, price);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return Error();
            }
        }
    }
}
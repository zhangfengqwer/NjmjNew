using ETModel;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    public class DataStatistics
    {
        public static async Task Start()
        {
            try
            {
                string time = CommonUtil.timeAddDays(CommonUtil.getCurDataNormalFormat(), -1);
                time = Convert.ToDateTime(time).ToString("yyyy-MM-dd");

                string logData = "";
                logData += await NewUser(time);
                logData += await DailyLogin(time);
                logData += await LoadOldUserCount(time);
                logData += await CiLiu(time);
                logData += await RechargeNum(time);
                logData += await RechargeUserNum(time);
                logData += await GameCount(time);
                logData += await GameUserCount(time);

                writeLogToLocalNow(logData);
            }
            catch (Exception ex)
            {

            }
        }

        // time : yyyy-MM-dd
        public static async Task<string> Start(string time)
        {
            try
            {
                string logData = "";
                logData += await NewUser(time);
                logData += await DailyLogin(time);
                logData += await LoadOldUserCount(time);
                logData += await CiLiu(time);
                logData += await RechargeNum(time);
                logData += await RechargeUserNum(time);
                logData += await GameCount(time);
                logData += await GameUserCount(time);
                writeLogToLocalNow(logData);
                return logData;
            }
            catch (Exception ex)
            {
                Log.Error("生成报表异常" + ex);
                return "";
            }
        }

        // 新增用户
        static async Task<string> NewUser(string time)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<AccountInfo> listData = await proxyComponent.QueryJsonDBInfos<AccountInfo>(time);
            return "新增用户：" + listData.Count + "\r\n";
        }

        // 日活
        static async Task<string> DailyLogin(string time)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<Log_Login> listData = await proxyComponent.QueryJsonDBInfos<Log_Login>(time);
            List<long> listPlayer = new List<long>();
            for (int i = 0; i < listData.Count; i++)
            {
                bool isFind = false;
                for(int j = 0; j < listPlayer.Count; j++)
                {
                    if (listPlayer[j] == listData[i].Uid)
                    {
                        isFind = true;
                        break;
                    }
                }

                if (!isFind)
                {
                    listPlayer.Add(listData[i].Uid);
                }
            }

            return "日活：" + listPlayer.Count + "\r\n";
        }

        // 导入老用户
        static async Task<string> LoadOldUserCount(string time)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<Log_OldUserBind> listData = await proxyComponent.QueryJsonDBInfos<Log_OldUserBind>(time);
            return "导入老用户：" + listData.Count + "\r\n";
        }

        // 次留
        static async Task<string> CiLiu(string time)
        {
            string zuotianTime = Convert.ToDateTime(time).AddDays(-1).ToString("yyyy-MM-dd");

            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<AccountInfo> listData = await proxyComponent.QueryJsonDBInfos<AccountInfo>(zuotianTime);

            float ciliu = 0;
            int loginCount = 0;

            if (listData.Count > 0)
            {
                for (int i = 0; i < listData.Count; i++)
                {
                    long uid = listData[i].Id;
                    List<Log_Login> listData2 = await proxyComponent.QueryJsonDBInfos<Log_Login>(time, uid);
                    if (listData2.Count > 0)
                    {
                        ++loginCount;
                    }
                }

                ciliu = ((float)loginCount / (float)listData.Count) * 100;
            }

            //Log.Debug("昨日新增：" + listData.Count);
            //Log.Debug("今日登陆：" + loginCount);

            return "次留：" + ciliu + "%\r\n";
        }

        // 充值金额
        static async Task<string> RechargeNum(string time)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<Log_Recharge> listData = await proxyComponent.QueryJsonDBInfos<Log_Recharge>(time);
            int num = 0;
            for (int i = 0; i < listData.Count; i++)
            {
                num += listData[i].Price;
            }

            return "充值金额：" + num + "\r\n";
        }

        // 充值人数
        static async Task<string> RechargeUserNum(string time)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<Log_Recharge> listData = await proxyComponent.QueryJsonDBInfos<Log_Recharge>(time);
            List<long> listPlayer = new List<long>();
            for (int i = 0; i < listData.Count; i++)
            {
                bool isFind = false;
                for (int j = 0; j < listPlayer.Count; j++)
                {
                    if (listPlayer[j] == listData[i].Uid)
                    {
                        isFind = true;
                        break;
                    }
                }

                if (!isFind)
                {
                    listPlayer.Add(listData[i].Uid);
                }
            }

            return "充值人数：" + listPlayer.Count + "\r\n";
        }

        // 游戏局数
        static async Task<string> GameCount(string time)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<Log_Game> listData = await proxyComponent.QueryJsonDBInfos<Log_Game>(time);
            return "游戏局数：" + listData.Count + "\r\n";
        }

        // 游戏人数
        static async Task<string> GameUserCount(string time)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<Log_Game> listData = await proxyComponent.QueryJsonDBInfos<Log_Game>(time);
            List<long> listPlayer = new List<long>();
            for (int i = 0; i < listData.Count; i++)
            {
                {
                    bool isFind = false;
                    for (int j = 0; j < listPlayer.Count; j++)
                    {
                        if (listPlayer[j] == listData[i].Player1_uid)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        listPlayer.Add(listData[i].Player1_uid);
                    }
                }

                {
                    bool isFind = false;
                    for (int j = 0; j < listPlayer.Count; j++)
                    {
                        if (listPlayer[j] == listData[i].Player2_uid)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        listPlayer.Add(listData[i].Player2_uid);
                    }
                }

                {
                    bool isFind = false;
                    for (int j = 0; j < listPlayer.Count; j++)
                    {
                        if (listPlayer[j] == listData[i].Player3_uid)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        listPlayer.Add(listData[i].Player3_uid);
                    }
                }

                {
                    bool isFind = false;
                    for (int j = 0; j < listPlayer.Count; j++)
                    {
                        if (listPlayer[j] == listData[i].Player4_uid)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        listPlayer.Add(listData[i].Player4_uid);
                    }
                }
            }

            return "游戏人数：" + listPlayer.Count + "\r\n";
        }

        // 记录文本日志到本地
        static void writeLogToLocalNow(string data)
        {
            data = CommonUtil.getCurTimeNormalFormat() + ":\r\n" + data;
            StreamWriter sw = null;
            try
            {
                string folderPath = AppDomain.CurrentDomain.BaseDirectory + "../BaoBiao/";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = folderPath + "/" + CommonUtil.getCurDataNormalFormat() + ".txt";
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();
                }

                sw = new StreamWriter(filePath, true);

                sw.WriteLine(data);

                //清空缓冲区
                sw.Flush();

                //关闭流
                sw.Close();
            }
            catch (Exception ex)
            {
                Log.Error("writeLogToLocalNow异常:" + ex);
            }
            finally
            {
                sw.Close();
            }
        }
    }
}

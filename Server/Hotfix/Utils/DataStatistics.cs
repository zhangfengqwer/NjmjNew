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
        public static List<string> channelList = new List<string>() { "","javgame", "oppo", "vivo","huawei","xiaomi", "baidu", "baidudk", "baidu91", "baidutb","qihoo360" };

        public static async Task Start(string channelName)
        {
            try
            {
                string time = CommonUtil.timeAddDays(CommonUtil.getCurDataNormalFormat(), -1);
                time = Convert.ToDateTime(time).ToString("yyyy-MM-dd");

                string logData = "";
                if (string.IsNullOrEmpty(channelName))
                {
                    logData = "渠道：所有\r\n";
                }
                else
                {
                    logData = "渠道：" + channelName + "\r\n";
                }
               
                logData += await NewUser(time, channelName);
                logData += await DailyLogin(time, channelName);
                logData += await LoadOldUserCount(time, channelName);
                logData += await CiLiu(time, channelName);
                logData += await RechargeNum(time, channelName);
                logData += await RechargeUserNum(time, channelName);
                logData += await GameCount(time, channelName);
                logData += await GameUserCount(time, channelName);

                writeLogToLocalNow(logData);
            }
            catch (Exception ex)
            {

            }
        }

        // time : yyyy-MM-dd
        public static async Task<string> Start(string time,string channelName)
        {
            try
            {
                if (string.IsNullOrEmpty(channelName))
                {
                    string backData = "";
                    for (int i = 0; i < channelList.Count; i++)
                    {
                        string logData = "";
                        if (string.IsNullOrEmpty(channelList[i]))
                        {
                            logData = "渠道：所有\r\n";
                            logData += await NewUser(time, channelList[i]);
                            logData += await DailyLogin(time, channelList[i]);
                            logData += await LoadOldUserCount(time, channelList[i]);
                            logData += await CiLiu(time, channelList[i]);
                            logData += await RechargeNum(time, channelList[i]);
                            logData += await RechargeUserNum(time, channelList[i]);
                            logData += await GameCount(time, channelList[i]);
                            logData += await GameUserCount(time, channelList[i]);
                            
                            writeLogToLocalNow(logData);

                            backData = logData;
                        }
                        else
                        {
                            logData = "渠道：" + channelList[i] + "\r\n";
                            logData += await NewUser(time, channelList[i]);
                            logData += await DailyLogin(time, channelList[i]);
                            logData += await LoadOldUserCount(time, channelList[i]);
                            logData += await CiLiu(time, channelList[i]);
                            logData += await RechargeNum(time, channelList[i]);
                            logData += await RechargeUserNum(time, channelList[i]);
                            logData += await GameCount(time, channelList[i]);
                            logData += await GameUserCount(time, channelList[i]);
                            writeLogToLocalNow(logData);
                        }
                    }

                    return backData;
                }
                else
                {
                    string logData = "渠道：" + channelName + "\r\n";
                    logData += await NewUser(time, channelName);
                    logData += await DailyLogin(time, channelName);
                    logData += await LoadOldUserCount(time, channelName);
                    logData += await CiLiu(time, channelName);
                    logData += await RechargeNum(time, channelName);
                    logData += await RechargeUserNum(time, channelName);
                    logData += await GameCount(time, channelName);
                    logData += await GameUserCount(time, channelName);
                    writeLogToLocalNow(logData);
                    return logData;
                }
            }
            catch (Exception ex)
            {
                Log.Error("生成报表异常" + ex);
                return "";
            }
        }

        // 新增用户
        static async Task<string> NewUser(string time, string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<AccountInfo> listData = await proxyComponent.QueryJsonDBInfos<AccountInfo>(time);
                return "新增用户：" + listData.Count + "\r\n";
            }
            else
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<AccountInfo> listData = await proxyComponent.QueryJsonDBInfos<AccountInfo>(time);

                int count = 0;

                for (int i = 0; i < listData.Count; i++)
                {
                    if (listData[i].ChannelName.CompareTo(channelName) == 0)
                    {
                        ++count;
                    }
                }

                return "新增用户：" + count + "\r\n";
            }
        }

        // 日活
        static async Task<string> DailyLogin(string time, string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<Log_Login> listData = await proxyComponent.QueryJsonDBInfos<Log_Login>(time);
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

                return "日活：" + listPlayer.Count + "\r\n";
            }
            else
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<Log_Login> listData = await proxyComponent.QueryJsonDBInfos<Log_Login>(time);
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

                int count = 0;

                for (int i = 0; i < listPlayer.Count; i++)
                {
                    AccountInfo accountInfo = await DBCommonUtil.getAccountInfo(listPlayer[i]);
                    if (accountInfo.ChannelName.CompareTo(channelName) == 0)
                    {
                        ++count;
                    }
                }

                return "日活：" + count + "\r\n";
            }
        }

        // 导入老用户
        static async Task<string> LoadOldUserCount(string time, string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<Log_OldUserBind> listData = await proxyComponent.QueryJsonDBInfos<Log_OldUserBind>(time);
                return "导入老用户：" + listData.Count + "\r\n";
            }
            else
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<Log_OldUserBind> listData = await proxyComponent.QueryJsonDBInfos<Log_OldUserBind>(time);

                int count = 0;

                for (int i = 0; i < listData.Count; i++)
                {
                    AccountInfo accountInfo = await DBCommonUtil.getAccountInfo(listData[i].Uid);
                    if (accountInfo.ChannelName.CompareTo(channelName) == 0)
                    {
                        ++count;
                    }
                }

                return "导入老用户：" + count + "\r\n";
            }
        }

        // 次留
        static async Task<string> CiLiu(string time, string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
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
            else
            {
                string zuotianTime = Convert.ToDateTime(time).AddDays(-1).ToString("yyyy-MM-dd");

                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<AccountInfo> listData = await proxyComponent.QueryJsonDBInfos<AccountInfo>(zuotianTime);
                int allCount = 0;
                {
                    for (int i = 0; i < listData.Count; i++)
                    {
                        if (listData[i].ChannelName.CompareTo(channelName) == 0)
                        {
                            ++allCount;
                        }
                    }
                }

                float ciliu = 0;
                int loginCount = 0;

                if (allCount > 0)
                {
                    for (int i = 0; i < listData.Count; i++)
                    {
                        if (listData[i].ChannelName.CompareTo(channelName) == 0)
                        {
                            long uid = listData[i].Id;
                            List<Log_Login> listData2 = await proxyComponent.QueryJsonDBInfos<Log_Login>(time, uid);
                            if (listData2.Count > 0)
                            {
                                ++loginCount;
                            }
                        }
                    }

                    ciliu = ((float)loginCount / (float)allCount) * 100;
                }

                //Log.Debug("昨日新增：" + listData.Count);
                //Log.Debug("今日登陆：" + loginCount);

                return "次留：" + ciliu + "%\r\n";
            }
        }

        // 充值金额
        static async Task<string> RechargeNum(string time, string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
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
            else
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<Log_Recharge> listData = await proxyComponent.QueryJsonDBInfos<Log_Recharge>(time);

                int num = 0;
                for (int i = 0; i < listData.Count; i++)
                {
                    AccountInfo accountInfo = await DBCommonUtil.getAccountInfo(listData[i].Uid);
                    if (accountInfo.ChannelName.CompareTo(channelName) == 0)
                    {
                        num += listData[i].Price;
                    }
                    
                }

                return "充值金额：" + num + "\r\n";
            }
        }

        // 充值人数
        static async Task<string> RechargeUserNum(string time, string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
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
            else
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

                int count = 0;

                for (int i = 0; i < listPlayer.Count; i++)
                {
                    AccountInfo accountInfo = await DBCommonUtil.getAccountInfo(listPlayer[i]);
                    if (accountInfo.ChannelName.CompareTo(channelName) == 0)
                    {
                        ++count;
                    }
                }

                return "充值人数：" + count + "\r\n";
            }
        }

        // 游戏局数
        static async Task<string> GameCount(string time, string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<Log_Game> listData = await proxyComponent.QueryJsonDBInfos<Log_Game>(time);
                return "游戏局数：" + listData.Count + "\r\n";
            }
            else
            {
                return "游戏局数：无法查看" + "\r\n";
            }
        }

        // 游戏人数
        static async Task<string> GameUserCount(string time, string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
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
            else
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

                int count = 0;

                for (int i = 0; i < listPlayer.Count; i++)
                {
                    AccountInfo accountInfo = await DBCommonUtil.getAccountInfo(listPlayer[i]);
                    if (accountInfo.ChannelName.CompareTo(channelName) == 0)
                    {
                        ++count;
                    }
                }

                return "游戏人数：" + count + "\r\n";
            }
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
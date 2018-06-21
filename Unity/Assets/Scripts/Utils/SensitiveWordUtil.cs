using ETModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unity_Utils;
using UnityEngine;

namespace Unity_Utils
{
    public class SensitiveWordUtil
    {
        public static string[] WordsDatas;

        public static async Task Req(string url)
        {
            using (UnityWebRequestAsync webRequestAsync = ETModel.ComponentFactory.Create<UnityWebRequestAsync>())
            {
                await webRequestAsync.DownloadAsync(url);
                InitWords(webRequestAsync.Request.downloadHandler.text);
            }
        }

        public static void Init()
        {
            string data = CommonUtil.getTextFileByBundle("config", "stopwords");
            WordsDatas = data.Split(',');
        }

        public static void InitWords(string data)
        {
            WordsDatas = data.Split(',');
        }

        public static bool IsSensitiveWord(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            for (int i = 0; i < WordsDatas.Length; i++)
            {
                string words = WordsDatas[i];
                if ((str.Length >= words.Length) && (!string.IsNullOrEmpty(words)))
                {
                    if (CommonUtil.isStrContain(str, words))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //public static bool IsSensitiveWord(string str)
        //{
        //    if (string.IsNullOrEmpty(str))
        //    {
        //        return false;
        //    }

        //    for (int i = 0; i < WordsDatas.Length; i++)
        //    {
        //        string words = WordsDatas[i];
        //        if ((str.Length >= words.Length) && (!string.IsNullOrEmpty(words)))
        //        {
        //            if (Regex.IsMatch(str, words))
        //            {
        //                Log.Debug("i：" + i);
        //                Log.Debug("敏感词：" + str);
        //                Log.Debug("敏感词：" + words);
        //                Log.Debug("前一个敏感词：" + WordsDatas[i - 1]);
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        public static string deleteSensitiveWord(string str)
        {
            string final_str = str;
            if (string.IsNullOrEmpty(final_str))
            {
                return final_str;
            }

            foreach (var words in WordsDatas)
            {
                if (CommonUtil.isStrContain(final_str, words))
                {
                    string temp = "";
                    for (int i = 0; i < words.Length; i++)
                    {
                        temp += "*";
                    }

                    final_str = final_str.Replace(words, temp);
                }
            }

            return final_str;
        }
    }
}
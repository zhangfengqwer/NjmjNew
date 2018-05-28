using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

namespace ETHotfix
{
    public class SensitiveWordUtil
    {
        public static string[] WordsDatas;

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

            foreach (var words in WordsDatas)
            {
                if (CommonUtil.isStrContain(str, words))
                {
                    Log.Debug("敏感词：" + words);
                    return true;
                }
            }
            return false;
        }

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
                    Log.Debug("敏感词：" + words);

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
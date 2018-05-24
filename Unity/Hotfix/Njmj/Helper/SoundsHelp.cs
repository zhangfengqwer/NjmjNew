using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    public class SoundsHelp
    {
        private static SoundsHelp instance;

        public static SoundsHelp Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                instance = new SoundsHelp();

                instance.SoundMute(PlayerPrefs.GetInt("isOpenSound", 1) == 1);

                return instance;
            }
        }

        // 男声1出牌
        public void playSound_Card_Nan1(int mahjongWeight)
        {
            playSound("card_nan1_" + mahjongWeight);
        }

        // 男声2出牌
        public void playSound_Card_Nan2(int mahjongWeight)
        {
            playSound("card_nan2_" + mahjongWeight);
        }

        // 女声1出牌
        public void playSound_Card_Nv1(int mahjongWeight)
        {
            playSound("card_nv1_" + mahjongWeight);
        }

        // 女声2出牌
        public void playSound_Card_Nv2(int mahjongWeight)
        {
            playSound("card_nv2_" + mahjongWeight);
        }

        // 出牌声
        public void playSound_ChuPai()
        {
            playSound("effect_dapai");
        }

        // 失败
        public void playSound_Fail()
        {
            playSound("effect_fail");
        }

        // 胜利
        public void playSound_Win()
        {
            playSound("effect_win");
        }

        // 进入
        public void playSound_JinRu()
        {
            playSound("effect_jinru");
        }

        // 离开
        public void playSound_LiKai()
        {
            playSound("effect_likai");
        }

        // 准备
        public void playSound_ZhunBei()
        {
            playSound("effect_zhunbei");
        }

        // 摸牌
        public void playSound_MoPai()
        {
            playSound("effect_mopai");
        }

        // 碰
        public void playSound_Peng()
        {
            playSound("effect_peng");
        }

        // 骰子
        public void playSound_ShaiZi()
        {
            playSound("effect_shaizi");
        }

        // 男1补花
        public void playSound_Nan1_BuHua()
        {
            playSound("effect_nan1_buhua");
        }

        // 男2补花
        public void playSound_Nan2_BuHua()
        {
            playSound("effect_nan2_buhua");
        }

        // 女1补花
        public void playSound_Nv1_BuHua()
        {
            playSound("effect_nv1_buhua");
        }

        // 女2补花
        public void playSound_Nv2_BuHua()
        {
            playSound("effect_nv2_buhua");
        }

        // 男杠
        public void playSound_Nan_Gang()
        {
            playSound("talk_nan_gang_" + UnityEngine.Random.Range(1, 6 + 1));
        }

        // 男胡
        public void playSound_Nan_Hu()
        {
            playSound("talk_nan_hu_" + UnityEngine.Random.Range(1, 4 + 1));
        }

        // 男碰
        public void playSound_Nan_Peng()
        {
            playSound("talk_nan_peng_" + UnityEngine.Random.Range(1, 6 + 1));
        }

        // 男自摸
        public void playSound_Nan_ZiMo()
        {
            playSound("talk_nan_zimo_" + UnityEngine.Random.Range(1, 4 + 1));
        }

        // 女杠
        public void playSound_Nv_Gang()
        {
            playSound("talk_nv_gang_" + UnityEngine.Random.Range(1, 6 + 1));
        }

        // 女胡
        public void playSound_Nv_Hu()
        {
            playSound("talk_nv_hu_" + UnityEngine.Random.Range(1, 4 + 1));
        }

        // 女碰
        public void playSound_Nv_Peng()
        {
            playSound("talk_nv_peng_" + UnityEngine.Random.Range(1, 6 + 1));
        }

        // 女自摸
        public void playSound_Nv_ZiMo()
        {
            playSound("talk_nv_zimo_" + UnityEngine.Random.Range(1, 4 + 1));
        }

        public void playSound(string name)
        {
            ETModel.Game.Scene.GetComponent<SoundComponent>().PlayClip(name);
        }

        public void SoundMute(bool isMute)
        {
            if (isMute)
            {
                ETModel.Game.Scene.GetComponent<SoundComponent>().SoundVolume = 0;
            }
            else
            {
                ETModel.Game.Scene.GetComponent<SoundComponent>().SoundVolume = 1;
            }
        }
    }
}
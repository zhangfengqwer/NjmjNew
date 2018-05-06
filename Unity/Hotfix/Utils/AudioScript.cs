using UnityEngine;
using System.Collections.Generic;

namespace ETHotfix
{
    public class AudioScript : MonoBehaviour
    {
        public static GameObject s_audioObj = null;
        public static AudioScript s_audioScript;

        //#背景音乐只会有一个;
        public AudioSource m_musicPlayer;
        //#音效会同时播放多个，所以用List;
        public List<AudioSource> m_soundPlayer;

        public float m_musicVolume = 1.0f;
        public float m_soundVolume = 1.0f;

        static public AudioScript getAudioScript()
        {
            if (!s_audioObj)
            {
                s_audioObj = new GameObject();
                s_audioObj.transform.name = "Audio";
                MonoBehaviour.DontDestroyOnLoad(s_audioObj);
                s_audioScript = s_audioObj.AddComponent<AudioScript>();
                s_audioScript.init();
            }

            return s_audioScript;
        }

        public void init()
        {
            initMusicPlayer();
            initSoundPlayer();
        }

        public void initMusicPlayer()
        {
            m_musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);

            GameObject go = new GameObject("musicPlayer");
            go.transform.SetParent(transform, false);
            AudioSource player = go.AddComponent<AudioSource>();
            m_musicPlayer = player;

            player.loop = true;
            player.mute = false;
            player.volume = m_musicVolume;
            player.pitch = 1.0f;
            player.playOnAwake = false;
        }

        public void initSoundPlayer()
        {
            m_soundVolume = PlayerPrefs.GetFloat("SoundVolume", 1.0f);

            m_soundPlayer = new List<AudioSource>();

            GameObject go = new GameObject("soundPlayer");
            go.transform.SetParent(transform, false);
            AudioSource player = go.AddComponent<AudioSource>();
            m_soundPlayer.Add(player);

            player.loop = false;
            player.mute = false;
            player.volume = m_soundVolume;
            player.pitch = 1.0f;
            player.playOnAwake = false;
        }

        public void playMusic(string audioName)
        {
            m_musicPlayer.clip = CommonUtil.getAudioClipByAssetBundle("sounds.unity3d", audioName);
            m_musicPlayer.clip = (AudioClip)Resources.Load(audioName, typeof(AudioClip));
            m_musicPlayer.Play();
            m_musicPlayer.volume = m_musicVolume;
        }

        public void playSound(string audioName)
        {
            for (int i = 0; i < m_soundPlayer.Count; i++)
            {
                if (!m_soundPlayer[i].isPlaying)
                {
                    m_soundPlayer[i].clip = CommonUtil.getAudioClipByAssetBundle("sounds.unity3d", audioName);
                    m_soundPlayer[i].clip = (AudioClip)Resources.Load(audioName, typeof(AudioClip));
                    m_soundPlayer[i].Play();

                    return;
                }
            }

            // 如果执行到这里，说明暂时没有空余的音效组件使用，需要再新建一个
            {
                GameObject go = new GameObject("soundPlayer");
                go.transform.SetParent(transform, false);
                AudioSource player = go.AddComponent<AudioSource>();
                m_soundPlayer.Add(player);

                player.loop = false;
                player.mute = false;
                player.volume = m_soundVolume;
                player.pitch = 1.0f;
                player.playOnAwake = false;

                player.clip = CommonUtil.getAudioClipByAssetBundle("sounds.unity3d", audioName);
                player.Play();
            }
        }

        public float getMusicVolume()
        {
            return m_musicVolume;
        }

        public void setMusicVolume(float volume)
        {
            m_musicVolume = volume;
            m_musicPlayer.volume = m_musicVolume;

            PlayerPrefs.SetFloat("MusicVolume", m_musicVolume);
        }

        public float getSoundVolume()
        {
            return m_soundVolume;
        }

        public void setSoundVolume(float volume)
        {
            m_soundVolume = volume;
            for (int i = 0; i < m_soundPlayer.Count; i++)
            {
                m_soundPlayer[i].volume = m_soundVolume;
            }

            PlayerPrefs.SetFloat("SoundVolume", m_soundVolume);
        }

        public void stopMusic()
        {
            if (m_musicPlayer.isPlaying)
            {
                m_musicPlayer.Stop();
            }
        }

        public void stopSound()
        {
            for (int i = 0; i < m_soundPlayer.Count; i++)
            {
                if (m_soundPlayer[i].isPlaying)
                {
                    m_soundPlayer[i].Stop();
                }
            }
        }

        //----------------------------------------------------------------------------播放 start

        //// 主界面背景音乐
        //public void playMusic_MainBg()
        //{
        //    playMusic("bg_main");
        //}


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
            playSound("fail");
        }

        // 胜利
        public void playSound_Win()
        {
            playSound("win");
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

        //----------------------------------------------------------------------------播放 end
    }
}
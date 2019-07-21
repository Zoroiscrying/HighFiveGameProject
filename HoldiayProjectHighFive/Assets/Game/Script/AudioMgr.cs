using Game.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Game.Const;
using Game.MemorySystem;
using UnityEngine;

namespace Game.Script
{
    public class AudioMgr : MonoSingleton<AudioMgr>
    {
        #region Private

        private Dictionary<string, AudioClip> audioclips = new Dictionary<string, AudioClip>();

        protected override void Awake()
        {
            base.Awake();
            //实现从文件夹读取所有文件
            MemoryMgr.LoadAssetFromResourceDir<AudioClip>(typeof(AudioName), "Audio/", (name, audio) =>
            audioclips.Add(name, audio));
        }

        private List<AudioSource> effectSources = new List<AudioSource>();
        private AudioSource bgmAudioSource;

        /// <summary>
        /// 获取一个闲置的AudioEffect
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private AudioSource GetAudioSource()
        {
            foreach (var source in effectSources)
            {
                if (!source.isPlaying)
                    return source;
            }

            var newSource = gameObject.AddComponent<AudioSource>();
            newSource.volume = 1;
            newSource.loop = false;

            effectSources.Add(newSource);

            return newSource;
        }

        #endregion


        /// <summary>
        /// 背景音乐AudioSource组件
        /// </summary>
        public AudioSource BgmAudioSource
        {
            get
            {
                if (bgmAudioSource == null)
                {
                    bgmAudioSource = (GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>());
                }

                return bgmAudioSource;
            }
        }

        /// <summary>
        /// 音乐、音效播放开关
        /// </summary>
        public bool enableBgmAudio = true;

        public bool enableEffectAudio = true;


        /// <summary>
        /// 背景音乐音量
        /// </summary>
        public float BgmVolume
        {
            set { BgmAudioSource.volume = value; }
            get { return BgmAudioSource.volume; }
        }

        /// <summary>
        /// 设置音效的音量
        /// </summary>
        public float EffectVolume
        {
            set
            {
                foreach (var VARIABLE in effectSources)
                {
                    VARIABLE.volume = value;
                }
            }
        }


        /// <summary>
        /// 播放指定名字的背景音乐
        /// </summary>
        /// <param name="name"></param>
        public void PlayBgm(string name)
        {
            if (!enableBgmAudio)
                return;

            var clip = MemoryMgr.GetSourceFromResources<AudioClip>(name);
            if (clip == null)
            {
                Debug.Log("音效获取失败：" + name);
                return;
            }

            BgmAudioSource.clip = clip;
            BgmAudioSource.loop = true;
            BgmAudioSource.volume = 1;
            BgmAudioSource.Play();
        }

        /// <summary>
        /// 播放指定名字音效
        /// </summary>
        /// <param name="name"></param>
        public void PlayEffect(string name)
        {
            if (!enableEffectAudio)
                return;

            var clip = MemoryMgr.GetSourceFromResources<AudioClip>(name);

            if (clip == null)
            {
                Debug.Log("音效获取失败：" + name);
                return;
            }

            var audioSource = GetAudioSource();
            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.Play();
        }


        /// <summary>
        /// 关闭指定名字音效
        /// </summary>
        /// <param name="name"></param>
        public void CloseEffect(string name)
        {
            foreach (var VARIABLE in effectSources)
            {
                if (VARIABLE.clip.name == name)
                    VARIABLE.Stop();
            }
        }

        /// <summary>
        /// 在指定位置播放音效
        /// </summary>
        /// <param name="audioName"></param>
        /// <param name="position"></param>
        public void PlayEffect(string audioName, Vector3 position)
        {
            AudioSource.PlayClipAtPoint(audioclips[audioName], position);
        }
    }
}
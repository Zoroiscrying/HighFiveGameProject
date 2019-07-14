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
        private Dictionary<string, AudioClip> audioclips = new Dictionary<string, AudioClip>();
        private AudioSource audioBg;
        private AudioSource audioEffect;
        private string currentBgName;

        public float BgSound
        {
            get => audioBg.volume;
            set => audioBg.volume = value;
        }

        public float EffectVolume
        {
            get => audioEffect.volume;
            set => audioEffect.volume = value;
        }

        protected override void Awake()
        {
            base.Awake();
            //实现从文件夹读取所有文件
            MemoryMgr.LoadAssetFromResourceDir<AudioClip>(typeof(AudioName), "Audio/",(name,audio)=>audioclips.Add(name,audio));


            //加载组件
            this.audioBg = this.gameObject.AddComponent<AudioSource>();
            this.audioBg.loop = true;
            this.audioBg.playOnAwake = true;
            
            

            this.audioEffect = this.gameObject.AddComponent<AudioSource>();
            this.audioBg.loop = false;
            this.audioBg.playOnAwake = false;



        }

        void Start()
        {
            //PlayAuBg("Bg");
            //this.audioBg.volume = 0f;
        }



        public void PlayAuBg(string name)
        {
            if (!audioclips.ContainsKey(name))
                throw new Exception("没有这个音频文件");
            currentBgName = name;
            PlayBg();
        }

        public void PlayAuEffect(string name, float delayTime = 0)
        {
            if (!audioclips.ContainsKey(name))
                throw new Exception("没有这个音频文件");
            this.audioEffect.clip = audioclips[name];
            this.audioEffect.PlayDelayed(delayTime);
        }

        public void PlayAuEffect(string name, Vector3 pos)
        {
            if (!audioclips.ContainsKey(name))
                throw new Exception("没有这个音频文件");
            AudioSource.PlayClipAtPoint(this.audioclips[name], pos);
        }

        void PlayBg()
        {
            this.audioBg.clip = audioclips[currentBgName];
            this.audioBg.PlayDelayed(0);
            MainLoop.Instance.ExecuteLater(() => { PlayBg(); }, this.audioBg.clip.length);
        }
        
    }
}

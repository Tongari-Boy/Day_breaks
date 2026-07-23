using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    /// <summary>
    /// サウンドのレジストリ
    /// </summary>
    public class AudioSourceRegistry
    {
        public static readonly AudioSourceRegistry INSTANCE = new();

        private readonly Dictionary<string, AudioSourceHolder> audioSources = new();

        private AudioSourceRegistry() { }

        public bool Register(string id, AudioSource audioSource)
        {
            if (audioSource == null)
                return false;

            if (id == null || id == "")
            {
                Debug.Log($"IDがnullもしくは空文字であるため、サウンドを登録できません…");

                return false;
            }

            try
            {
                this.audioSources.Add(id, new(audioSource));

                Debug.Log($"サウンド（ID: {id}）が登録されました！");

                return true;
            }
            catch (ArgumentException)
            {
                Debug.LogWarning($"サウンド（ID: {id}）はすでに登録されています！");
            }

            return false;
        }

        /// <summary>
        /// 登録されたサウンドを削除する
        /// </summary>
        public bool Delete(string id)
        {
            return this.audioSources.Remove(id);
        }

        /// <summary>
        /// <para>登録されたサウンドを取得する</para>
        /// <para>存在しない場合はAudioSourceHolder.EMPTYを返す</para>
        /// </summary>
        public AudioSourceHolder Get(string id)
        {
            if (id != null && id != "" && this.audioSources.ContainsKey(id))
            {
                return this.audioSources[id] ?? AudioSourceHolder.EMPTY;
            }

            return AudioSourceHolder.EMPTY;
        }

        /// <summary>
        /// 登録されたサウンドを再生する
        /// </summary>
        public bool Play(string id)
        {
            if (id != null && id != "" && this.audioSources.ContainsKey(id))
            {
                AudioSource audioSource = this.audioSources[id]?.audioSource;

                if (audioSource != null)
                {
                    if (audioSource.enabled)
                    {
                        // サウンドを再生
                        audioSource.Play();

                        return true;
                    }
                    else
                    {
                        Debug.Log($"登録されたサウンド（ID：{id}）が無効になっているため再生できませんでした…");
                    }
                }

                return false;
            }

            Debug.Log($"レジストリにIDが存在しないため、サウンド（ID: {id}）を再生できませんでした…");

            return false;
        }

        /// <summary>
        /// サウンドホルダー
        /// </summary>
        public class AudioSourceHolder
        {
            public static readonly AudioSourceHolder EMPTY = new(null);

            public readonly AudioSource audioSource;

            public AudioSourceHolder(AudioSource audioSource)
            {
                this.audioSource = audioSource;
            }
        }
    }
}

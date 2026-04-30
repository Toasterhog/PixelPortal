using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelPortal
{
    public class SoundHandler
    {
        private static SoundEffect[] noises;
        private static Dictionary<SoundEffectInstance, float> decayDic = new Dictionary<SoundEffectInstance, float>();

        public static void innitNoises(SoundEffect[] seArr)
        {
            noises = seArr;
        }
        public static void PlaySoundEffect(int index)
        {
            index = Mathlike.ClampI(index, 0, noises.Length-1); //antar minst en se i arr
            noises[index].Play();

        }
        public static void PlaySoundEffect(int index, float pitch = 0f, float volume = 1f)
        {
            index = Mathlike.ClampI(index, 0, noises.Length-1);
            SoundEffectInstance sei = noises[index].CreateInstance();
            sei.Pitch = pitch;
            sei.Volume = volume;
            sei.Play();
        }
        public static void PlaySoundEffectDecay(int index, float pitch = 0f, float volume = 1f, float duration = 1)
        {
            index = Mathlike.ClampI(index, 0, noises.Length - 1);
            SoundEffectInstance sei = noises[index].CreateInstance();
            sei.Pitch = pitch;
            sei.Volume = volume;
            sei.Play();

            decayDic.Add(sei, 1 / duration);
        }
        

        public static void Update(GameTime gematime)
        {
            float delta = (float)gematime.ElapsedGameTime.TotalSeconds;
            SoundEffectInstance[] keys = new SoundEffectInstance[decayDic.Count];
            decayDic.Keys.CopyTo(keys, 0);

            foreach (SoundEffectInstance sei in keys) 
            {
                sei.Volume = MathF.Max(0, sei.Volume - decayDic[sei] * delta);
                if (sei.Volume <= 0f)
                {
                    sei.Stop();
                    decayDic.Remove(sei);
                }
                else if(sei.State == SoundState.Stopped || sei.IsDisposed)
                {
                    decayDic.Remove(sei);
                }
            }
        }

        


    }
}

//public enum ENoiseNames { shootProj, openPortal, closePortal };
//private static Dictionary<ENoiseNames, SoundEffect> noises;

//public static void PlaySoundEffect(ENoiseNames choice)
//{
//    noises[choice].Play();
//}

//public void initDic(SoundEffect[] soundEffects)
//{
//    foreach (var se in soundEffects)
//    {
//        noises.Add(hehuuhh, se);
//    }
//}
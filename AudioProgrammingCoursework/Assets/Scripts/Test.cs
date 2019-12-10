using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int position = 0;
    const int samplerate = 44100;
    public float frequency = 440;



    List<float> samp = new List<float>();
    List<float> samp2 = new List<float>();
    void Start()
    {
                         

        int length = 3;

        for (int i = 0; i < length; i++)
        {
            samp = Synth(samp, 0.25f, 0 - 21);
            samp = Synth(samp, 0.25f, 1 - 21);
            samp = Synth(samp, 0.25f, 2 - 21);
            samp = Synth(samp, 0.25f, 3 - 21);
            samp = Synth(samp, 0.25f, 2 - 21);
            samp = Synth(samp, 0.25f, 1 - 21);
            samp = Synth(samp, 0.25f, 0 - 21);
            samp = Synth(samp, 0.25f, 1 - 21);
            samp = Synth(samp, 0.25f, 2 - 21);
            samp = Synth(samp, 0.25f, 3 - 21);
            samp = Synth(samp, 0.25f, 2 - 21);
            samp = Synth(samp, 0.25f, 1 - 21);
        }

        for(int j = 0; j < length * 12; j++)
        {
            samp2 = Synth(samp2, 0.25f, 0 + 9);
        }


        AudioClip myClip = AudioClip.Create("Test", samp.Count, 2, samplerate, false);
        myClip.SetData(samp.ToArray(), 0);
        AudioClip myClip2 = AudioClip.Create("Test2", samp2.Count, 2, samplerate, false);
        myClip2.SetData(samp2.ToArray(), 0);



        //AudioClip myClip = AudioClip.Create("MySinusoid", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);


        

        AudioSource aud = GetComponent<AudioSource>();

        //aud.PlayOneShot(myClip, 0.7f);
        //aud.PlayOneShot(myClip2, 0.7f);
        //aud.clip = myClip;
        //aud.Play();
    }


    List<float> Synth(List<float> sample, float length, int note)
    {
        float amplitude = 2.0f;

        int x = note;//69 - note;
        float time = 1.0f;

        //time by which the note should have faded by in milliseconds
        float fadeBy = 0;
        float ampInc = 0;

        frequency = 440 * Mathf.Pow(1.0594f, x);

        if (fadeBy > 0)
        {
            ampInc = (float)amplitude / (samplerate * length / (length * 1000 / fadeBy));
        }

        for (int i = 0; i < samplerate * length; i++)
        {
            time = (float)i / samplerate;
            if (amplitude < 0)
            {
                amplitude = 0;
            }
            sample.Add((amplitude * Mathf.Sin(time * frequency * (2 * 3.14f))) * 1000);

            if (fadeBy > 0)
            {
                amplitude -= ampInc;
            }
        }

        return sample;

    }
    List<float> CombineSampleVectors(List<float> s1, List<float> s2)
    {
        for (int i = 0; i < s2.Count; i++)
        {
            if (i < s1.Count)
            {
                s1[i] += s2[i];
            }
            else
            {
                s1.Add(s2[i]);
            }
        }

        return s1;
    }
}


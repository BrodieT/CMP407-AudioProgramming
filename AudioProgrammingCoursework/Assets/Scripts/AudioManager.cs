using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class AudioManager 
{

    //Creepy Music clips
    static AudioClip musicBass;
    static AudioClip musicAccomp;

    //Win and lose audio clips
    static AudioClip winSound;
    static AudioClip loseSound;

    //Sample rate and frequency
    const int samplerate = 44100;
    static float frequency = 440;

    //This function takes in a name and samples list to produce an audio clip 
    public static AudioClip CreateClip(List<float> samples, string name)
    {
        AudioClip clip;
        clip = AudioClip.Create(name, samples.Count, 1, samplerate, false);
        clip.SetData(samples.ToArray(), 0);
        return clip;
    }

    //Overload function for the create clip function that takes an array of samples instead
    public static AudioClip CreateClip(float[] samples, string name)
    {
        AudioClip clip;
        clip = AudioClip.Create(name, samples.Length, 1, samplerate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    //This function mixes two samples lists together by adding the values together
    public static List<float> MixSamples(List<float> a, List<float> b)
    {
        List<float> c = new List<float>();
        for(int i = 0; i < a.Count; i++)
        {
            if (i < b.Count)
            {
                c.Add(a[i] + b[i]);
            }
            else
            {
                break;
            }
        }

        return c;
    }

    //This function will handle the synthesis of the creepy music to be played when the enemy appears
    //as well as the win/lose sound
    public static void GenerateAudio()
    {
        //The two samples lists for the bass and accompaniments
        List<float> samplesBass = new List<float>();
        List<float> samplesAccomp = new List<float>();

       
        Envelope e = new Envelope();
        e.Init(0.1f, 0.1f, 0.04f, 0.01f, 1.0f, 1.0f);

        List<float> Note1 = new List<float>();
        List<float> Note2 = new List<float>();
        List<float> Note3 = new List<float>();

        //Generate the pulsing bass for the creepy music
        WaveType w = WaveType.SAW;
        for (int i = 0; i < 20; i++)
        {
            Note1 = new List<float>();
            Note2 = new List<float>();
            Note3 = new List<float>();

            Note1 = Synth(Note1, 0.25f, 0 - 21, w, 0.5f);
            Note1 = Modulate(Note1, e);
            Note2 = Synth(Note2, 0.25f, 1 - 21, w, 0.5f);
            Note2 = Modulate(Note2, e);
            Note3 = Synth(Note3, 0.25f, 2 - 21, w, 0.5f);
            Note3 = Modulate(Note3, e);

            samplesBass.AddRange(Note1);
            samplesBass.AddRange(Note2);
            samplesBass.AddRange(Note3);


            Note1 = new List<float>();
            Note2 = new List<float>();
            Note3 = new List<float>();

            Note1 = Synth(Note1, 0.25f, 3 - 21, w, 0.5f);
            Note1 = Modulate(Note1, e);
            Note2 = Synth(Note2, 0.25f, 2 - 21, w, 0.5f);
            Note2 = Modulate(Note2, e);
            Note3 = Synth(Note3, 0.25f, 1 - 21, w, 0.5f);
            Note3 = Modulate(Note3, e);

            samplesBass.AddRange(Note1);
            samplesBass.AddRange(Note2);
            samplesBass.AddRange(Note3);
        }
       
        //Generate the high pitched accompanying tone
        for (int i = 0; i < 20; i++)
        {
            Note1 = new List<float>();
            Note2 = new List<float>();
            Note3 = new List<float>();

            Note1 = Synth(Note1, 0.25f, 0 - 1, w, 0.5f);
            Note1 = Modulate(Note1, e);
            Note2 = Synth(Note2, 0.25f, 0 - 2, w, 0.5f);
            Note2 = Modulate(Note2, e);
            Note3 = Synth(Note3, 0.25f, 0 - 3, w, 0.5f);
            Note3 = Modulate(Note3, e);

            samplesAccomp.AddRange(Note1);
            samplesAccomp.AddRange(Note2);
            samplesAccomp.AddRange(Note3);


            Note1 = new List<float>();
            Note2 = new List<float>();

            Note1 = Synth(Note1, 0.25f, 0 - 2, w, 0.5f);
            Note1 = Modulate(Note1, e);
            Note2 = Synth(Note2, 0.25f, 0 - 1, w, 0.5f);
            Note2 = Modulate(Note2, e);


            samplesAccomp.AddRange(Note1);
            samplesAccomp.AddRange(Note2);
        }


        samplesBass = AlterGain(samplesBass, 0.01f);
        samplesAccomp = AlterGain(samplesAccomp, 0.01f);

        samplesBass = TrimSilence(samplesBass, 0.2f);

        //Create the bass audio clip
        musicBass = AudioClip.Create("musicBass", samplesBass.Count, 2, samplerate, false);
        musicBass.SetData(samplesBass.ToArray(), 0);

        //Create the accompaniments audio clip
        musicAccomp = AudioClip.Create("musicAccomp", samplesAccomp.Count, 2, samplerate, false);
        musicAccomp.SetData(samplesAccomp.ToArray(), 0);



        w = WaveType.TRIANGLE;
        //Create the win and lose audio
        e.Init(0.01f, 0, 0.8f, 0.01f, 1.0f, 1.0f);

        List<float> winSamples = new List<float>();
        List<float> loseSamples = new List<float>();

        Note1 = new List<float>();
        Note2 = new List<float>();
        Note3 = new List<float>();

        Note1 = Synth(Note1, 0.1f, 0 - 9, w, 1.0f);
        Note1 = Modulate(Note1, e);
        Note2 = Synth(Note2, 0.1f, 0 - 7, w, 1.0f);
        Note2 = Modulate(Note2, e);

        e.Init(0.01f, 0.05f, 0.05f, 0.069f, 1.0f, 1.0f);

        Note3 = Synth(Note3, 0.075f, 0 - 9, w, 1.0f);
        Note3 = Modulate(Note3, e);

        winSamples.AddRange(Note1);
        winSamples.AddRange(Note2);
        winSamples.AddRange(Note3);

        e.Init(0.01f, 0, 0.18f, 0.01f, 1.0f, 1.0f);

        Note3 = new List<float>();
        Note3 = Synth(Note3, 0.2f, 0 - 1, w, 1.0f);
        Note3 = Modulate(Note3, e);
        winSamples.AddRange(Note3);


        Note1 = new List<float>();
        Note2 = new List<float>();
        Note3 = new List<float>();

        Note1 = Synth(Note1, 0.1f, 0 - 10, w, 1.0f);
        Note1 = Modulate(Note1, e);
        Note2 = Synth(Note2, 0.1f, 0 - 8, w, 1.0f);
        Note2 = Modulate(Note2, e);

        e.Init(0.01f, 0.05f, 0.05f, 0.069f, 1.0f, 1.0f);

        Note3 = Synth(Note3, 0.075f, 0 - 10, w, 1.0f);
        Note3 = Modulate(Note3, e);


        loseSamples.AddRange(Note1);
        loseSamples.AddRange(Note2);
        loseSamples.AddRange(Note3);

        e.Init(0.01f, 0, 0.18f, 0.01f, 1.0f, 1.0f);
        Note3 = new List<float>();
        Note3 = Synth(Note3, 0.075f, 0 - 12, w, 1.0f);
        Note3 = Modulate(Note3, e);
        loseSamples.AddRange(Note3);


        //Create the bass audio clip
        winSound = AudioClip.Create("Win Sound", winSamples.Count, 1, samplerate, false);
        winSound.SetData(winSamples.ToArray(), 0);

        //Create the accompaniments audio clip
        loseSound = AudioClip.Create("Lose Sound", loseSamples.Count, 1, samplerate, false);
        loseSound.SetData(loseSamples.ToArray(), 0);

    }

    public static List<float> TrimSilence(List<float> samples, float min)
    {
        int i = 0;

        for(i = 0; i < samples.Count; i++)
        {
            if(Mathf.Abs(samples[i]) > min)
            { 
                break;
            }
        }

        samples.RemoveRange(0, i);

        for(i = samples.Count - 1; i > 0; i--)
        {
            if(Mathf.Abs(samples[i]) > min)
            {
                break;
            }
        }
        samples.RemoveRange(i, samples.Count - i);


        return samples;
    }
    public static List<float> AlterGain(List<float> samples, float multiplier)
    {
        for(int i = 0; i < samples.Count; i++)
        {
            samples[i] *= multiplier;
        }

        return samples;
    }

    public static AudioClip getAccomp()
    {
        return musicAccomp;
    }
    public static AudioClip getBass()
    {
        return musicBass;
    }
    public static AudioClip getWin()
    {
        return winSound;
    }

    public static AudioClip getLose()
    {
        return loseSound;
    }

    public enum WaveType {  SINE, SQUARE, TRIANGLE, SAW, NOISE};

    //This function will generate the harmonica 
    //It will recieve a note value and will create the three components for the harmonica 
    //The three components are then mixed and an audio clip is created from the result
    public static AudioClip GenerateHarmonica(int note)
    {
        Envelope e = new Envelope();
        e.Init(0.05f, 0.05f, 0.7f, 0.2f, 1.0f, 0.95f);
        List<float> Samples1 = new List<float>();
        List<float> Samples2 = new List<float>();
        List<float> Samples3 = new List<float>();
        List<float> mixed = new List<float>();


        Samples1 = Synth(Samples1, 1, note, WaveType.SQUARE, 1);
        Samples1 = AlterGain(Samples1, 0.065f);
        Samples1 = Modulate(Samples1, e);
     
        Samples2 = new List<float>();
        Samples2 = Synth(Samples2, 1, note + 12, WaveType.SQUARE, 1);
        Samples2 = AlterGain(Samples2, 0.03f);
        Samples2 = Modulate(Samples2, e);
       

        Samples3 = new List<float>();
        Samples3 = Synth(Samples3, 1, note + 12, WaveType.NOISE, 1);
        Samples3 = AlterGain(Samples3, 0.005f);
        Samples3 = Modulate(Samples3, e);


        mixed = MixSamples(Samples1, Samples2);
        mixed = MixSamples(mixed, Samples3);
        mixed = Modulate(mixed, e);
        mixed = AlterGain(mixed, 0.25f);

        return CreateClip(mixed, "Base");
    }

    //This function handles the synthesis of notes
    //It recieves the samples list it wants to add to, the length of the note, and the note number
    //https://github.com/SFML/SFML/wiki/Synth
    public static List<float> Synth(List<float> sample, float length, int note, WaveType wave, float amplitude)
    {

        int x = note;//69 - note;
        float time = 1.0f;


        frequency = 440 * Mathf.Pow(1.0594f, x);

       

        for (int i = 0; i < samplerate * length; i++)
        {
            time = (float)i / samplerate;
            if (amplitude < 0)
            {
                amplitude = 0;
            }
            switch(wave)
            {
                case WaveType.SINE:
                    sample.Add((amplitude * Mathf.Sin(time * frequency * (2 * Mathf.PI))) * 1000);
                    break;
                case WaveType.SQUARE:
                    sample.Add((Mathf.Sin(time * frequency * (2 * 3.14f)) > 0 ? 1.0f : -1.0f)*1000);
                    break;
                case WaveType.TRIANGLE:
                    sample.Add((Mathf.Asin(Mathf.Sin(time * frequency * (2 * Mathf.PI)) * (2.0f / Mathf.PI))) * 1000);
                    break;
                case WaveType.SAW:
                    float remainder = time % (1 / frequency);
                    sample.Add(((2.0f * Mathf.PI) * (frequency * Mathf.PI * remainder) - (Mathf.PI / 2.0f)) * 1000);
                    break;
                case WaveType.NOISE:
                    sample.Add((2.0f * (Random.Range(0, float.MaxValue) / float.MaxValue) - 1.0f) * 1000.0f) ;
                    break;
                default:
                    sample.Add((amplitude * Mathf.Sin(time * frequency * (2 * Mathf.PI))) * 1000);
                    break;
            }
        }

        return sample;

    }

    //FIXME Doesnt work
    public static List<float> HighPassFilter(List<float> samples)
    {
        List<float> output = new List<float>(samples.Count);
        float cutoff = 440;
        cutoff /= samplerate;
        float a = 2 * Mathf.PI * cutoff;
        int mid = samples.Count / 2;

        for(int i = 0; i < samples.Count; i++)
        {
            //if(i == 0)
            //{
            //    output[mid] = 1 - (2 * cutoff);
            //}
            //else
            //{
                output.Add(samples[i] -Mathf.Sin(((2 * Mathf.PI * cutoff) * i) / (Mathf.PI * i)));
            //}
        }



        return output;
    }
   
    //This function performs a low pass filter on a given sample list
    public static List<float> LowPassFilter(List<float> sample)
    {
        float[] coeff = new float[7] { 0.0152193f, 0.08115433f, 0.23941581f, 0.32842112f, 0.23941581f, 0.08115433f, 0.0152193f };
        float[] i = new float[7];


        for (int x = 0; x < sample.Count; x++)
        {
            for (int y = 0; y < 7; y++)
            {

                if (y < 4)
                {
                    if (x - y < 0)
                    {
                        i[y] = 0;
                    }
                    else
                    {
                        i[y] = sample[x - y];
                    }
                }
                else
                {
                    if (x + (7 - y) >= sample.Count)
                    {
                        i[y] = 0;
                    }
                    else
                    {
                        i[y] = sample[x + (7 - y)];
                    }
                }
            }

            sample[x] = 0;

            for (int z = 0; z < 7; z++)
            {
                sample[x] += i[z] * coeff[z];
            }
        }

        return sample;
    }

    //This function uses an envelope to modulate a given sample list
    static public List<float> Modulate(List<float> samples, Envelope env)
    {

        float TimeOn = 0;
        float TimeOff = env.attackTime + env.decayTime + env.sustainTime;
        float amp = 0;
        
        
        for(int i = 0; i < samples.Count; i++)
        {     
            float t = (float)i / samplerate;

            if(t > TimeOff)
            {
                amp = ((t - TimeOff) / env.releaseTime) * (0.0f - env.sustainAmp) + env.sustainAmp;
            }
            else if(t > (env.attackTime + env.decayTime))
            {
                amp = env.sustainAmp;
            }
            else if (t > env.attackTime && t <= (env.attackTime + env.decayTime))
            {
                amp = ((t - env.attackTime) / env.decayTime) * (env.sustainAmp - env.startAmp) + env.startAmp;
            }
            else if(t <= env.attackTime)
            {
                amp = (t / env.attackTime) * env.startAmp;
            }
            
            if(amp <= 0)
            {
                amp = 0;
            }

            samples[i] *= amp *  Mathf.Sin((frequency * Mathf.PI * 2.0f)*t);
        }
        return samples;
    }

    static List<float> AddSamples(List<float> s1, List<float> s2)
    {
        s1.AddRange(s2);
        
        return s1;
    }

    //This function creates a samples list with a delay at the start
    public static List<float> AddDelay(List<float> samples, int delay)
    {
        List<float> delayed = new List<float>();

        for(int i = 0; i < delay; i++)
        {
            delayed.Add(0);
        }

        delayed.AddRange(samples);
        return delayed;
    }
    //This function creates a simplistic echo effect utilising a delay, a low pass filter, and attenuation
    static public List<float> Reverb(List<float> samples, int delayInc, int reverbCount)
    {
        List<float> reverb;
        reverb = samples;
        for (int i = 1; i < reverbCount + 1; i++)
        {
            reverb = AddSamples(reverb, LowPassFilter(AlterGain(AddDelay(samples, delayInc*i), 1 - ((1/(reverbCount + 1)) * i))));

        }

        return reverb;
    }

    public struct Envelope
    {
        public float attackTime;
        public float decayTime;
        public float sustainTime;
        public float releaseTime;
        public float startAmp;
        public float sustainAmp;

        public void Init(float attack, float decay, float sustain, float release, float stAmp, float susAmp)
        {
            attackTime = attack;
            decayTime = decay;
            sustainTime = sustain;
            releaseTime = release;
            startAmp = stAmp;
            sustainAmp = susAmp;
        }
    }
}

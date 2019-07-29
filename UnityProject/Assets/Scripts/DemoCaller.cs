using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Include these namespaces to use BinaryFormatter
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using NAudio.Wave;


namespace Scripts
{

    // Demo class to illustrate the usage of the FileBrowser script
    // Able to save and load files containing serialized data (e.g. text)
    public class DemoCaller : MonoBehaviour
    {
        public static GameObject curPoster;
        public static List<Sprite> posters = new List<Sprite>();
        public static AudioSource radio;
        // Use the file browser prefab
        public GameObject FileBrowserPrefab;

        // Define a file extension
        public string[] FileExtensions;

        // Input field to get text to save
        private GameObject _textToSaveInputField;

        // Label to display loaded text
        private GameObject _loadedText;

        // Variable to save intermediate input result
        private string _textToSave;

        public bool PortraitMode;

        public Image img;
        public GameObject UI;
        // Find the input field, label objects and add a onValueChanged listener to the input field
        private void Start()
        {
            
            _textToSaveInputField = GameObject.Find("TextToSaveInputField");
            _textToSaveInputField.GetComponent<InputField>().onValueChanged.AddListener(UpdateTextToSave);

            _loadedText = GameObject.Find("LoadedText");

            GameObject uiCanvas = GameObject.Find("Canvas");
            if (uiCanvas == null)
            {
                Debug.LogError("Make sure there is a canvas GameObject present in the Hierarcy (Create UI/Canvas)");
            }
            UI.SetActive(false);
        }

        // Updates the text to save with the new input (current text in input field)
        public void UpdateTextToSave(string text)
        {
            _textToSave = text;
        }

        // Open the file browser using boolean parameter so it can be called in GUI
        public void OpenFileBrowser(bool saving)
        {
            OpenFileBrowser(saving ? FileBrowserMode.Save : FileBrowserMode.Load);
        }

        // Open a file browser to save and load files
        private void OpenFileBrowser(FileBrowserMode fileBrowserMode)
        {
            // Create the file browser and name it
            GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
            fileBrowserObject.name = "FileBrowser";
            // Set the mode to save or load
            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser(PortraitMode ? ViewMode.Portrait : ViewMode.Landscape);
            if (fileBrowserMode == FileBrowserMode.Save)
            {
                fileBrowserScript.SaveFilePanel("DemoText", FileExtensions);
                // Subscribe to OnFileSelect event (call SaveFileUsingPath using path) 
                fileBrowserScript.OnFileSelect += SaveFileUsingPath;
            }
            else
            {
                fileBrowserScript.OpenFilePanel(FileExtensions);
                // Subscribe to OnFileSelect event (call LoadFileUsingPath using path) 
                fileBrowserScript.OnFileSelect += LoadFileUsingPath;
            }
        }

        // Saves a file with the textToSave using a path
        private void SaveFileUsingPath(string path)
        {
            // Make sure path and _textToSave is not null or empty
            if (!String.IsNullOrEmpty(path) && !String.IsNullOrEmpty(_textToSave))
            {
                BinaryFormatter bFormatter = new BinaryFormatter();
                // Create a file using the path
                FileStream file = File.Create(path);
                // Serialize the data (textToSave)
                bFormatter.Serialize(file, _textToSave);
                // Close the created file
                file.Close();
            }
            else
            {
                Debug.Log("Invalid path or empty file given");
            }
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        // Loads a file using a path
        private void LoadFileUsingPath(string path)
        {
            // Open the file using the path

            byte[] bytes = File.ReadAllBytes(path);
            if (path.EndsWith(".mp3"))
            {
                Debug.Log("Is reading mp3 file");
                AudioClip clip = loadFromMp3(bytes);
                clip.name = path.Substring(path.LastIndexOf('\\') + 1);
                clip.name=clip.name.Remove(clip.name.Length - 4); ;
                radio.clip = clip;
                Debug.Log("ClipName: "+clip.name);
                MyGUI.setInfoTextFadeout("Loaded clip '"+clip.name+"' in radio.\nPress Play to groove!");
            }
            else
            {
                int size = 10;
                if (curPoster.name.Contains("Middle")) size = 20;
                else if (curPoster.name.Contains("XXL")) size = 30;
                Texture2D tex = new Texture2D(10, 10);
                tex.LoadImage(bytes);
                //Sprite s = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.width/size);
                curPoster.GetComponent<MeshRenderer>().materials[0].SetTexture(1, tex);
                //SpriteRenderer sr = curPoster.GetComponent<SpriteRenderer>();
                //sr.sprite = s;
                //Debug.Log(sr.sprite.ToString());
                //Debug.Log(curPoster.GetComponent<SpriteRenderer>().sprite.ToString());
                MyGUI.setInfoTextFadeout("Created Poster from your Image. Looks fine!");
            }
            Camera.main.GetComponent<MyGUI>().closeAllContentPanels();
        }

        public static AudioClip loadFromMp3(byte[] data)
        {
            // Load the data into a stream
            MemoryStream mp3Stream = new MemoryStream(data);
            // Convert the data in the stream to WAV format
            Mp3FileReader mp3Audio = new Mp3FileReader(mp3Stream);
            WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(mp3Audio);
            // Convert to WAV data
            WAV wav = new WAV(audioMemStream(waveStream).ToArray());

            int channels = wav.ChannelCount;
            AudioClip audioClip = AudioClip.Create("testSound", wav.SampleCount, channels, wav.Frequency, false);

            switch (channels)
            {
                case 1:
                    setMonoAudioData(audioClip, wav);
                    break;
                case 2:
                    setStereoAudioData(audioClip, wav);
                    break;
                default:
                    throw new ArgumentException("Channel count not supported: " + channels);
            }
            
            return audioClip;
        }

        private static MemoryStream audioMemStream(WaveStream waveStream)
        {
            MemoryStream outputStream = new MemoryStream();
            using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream, waveStream.WaveFormat))
            {
                Debug.Log(waveStream.Length);
                byte[] bytes = new byte[waveStream.Length];
                waveStream.Position = 0;
                waveStream.Read(bytes, 0, Convert.ToInt32(waveStream.Length));
                waveFileWriter.Write(bytes, 0, bytes.Length);
                waveFileWriter.Flush();
            }
            return outputStream;
        }

        private static void setMonoAudioData(AudioClip audioClip, WAV wav)
        {
            audioClip.SetData(wav.LeftChannel, 0);
        }

        private static void setStereoAudioData(AudioClip audioClip, WAV wav)
        {
            // create new float for interleaved audio contents
            float[] combinedChannels = new float[wav.LeftChannel.Length + wav.RightChannel.Length];
            int pointer = 0;
            int lpointer = 0;
            int rpointer = 0;
            while (pointer < combinedChannels.Length)
            {
                combinedChannels[pointer] = wav.LeftChannel[lpointer];
                lpointer++;
                pointer++;
                combinedChannels[pointer] = wav.RightChannel[rpointer];
                rpointer++;
                pointer++;
            }

            audioClip.SetData(combinedChannels, 0);
        }
        public void setRadio(AudioSource audio)
        {
            radio = audio;
        }
        public void addPoster(Sprite poster) {
            posters.Add(poster);
        }
        public void setCurPoster(GameObject p)
        {
            curPoster = p;
            Debug.Log(p.ToString());
        }
    }
    

}

public class WAV
{

    // convert two bytes to one float in the range -1 to 1
    static float bytesToFloat(byte firstByte, byte secondByte)
    {
        // convert two bytes to one short (little endian)
        short s = (short)((secondByte << 8) | firstByte);
        // convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }

    static int bytesToInt(byte[] bytes, int offset = 0)
    {
        int value = 0;
        for (int i = 0; i < 4; i++)
        {
            value |= ((int)bytes[offset + i]) << (i * 8);
        }
        return value;
    }
    // properties
    public float[] LeftChannel { get; internal set; }
    public float[] RightChannel { get; internal set; }
    public int ChannelCount { get; internal set; }
    public int SampleCount { get; internal set; }
    public int Frequency { get; internal set; }

    public WAV(byte[] wav)
    {

        // Determine if mono or stereo
        ChannelCount = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels

        // Get the frequency
        Frequency = bytesToInt(wav, 24);

        // Get past all the other sub chunks to get to the data subchunk:
        int pos = 12;   // First Subchunk ID from 12 to 16

        // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
        while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
        {
            pos += 4;
            int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
            pos += 4 + chunkSize;
        }
        pos += 8;

        // Pos is now positioned to start of actual sound data.
        SampleCount = (wav.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)
        if (ChannelCount == 2) SampleCount /= 2;        // 4 bytes per sample (16 bit stereo)

        // Allocate memory (right will be null if only mono sound)
        LeftChannel = new float[SampleCount];
        if (ChannelCount == 2) RightChannel = new float[SampleCount];
        else RightChannel = null;

        // Write to double array/s:
        int i = 0;
        while (pos < wav.Length)
        {
            LeftChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
            pos += 2;
            if (ChannelCount == 2)
            {
                RightChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
            }
            i++;
        }
    }

    public override string ToString()
    {
        return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", LeftChannel, RightChannel, ChannelCount, SampleCount, Frequency);
    }
}
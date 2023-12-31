﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace PictureImpoverishment
{
    internal class Program
    {
        public const int timer = 2000;
        public const int timerFiles = 250;
        public const int timerIntermission = 500;

        public static readonly string preprossessingIMGFilePath = @"IMG\Unmod\";
        public static readonly string savingIMGFilePath = @"IMG\Mod\";

        public static readonly string audioFilePathSuccess = @"Audio\successAudio.mp3";
        public static readonly string audioFilePathFinished = @"Audio\finishedAudio.mp3";
        public static readonly string audioFilePath = @"Audio\Background\";
        static bool stopMusic = false;

        // Import the necessary Windows API functions from kernel32.dll
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_MAXIMIZE = 3;

        static void Main(string[] args)
        {
            IntPtr hWnd = GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, SW_MAXIMIZE);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("                        --------------------------------------------------------------------------------------------------");
            Console.WriteLine("                        |                     --> PICTURE IMPOVERISHMENT V1.3 Release Build <--                          |");
            Console.WriteLine("                        --------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("                        Press any button to continue.");            

            try
            {
                List<string> audioFiles = GetAudioFiles();

                // Start a new thread to play background music
                Thread musicThread = new Thread(() => PlayRandomBackgroundMusic(audioFiles));
                musicThread.Start();

                Console.ReadKey();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;

                int files = RenameFilesIncremental(preprossessingIMGFilePath);
                LoadPicture(files);

                // Once the main logic is done, signal the music thread to stop
                stopMusic = true;
                musicThread.Join(); // Wait for the music thread to stop
            }
            catch (Exception ex)
            {
                Console.WriteLine($"                        An error occurred: {ex.Message}");
            }            
        }
        static List<string> GetAudioFiles()
        {
            string[] audioFiles = Directory.GetFiles(audioFilePath, "*.mp3");
            List<string> files = new List<string>();
            foreach (var item in audioFiles)
            {
                files.Add(item);
            }
            return files;
        }
        static void PlayRandomBackgroundMusic(List<string> audioFiles)
        {
            Random random = new Random();

            while (!stopMusic)
            {
                string randomAudioFilePath = audioFiles[random.Next(audioFiles.Count)];

                using (AudioFileReader audioFile = new AudioFileReader(randomAudioFilePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Volume = 0.5f;
                    outputDevice.Play();

                    while (!stopMusic && outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(timer);
                    }
                }
            }
        }
        static int RenameFilesIncremental(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("                        Directory not found.");
            }
            Console.CursorVisible = false;
            Console.WriteLine("                        DIRECTORY FOUND...");
            Console.WriteLine();
            Thread.Sleep(1000);

            string[] files = Directory.GetFiles(directoryPath);

            for (int i = 0; i < files.Length; i++)
            {
                string oldFilePath = files[i];
                string newFileName = $"IMG_{i + 1:D4}.jpg";
                string newFilePath = Path.Combine(directoryPath, newFileName);

                try
                {
                    File.Move(oldFilePath, newFilePath);
                    Console.WriteLine($"                        Renamed '{Path.GetFileName(oldFilePath)}' to '{newFileName}'.");
                    Thread.Sleep(timerFiles);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"                        Error renaming '{Path.GetFileName(oldFilePath)}': {ex.Message}");
                }
            }

            return files.Length;
        }
        static void LoadPicture(int files)
        {
            for (int i = 1; i < files + 1; i++)
            {
                string formattedCounter = String.Format("{0:D4}", i);
                string filename = $"IMG_{formattedCounter}.jpg";

                Console.Clear();
                Console.WriteLine($"                        LOADING 'IMG_{formattedCounter}.JPG'...");
                Thread.Sleep(timerIntermission);

                try
                {                    
                    using (Bitmap imgLoad = new Bitmap(preprossessingIMGFilePath + filename))
                    {
                        Console.WriteLine("                        CALCULATING AVG COLOR VALUE...");
                        Thread.Sleep(timer);
                        double avg = Math.Round(CalculateAverage(imgLoad, filename));

                        Console.Clear();
                        Console.WriteLine($"                        AVERAGE R/G/B VALUE IS: {avg}");
                        Thread.Sleep(timerIntermission);
                        Console.ForegroundColor = ConsoleColor.White;

                        Console.WriteLine("                        CALCULATING INDIVIDUAL PIXELS...");
                        Thread.Sleep(timer);
                        CalculatePixel(imgLoad, filename, avg);
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine("                        An error occurred while loading the image: " + ex.Message);
                }
            }

            Thread.Sleep(1000);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                        ALL PICTURES PROCESSED.");
            PlayAudio(audioFilePathFinished);

            Console.WriteLine();
            Console.WriteLine("                        Closing in 60 seconds.");
            Thread.Sleep(10000);
            Console.WriteLine("                        Closing in 50 seconds.");
            Thread.Sleep(10000);
            Console.WriteLine("                        Closing in 40 seconds.");
            Thread.Sleep(10000);
            Console.WriteLine("                        Closing in 30 seconds.");
            Thread.Sleep(10000);
            Console.WriteLine("                        Closing in 20 seconds.");
            Thread.Sleep(10000);
            Console.WriteLine("                        Closing in 10 seconds.");
            Thread.Sleep(5000);
            Console.WriteLine("                        Closing in 5 seconds.");
            Thread.Sleep(1000);
            Console.WriteLine("                        Closing in 4 seconds.");
            Thread.Sleep(1000);
            Console.WriteLine("                        Closing in 3 seconds.");
            Thread.Sleep(1000);
            Console.WriteLine("                        Closing in 2 seconds.");
            Thread.Sleep(1000);
            Console.WriteLine("                        Closing in 1 seconds.");
            Thread.Sleep(1000);
        }
        static double CalculateAverage(Bitmap imgCalc, string filename)
        {
            double i = 0;
            double max = imgCalc.Height * imgCalc.Width;
            double percentage = 0;
            double value = 0;
            string formattedMax = (max % 100000000).ToString("00\\.000\\.000");

            Console.Clear();

            // Loop through each pixel
            for (int y = 0; y < imgCalc.Height; y++)
            {
                for (int x = 0; x < imgCalc.Width; x++)
                {
                    Color pixelColor = imgCalc.GetPixel(x, y);

                    value += pixelColor.R + pixelColor.G + pixelColor.B;

                    i++;

                    if (i % 100000 == 0)
                    {
                        string formattedCounter = (i % 100000000).ToString("00\\.000\\.000");
                        string formattedPercent = (Math.Round(percentage, 1) * 100).ToString("00\\,00\\%");
                        percentage = (i / max) * 100;
                        Console.WriteLine("             STEP 1/2 || Calculating average pixel value || " + formattedCounter + "/" + formattedMax + " pixel processed || " + formattedPercent + $" --> {filename}");
                    }
                }
            }

            return (value / 3) / max;
        }
        static void CalculatePixel(Bitmap imgCalc, string filename, double avg)
        {
            double i = 0;
            double max = imgCalc.Height * imgCalc.Width;
            double percentage = 0;
            string formattedMax = (max % 100000000).ToString("00\\.000\\.000");

            Console.Clear();

            // Loop through each pixel
            for (int y = 0; y < imgCalc.Height; y++)
            {
                for (int x = 0; x < imgCalc.Width; x++)
                {
                    // Get the color of the current pixel
                    Color pixelColor = imgCalc.GetPixel(x, y);

                    // Change the color of the pixel
                    Color newColor = PickColor(pixelColor, avg);

                    // Set the new color of the pixel
                    imgCalc.SetPixel(x, y, newColor);

                    i++;

                    if (i % 100000 == 0)
                    {
                        string formattedCounter = (i % 100000000).ToString("00\\.000\\.000");
                        string formattedPercent = (Math.Round(percentage, 1) * 100).ToString("00\\,00\\%");
                        percentage = (i / max) * 100;
                        Console.WriteLine("             STEP 2/2 || Calculating new pixel color || " + formattedCounter + "/" + formattedMax + " pixel processed || " + formattedPercent + $" --> {filename}");
                    }
                }
            }

            SavePicture(imgCalc, filename);
        }
        static Color PickColor(Color pxcolor, double avg)
        {
            if (pxcolor.R < avg && pxcolor.G < avg && pxcolor.B < avg)
            {
                return Color.Black;
            }

            return Color.White;
        }
        static void SavePicture(Bitmap imgSave, string filename)
        {
            try
            {                
                imgSave.Save(savingIMGFilePath + "MOD_" + filename);
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine("                        An error occurred while saving the image: " + ex.Message);
                Console.ReadKey();
            }
            finally
            {
                imgSave.Dispose();

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"                        PROCESSING FINISHED. YOUR FILE WAS SAVED SUCCESSFULLY AS 'MOD_{filename}'!");
                PlayAudio(audioFilePathSuccess);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        static void PlayAudio(string audioFilePath)
        {
            using (AudioFileReader audioFile = new AudioFileReader(audioFilePath))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                Thread.Sleep(timer);
            }
        }
    }
}
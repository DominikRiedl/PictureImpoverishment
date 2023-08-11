using System;
using System.Drawing;

namespace PictureImpoverishment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start? (Y/N)");

            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.Clear();
                LoadPicture();
            }
        }
        public static void LoadPicture()
        {
            try
            {
                // Load the image
                using (Bitmap imgload = new Bitmap("C:/Users/domrie01/Documents/PictureImpoverishment/PictureImpoverishment/IMG/Unmod/img1.jpg"))
                {
                    CalculatePixel(imgload);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while loading the image: " + ex.Message);
                Console.ReadKey();
            }
        }
        public static void CalculatePixel(Bitmap imgcalc)
        {
            double i = 0;
            double max = imgcalc.Height * imgcalc.Width;
            double cent = 0;
            // Loop through each pixel
            for (int y = 0; y < imgcalc.Height; y++)
            {
                for (int x = 0; x < imgcalc.Width; x++)
                {
                    // Get the color of the current pixel
                    Color pixelColor = imgcalc.GetPixel(x, y);

                    // Change the color of the pixel
                    Color newColor = PickColor(pixelColor);

                    // Set the new color of the pixel
                    imgcalc.SetPixel(x, y, newColor);

                    i++;

                    if(i % 10000  == 0)
                    {
                        cent = (i / max) * 100;
                        Console.WriteLine(i + " of " + max + " pixel processed || " + Math.Round(cent,2) + "% finished");
                    }
                }
            }

            SavePicture(imgcalc);
        }
        public static Color PickColor(Color pxcolor)
        {
            Color[] standardColors = new Color[]
            {
                Color.Black,
                Color.White                
            };

            Color closestColor = standardColors[0];

            int closestDistance = int.MaxValue;

            foreach (Color color in standardColors)
            {
                int distance = CalculateColorDistance(pxcolor, color);

                if (distance < closestDistance)
                {
                    closestColor = color;
                    closestDistance = distance;
                }
            }

            return closestColor;
        }

        private static int CalculateColorDistance(Color color1, Color color2)
        {
            int redDifference = color1.R - color2.R;
            int greenDifference = color1.G - color2.G;
            int blueDifference = color1.B - color2.B;

            return (redDifference * redDifference) + (greenDifference * greenDifference) + (blueDifference * blueDifference);
        }

        public static void SavePicture(Bitmap imgsave)
        {
            try
            {
                // Save the modified image
                imgsave.Save($"C:/Users/domrie01/Documents/PictureImpoverishment/PictureImpoverishment/IMG/Mod/mod_img.png");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while saving the image: " + ex.Message);
            }
            finally
            {
                imgsave.Dispose();

                Console.WriteLine();
                Console.WriteLine("Image Processing finished, push any button to close.");
                Console.ReadKey();
            }
        }
    }
}
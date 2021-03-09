using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteGenerator : MonoBehaviour {
    public Pixel_Data[,] SpriteArray;
    int Width;
    int Height;
    public static SpriteGenerator I;

    private void Awake() {
        I = this;
    }

    //Creates a new Array to work with, this needs to be done first for every generator
    public void NewSpriteArray(int width, int height, Color DefultColour) {
        Width = width;
        Height = height;
        SpriteArray = new Pixel_Data[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                SpriteArray[x, y] = new Pixel_Data(DefultColour, 0);
            }
        }
    }

    //retruns any pixels colour or state
    public Pixel_Data GetPixel(int x, int y) {
        if (x > Width - 1 || x < 0 || y > Height - 1 || y < 0) {
            return null;
        }
        return SpriteArray[x, y];
    }

    //Fills with perlean noise
    public void PerleanNoiseTiles(float scale, float offset, float fill) {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                float SampleX = (x + offset) / scale;
                float SampleY = (y + offset) / scale;
                float noise = Mathf.PerlinNoise(SampleX, SampleY);
                if (noise >= fill) {
                    SpriteArray[x, y].Shape = 1;
                }
            }
        }
    }

    //Returns the number of cells surrounding a given cell, including diagionals for a max of 8
    public int ReturnNeighbors(int x, int y, int shape) {
        int Neighbours = 0;
        for (int neightborx = x - 1; neightborx <= x + 1; neightborx++) {
            for (int neightbory = y - 1; neightbory <= y + 1; neightbory++) {
                if (neightborx >= 0 && neightborx < Width && neightbory >= 0 && neightbory < Height) {
                    if (neightborx == x && neightbory == y) {
                        continue;
                    }
                    if (SpriteArray[neightborx, neightbory].Shape == shape) {
                        Neighbours++;
                    }
                }
            }
        }
        return Neighbours;
    }

    //Fills the grid with random on or off cells, this uses a percentage
    public void RandomFill(float fill) {
        var seed = Random.Range(1.11111f, 1000f);
        System.Random psudoRandom = new System.Random(seed.GetHashCode());
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                SpriteArray[x, y].Shape = (psudoRandom.Next(0, 100) < fill) ? 1 : 0;
            }
        }
    }

    //Picks a random hue to fill in to the shape and minimizes cleanup
    public void FillShapesWithRandomHue(float FillMax, float FillMin, float OutlineValue) {

        Dictionary<int, ColourData> ColourFill = new Dictionary<int, ColourData>();
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (SpriteArray[x, y].Shape == 0) {
                    continue;
                }

                if (ColourFill.ContainsKey(SpriteArray[x, y].Shape)) {
                    if (SpriteArray[x, y].outline == false) {
                        SpriteArray[x, y].Colour = ColourFill[SpriteArray[x, y].Shape].Colour;
                    }
                    else {
                        SpriteArray[x, y].Colour = ColourFill[SpriteArray[x, y].Shape].Outline;
                    }
                }
                else {
                    Color RandomColour = new Color(
                    Random.Range(FillMin, FillMax),
                    Random.Range(FillMin, FillMax),
                    Random.Range(FillMin, FillMax));
                    Color OutlineColour = new Color(RandomColour.r - OutlineValue / 2, RandomColour.g - OutlineValue / 2, RandomColour.b - OutlineValue / 2);
                    ColourData data = new ColourData();
                    data.Colour = RandomColour;
                    data.Outline = OutlineColour;
                    ColourFill.Add(SpriteArray[x, y].Shape, data);


                    if (SpriteArray[x, y].outline == false) {
                        SpriteArray[x, y].Colour = ColourFill[SpriteArray[x, y].Shape].Colour;
                    }
                    else {
                        SpriteArray[x, y].Colour = ColourFill[SpriteArray[x, y].Shape].Outline;
                    }
                }
            }
        }
    }

    //picks colours from a gradient, "lerp" picks two colours and lerps between them from top to bottom
    //Glow creates a 1/4 chance to make the outline colour brighter than the fill colour
    public int FillShapesFromGradient(Gradient gradient,float OutlineValue,bool Lerp,bool Glow) {
        Color RandomColour = gradient.Evaluate(Random.Range(0f, 1f));
        Color SecondColour = gradient.Evaluate(Random.Range(0f, 1f));
        int Roll = 1;
        if(Lerp == true && Glow == true) {
            Roll = Random.Range(0, 4);
        }

        Color OutlineColour = RandomColour;
        if(Roll >= 1) {
            OutlineColour = new Color(RandomColour.r - OutlineValue / 2, RandomColour.g - OutlineValue / 2, RandomColour.b - OutlineValue / 2);
        }
        else {
            OutlineColour = new Color(RandomColour.r * 1.2f, RandomColour.g * 1.2f, RandomColour.b * 1.2f);
        }
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (SpriteArray[x, y].Shape == 0) {
                    continue;
                }
                else {
                    if(Lerp == false) {
                        if (SpriteArray[x, y].outline == false) {
                            SpriteArray[x, y].Colour = RandomColour;
                        }
                        else {
                            SpriteArray[x, y].Colour = OutlineColour;
                        }
                    }
                    else {
                        float ycount = y;
                        float heightcount = Height;
                        float ColourLerpCounter = y / heightcount;
                        if (SpriteArray[x, y].outline == false) {
                            SpriteArray[x, y].Colour = Color.Lerp(RandomColour, SecondColour, ColourLerpCounter); ;
                        }
                        else {
                            SpriteArray[x, y].Colour = OutlineColour;
                        }
                    }
                }
            }
        }
        return Roll;
    }

    public Color ColourFromGradient(Gradient gradient) {
        Color colour = new Color();
        colour = gradient.Evaluate(Random.Range(0f, 1f));
        return colour;
    }

    public void FillShapeWithColour(Color colour, float OutlineValue) {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (SpriteArray[x, y].Shape == 0) {
                    continue;
                }
                else {
                    if (SpriteArray[x, y].outline == false) {
                        SpriteArray[x, y].Colour = colour;
                    }
                    else {
                        SpriteArray[x, y].Colour = colour * OutlineValue;
                    }
                }
            }
        }
    }


    public void Mirror() {
        for (int y = 0; y < Height; y++) {
            int xoffset = Width - 1;
            for (int x = 0; x < Width / 2; x++) {
                SpriteArray[x + xoffset, y].Shape = SpriteArray[x, y].Shape;
                xoffset -= 2;
            }
        }
    }

    public void MirrorY() {
        for (int x = 0; x < Width; x++) {
            int xstart = Mathf.CeilToInt(Height / 2 - 1);
            int xoffset = Height - 1;
            for (int y = 0; y < Height / 4; y++) {
                SpriteArray[x, y + xoffset].Shape = SpriteArray[x, y + xstart].Shape;
                xoffset -= 2;
            }
        }
    }

    public void MirrorBadly() {
        int roll = Random.Range(0, 2);
        for (int y = 0; y < Height; y++) {
            int xoffset = Width - 1;
            for (int x = 0; x < Width / 2; x++) {
                SpriteArray[x + xoffset, y].Shape = SpriteArray[x, y].Shape;
                xoffset -= 2;
            }
        }
        if (roll >= 1) {
            for (int y = 0; y < Height; y++) {
                float width = Width;
                int xoffset = Mathf.CeilToInt(Width / 2 - 1);
                for (int x = 0; x < Width / 2; x++) {
                    SpriteArray[x + xoffset, y].Shape = SpriteArray[x, y].Shape;
                }
            }

            for (int y = 0; y < Height; y++) {
                int xoffset = Width - 1;
                for (int x = 0; x < Width / 2; x++) {
                    SpriteArray[x, y].Shape = SpriteArray[x + xoffset, y].Shape;
                    xoffset -= 2;
                }
            }
        }
        else {
            for (int y = 0; y < Height; y++) {
                float width = Width;
                int xoffset = Mathf.CeilToInt(Width / 2 - 1);
                for (int x = 0; x < Width / 2; x++) {
                    SpriteArray[x + xoffset, y].Shape = SpriteArray[x, y].Shape;
                }
            }

            for (int y = 0; y < Height; y++) {
                int xstart = Mathf.CeilToInt(Width / 2 - 1);
                int xoffset = Width - 1;
                for (int x = 0; x < Width / 4; x++) {
                    SpriteArray[x + xoffset, y].Shape = SpriteArray[x + xstart, y].Shape;
                    xoffset -= 2;
                }
            }

            for (int y = 0; y < Height; y++) {
                int xoffset = Width - 1;
                for (int x = 0; x < Width / 2; x++) {
                    SpriteArray[x, y].Shape = SpriteArray[x + xoffset, y].Shape;
                    xoffset -= 2;
                }
            }
        }
    }

    public void GenerateOutline(int width, int height, int shape) {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (CheckOutlineNeighbors(x, y, shape) == true) {
                    SpriteArray[x, y].outline = true;
                }
            }
        }
    }

    public bool CheckOutlineNeighbors(int x, int y, int shape) {
        bool OutlineCheck = false;
        int Neighbours = 0;
        for (int neightborx = x - 1; neightborx <= x + 1; neightborx++) {
            if (neightborx >= 0 && neightborx < Width) {
                if (SpriteArray[neightborx, y].Shape == shape) {
                    Neighbours++;
                }
            }
        }
        for (int neightbory = y - 1; neightbory <= y + 1; neightbory++) {
            if (neightbory >= 0 && neightbory < Height) {
                if (SpriteArray[x, neightbory].Shape == shape) {
                    Neighbours++;
                }
            }
        }
        if (Neighbours <= 5) {
            OutlineCheck = true;
        }
        return OutlineCheck;
    }

    public void Automata(int Loops, int shape, int ComeAlive, int StayAlive, float EdgeCulling) {
        Pixel_Data[,] temp = new Pixel_Data[Width, Height];
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                temp[x, y] = new Pixel_Data(SpriteArray[x, y].Colour, SpriteArray[x, y].Shape);
            }
        }

        for (int l = 0; l < Loops; l++) {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {

                    //Automata
                    int Neighbours = ReturnNeighbors(x, y, shape);
                    if (SpriteArray[x, y].Shape == 0) {
                        if (Neighbours >= ComeAlive) {
                            temp[x, y].Shape = 1;
                        }
                    }
                    if (SpriteArray[x, y].Shape == 1) {
                        if (Neighbours >= StayAlive) {
                            temp[x, y].Shape = 1;
                        }
                        else {
                            temp[x, y].Shape = 0;
                        }
                    }
                }
            }
        }
        System.Array.Copy(temp, SpriteArray, SpriteArray.Length);
    }

    public void AutomataHighRes(int Loops, int shape, int ComeAlive, int StayAlive,int Death) {
        Pixel_Data[,] temp = new Pixel_Data[Width, Height];
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                temp[x, y] = new Pixel_Data(SpriteArray[x, y].Colour, SpriteArray[x, y].Shape);
            }
        }

        for (int l = 0; l < Loops; l++) {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    int Neighbours = ReturnNeighbors(x, y, shape);
                    if (SpriteArray[x, y].Shape == 1) {

                        if (Neighbours >= StayAlive) {
                            temp[x, y].Shape = 1;
                        }
                        else if(Neighbours < Death) {
                            temp[x, y].Shape = 0;
                        }
                    }

                    if (SpriteArray[x, y].Shape == 0) {
                        if (Neighbours >= ComeAlive) {
                            temp[x, y].Shape = 1;
                        }
                    }
                }
            }
        }
        System.Array.Copy(temp, SpriteArray, SpriteArray.Length);
    }

    public void CleanUpStrayPixels(int Agressiveness) {
        Pixel_Data[,] temp = new Pixel_Data[Width, Height];
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                temp[x, y] = new Pixel_Data(SpriteArray[x, y].Colour, SpriteArray[x, y].Shape);
            }
        }

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {

                //Automata
                int Neighbours = ReturnNeighbors(x, y, 1);
                if (SpriteArray[x, y].Shape == 1) {
                    if (Neighbours < Agressiveness) {
                        temp[x, y].Shape = 0;
                    }
                }
            }
        }
        System.Array.Copy(temp, SpriteArray, SpriteArray.Length);
    }

    public float[,] GenerateFalloffMask() {
        int size = Width;
        float[,] Mask = new float[size, size];

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                Mask[i, j] = Evaluate(value);
            }
        }
        return Mask;
    }

    float Evaluate(float value) {
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }

    public void ApplyMaskToArray(float[,] Mask, float MaskStrenght) {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                float Roll = Random.Range(0, Mask[x, y]);
                if (Roll > MaskStrenght) {
                    SpriteArray[x, y].Shape = 0;
                }
                if(Mask[x,y] > 0.95f) {
                    SpriteArray[x, y].Shape = 0;
                }
            }
        }
    }

    public void FillInMask(float[,] Mask,float Strength) {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                float Roll = Random.Range(0, Mask[x, y]);
                if (Mask[x, y] < Strength) {
                    SpriteArray[x, y].Shape = 1;
                }
            }
        }
    }

    public float[,] CreateCustomMask(Sprite MaskImage) {
        float SpriteWidthFloat = MaskImage.rect.width;
        float SpriteHeightFloat = MaskImage.rect.height;
        float[,] Mask = new float[Width, Height];
        for (float x = 0; x < Width; x++) {
            for (float y = 0; y < Height; y++) {
                int DotProductX = Mathf.FloorToInt((x / Width * 1f) * SpriteWidthFloat);
                int DotProducty = Mathf.FloorToInt((y / Height * 1f) * SpriteHeightFloat);
                Color MaskColour = MaskImage.texture.GetPixel(DotProductX, DotProducty);
                Mask[Mathf.FloorToInt(x), Mathf.FloorToInt(y)] = MaskColour.r;
            }
        }
        return Mask;
    }

    public float[,] InvertMask(float[,] InputMask,int InputWidth,int InputHeight) {
        float[,] InvertedMask = new float[InputWidth, InputHeight];
        for (int x = 0; x < InputWidth; x++) {
            for (int y = 0; y < InputHeight; y++) {
                InvertedMask[x, y] = 1f - InputMask[x,y];
            }
        }
        return InvertedMask;
    }

    public void AddLeftShadow(float ShadowStrength) {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if(x < Width / 2) {
                    SpriteArray[x, y].Colour.r *= ShadowStrength;
                    SpriteArray[x, y].Colour.b *= ShadowStrength;
                    SpriteArray[x, y].Colour.g *= ShadowStrength;
                }
            }
        }
    }

    public void AddCenterLine(float Strength) {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                float WidthTemp = Width;
                if (x == Mathf.RoundToInt(WidthTemp/2)) {
                    SpriteArray[x, y].Colour.r *= Strength;
                    SpriteArray[x, y].Colour.b *= Strength;
                    SpriteArray[x, y].Colour.g *= Strength;
                }
            }
        }
    }

    public int CountCells() {
        int count = 0;
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (SpriteArray[x,y].Shape == 1) {
                    count++;
                }
            }
        }
        return count;
    }
}

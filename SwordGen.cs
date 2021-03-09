using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SwordGen : MonoBehaviour {
    public int width;
    public int height;
    public int GridSize;
    public float Spacing;
    public bool Mirror = true;
    public bool Perlean = false;
    [HeaderAttribute("RandomFill")]
    [Range(0, 100)]
    public float FillPercent;
    [Range(0, 100)]
    public float DecalFillPercent;
    [HeaderAttribute("PerleanFill")]
    [Range(1, 0)]
    public float Perleanfill;
    public float Scale;
    [Range(1, 0)]
    public float MaskStrength;
    [Range(1, 0)]
    public float DecalMaskStrength;
    public Color DefultColour;
    [Range(0, 1)]
    public float MaskAddBackInStrength;
    [Range(0, 1)]
    public float FillMax;
    [Range(0, 1)]
    public float FillMin;
    [Range(0, 1)]
    public float OutlineValue;
    public int CleanUp;
    public float centerlinestrength;
    public int Loops;
    public int ComeAlive;
    public int StayAlive;
    List<GameObject> GameObjectsToBeDeleted = new List<GameObject>();
    public Sprite[] BladeSpritePool;
    public Sprite[] BigBladeSpritePool;
    public Sprite[] FillInBladeSpritePool;
    public Sprite[] BigFillInBladeSpritePool;
    public Sprite[] AsymmetricBladeSpritePool;
    public Sprite[] HiltSpritePool;
    public Sprite[] HiltSpritePool2;
    public Sprite[] DecalSpritePool;
    public Gradient[] BladeColourRange;
    public Gradient[] HiltColourRange;
    public Gradient[] DecalColourRange;
    public Color[] RarityColours;
    public GameObject RareParticles;
    public GameObject UnCommonParticles;
    int heighttemp;
    int widthtemp;
    private void Awake() {
        heighttemp = height;
        widthtemp = width;
    }

    public Color RarityRoll(int SizeRoll, int Roll, int SuperRareRoll, int GlowRoll, int ColourLerpRoll,int BrightRareRoll,int Height,int MirrorY) {
        int Rarity = 0;
        Color RarityColour = RarityColours[0];
        //RNG Text Colour
        if (SizeRoll == 0) {
            Rarity++;
        }
        if (Roll == 0) {
            Rarity += 2;
        }
        if (SuperRareRoll == 0) {
            Rarity += 3;
        }
        if (GlowRoll == 0) {
            Rarity += 14;
        }
        if (ColourLerpRoll == 0) {
            Rarity += 5;

            if (BrightRareRoll == 0) {
                Rarity += 5;
            }
        }
        if(MirrorY == 0) {
            Rarity += 2;
        }
        if(Height > 90) {
            Rarity += 3;
        }

        if(Rarity >= 5 && Rarity < 9) {
            RarityColour = RarityColours[1];
        }
        if (Rarity >= 9 && Rarity < 20) {
            RarityColour = RarityColours[2];
        }
        if (Rarity >= 20 && Rarity < 24) {
            RarityColour = RarityColours[3];
        }
        if (Rarity >= 24) {
            RarityColour = RarityColours[4];
        }
        return RarityColour;
    }

    public void GenerateSwordWithRandomPresets() {
        for (int l = 0; l < GameObjectsToBeDeleted.Count; l++) {
            Destroy(GameObjectsToBeDeleted[l]);
        }
        GameObjectsToBeDeleted.Clear();
        for (int x = 0; x < GridSize; x++) {
            for (int y = 0; y < GridSize; y++) {
                height = Random.Range(57, 120);
                width = Random.Range(23, 32);
                //Sword
                SpriteGenerator.I.NewSpriteArray(width, height, DefultColour);
                int PerleanRandom = Random.Range(0, 2);
                if (PerleanRandom >= 1) {
                    SpriteGenerator.I.RandomFill(FillPercent + Random.Range(-5, 15));
                }
                else {
                    SpriteGenerator.I.PerleanNoiseTiles(Random.Range(10,25), Random.Range(-1000, 1000), Perleanfill);
                }
                float[,] Mask = SpriteGenerator.I.CreateCustomMask(BladeSpritePool[Random.Range(0, BladeSpritePool.Length)]);
                SpriteGenerator.I.ApplyMaskToArray(Mask, MaskStrength);
                int AutomataRoll = Random.Range(0, 3);
                if (AutomataRoll >= 1) {
                    SpriteGenerator.I.Automata(Random.Range(2, 5), 1, Random.Range(6, 7), Random.Range(4, 6), 0);
                }
                else {
                    SpriteGenerator.I.AutomataHighRes(4, 1, 5, 4, 2);
                }
                SpriteGenerator.I.CleanUpStrayPixels(CleanUp);
                if(SpriteGenerator.I.CountCells() < 5) {
                    if(y > 0) {
                        y--;
                        continue;
                    }
                    else {
                        x--;
                        y = GridSize;
                        continue;
                    }
                }
                int SizeRoll = Random.Range(0, 10);
                int Roll = Random.Range(0, 20);
                int SuperRareRoll = Random.Range(0, AsymmetricBladeSpritePool.Length);
                int GlowRoll = 4;
                int ColourLerpRoll = Random.Range(0, 15);
                int DoubleBladeRoll = Random.Range(0, 150);
                int MirrorY = Random.Range(0, 30);
                int BrightRareRoll = Random.Range(0, 10);
                if (Roll >= 1) {
                    if (SizeRoll == 0) {
                        if(AutomataRoll >= 1) {
                            float[,] FillMask = SpriteGenerator.I.CreateCustomMask(BigFillInBladeSpritePool[Random.Range(0, BigFillInBladeSpritePool.Length)]);
                            SpriteGenerator.I.FillInMask(FillMask, MaskAddBackInStrength);
                        }
                        else {
                            float[,] FillMask = SpriteGenerator.I.CreateCustomMask(FillInBladeSpritePool[Random.Range(0, FillInBladeSpritePool.Length)]);
                            SpriteGenerator.I.FillInMask(FillMask, MaskAddBackInStrength);
                        }


                        if (DoubleBladeRoll >= 1) {
                            if (MirrorY >= 1) {
                                SpriteGenerator.I.Mirror();
                            }
                            else {
                                SpriteGenerator.I.MirrorY();
                            }
                        }
                        else {
                            float[,] FillMask2 = SpriteGenerator.I.CreateCustomMask(BigFillInBladeSpritePool[Random.Range(0, BigFillInBladeSpritePool.Length)]);
                            SpriteGenerator.I.FillInMask(FillMask2, MaskAddBackInStrength);
                            FillMask2 = SpriteGenerator.I.CreateCustomMask(BigFillInBladeSpritePool[Random.Range(0, BigFillInBladeSpritePool.Length)]);
                            SpriteGenerator.I.FillInMask(FillMask2, MaskAddBackInStrength);
                            SpriteGenerator.I.MirrorBadly();
                            int superroll = Random.Range(0, 50);
                            if(superroll == 0) {
                                float[,] FillMask = SpriteGenerator.I.CreateCustomMask(AsymmetricBladeSpritePool[Random.Range(0, AsymmetricBladeSpritePool.Length)]);
                                SpriteGenerator.I.FillInMask(FillMask, MaskAddBackInStrength);
                            }
                            ColourLerpRoll = 0;
                        }
                    }
                    else {
                        float[,] FillMask = SpriteGenerator.I.CreateCustomMask(FillInBladeSpritePool[Random.Range(0, FillInBladeSpritePool.Length)]);
                        SpriteGenerator.I.FillInMask(FillMask, MaskAddBackInStrength);
                        if(MirrorY >= 1) {
                            SpriteGenerator.I.Mirror();
                        }
                        else {
                            SpriteGenerator.I.MirrorY();
                            int SecondMirrorYRoll = Random.Range(0, 5);
                            if (SecondMirrorYRoll == 0) {
                                Mask = SpriteGenerator.I.CreateCustomMask(BladeSpritePool[Random.Range(0, BladeSpritePool.Length)]);
                                SpriteGenerator.I.ApplyMaskToArray(Mask, MaskStrength);
                            }
                        }
                    }
                }
                else {
                    SpriteGenerator.I.Mirror();
                    float[,] FillMask = SpriteGenerator.I.CreateCustomMask(AsymmetricBladeSpritePool[Random.Range(0, AsymmetricBladeSpritePool.Length)]);
                    SpriteGenerator.I.FillInMask(FillMask, MaskAddBackInStrength);
                    if (SuperRareRoll == 0) {
                        if (MirrorY >= 1) {
                            SpriteGenerator.I.Mirror();
                        }
                        else {
                            int SecondMirrorYRoll = Random.Range(0, 5);
                            if (SecondMirrorYRoll == 0) {
                                Mask = SpriteGenerator.I.CreateCustomMask(BladeSpritePool[Random.Range(0, BladeSpritePool.Length)]);
                                SpriteGenerator.I.ApplyMaskToArray(Mask, MaskStrength);
                            }
                        }
                    }
                }
                SpriteGenerator.I.GenerateOutline(width, height, 1);
                if (ColourLerpRoll >= 1) {
                    SpriteGenerator.I.FillShapesFromGradient(BladeColourRange[Random.Range(0, BladeColourRange.Length)], OutlineValue, false, false);
                }
                else {
                    if(BrightRareRoll == 0) {
                        //fill here
                        GlowRoll = SpriteGenerator.I.FillShapesFromGradient(DecalColourRange[Random.Range(0, DecalColourRange.Length)], OutlineValue, true, true);
                    }
                    else {
                        GlowRoll = SpriteGenerator.I.FillShapesFromGradient(BladeColourRange[Random.Range(0, BladeColourRange.Length)], OutlineValue, true, true);
                    }
                }

                if (DoubleBladeRoll == 0) {
                    GlowRoll = SpriteGenerator.I.FillShapesFromGradient(BladeColourRange[Random.Range(0, BladeColourRange.Length)], OutlineValue, true, true);
                }
                SpriteGenerator.I.AddLeftShadow(0.8f);
                if (Roll >= 15) {

                    SpriteGenerator.I.AddCenterLine(Random.Range(1f, 1.27f));
                }
                Texture2D texture = SpriteHandler.I.CreateTexture(width, height, false, 0, 0);
                GameObject NewSword = new GameObject();
                SpriteHandler.I.AddSpriteToGameObject(width, height, texture, NewSword);
                NewSword.transform.SetParent(transform);
                NewSword.transform.position = new Vector3(x * Spacing, y * Spacing, 0);
                GameObjectsToBeDeleted.Add(NewSword);

                //Hilt
                SpriteGenerator.I.NewSpriteArray(width, height, DefultColour);
                if (Perlean == false) {
                    SpriteGenerator.I.RandomFill(FillPercent + Random.Range(0, 5));
                }
                else {
                    SpriteGenerator.I.PerleanNoiseTiles(Scale, Random.Range(-1000, 1000), Perleanfill);
                }
                float[,] Mask2 = SpriteGenerator.I.CreateCustomMask(HiltSpritePool[Random.Range(0, HiltSpritePool.Length)]);
                SpriteGenerator.I.ApplyMaskToArray(Mask2, MaskStrength);
                AutomataRoll = Random.Range(0, 3);
                if(AutomataRoll >= 1) {
                    SpriteGenerator.I.Automata(4, 1, Random.Range(5, 7), 5, 0);
                }
                else {
                    SpriteGenerator.I.AutomataHighRes(4, 1, 5, 4, 2);
                }

                SpriteGenerator.I.CleanUpStrayPixels(CleanUp);
                Mask2 = SpriteGenerator.I.CreateCustomMask(HiltSpritePool[Random.Range(0, HiltSpritePool.Length)]);
                SpriteGenerator.I.FillInMask(Mask2, MaskAddBackInStrength);
                SpriteGenerator.I.Mirror();
                SpriteGenerator.I.GenerateOutline(width, height, 1);
                int roll2 = Random.Range(0, 20);
                if (roll2 >= 1) {
                    SpriteGenerator.I.FillShapesFromGradient(HiltColourRange[Random.Range(0, HiltColourRange.Length)], OutlineValue, false, false);
                }
                else {
                    SpriteGenerator.I.FillShapesFromGradient(HiltColourRange[Random.Range(0, HiltColourRange.Length)], OutlineValue, true, false);
                }
                SpriteGenerator.I.AddLeftShadow(0.8f);
                Texture2D texture2 = SpriteHandler.I.CreateTexture(width, height, false, 0, 0);
                SpriteHandler.I.OverWriteSprite(width, height, NewSword);

                //Decal
                SpriteGenerator.I.NewSpriteArray(width, height, DefultColour);
                SpriteGenerator.I.RandomFill(DecalFillPercent + Random.Range(0, 5));
                int roll = Random.Range(0, 3);
                if (roll == 0) {
                    float[,] Mask3 = SpriteGenerator.I.CreateCustomMask(DecalSpritePool[Random.Range(0, DecalSpritePool.Length)]);
                    SpriteGenerator.I.ApplyMaskToArray(Mask3, DecalMaskStrength);
                }
                else {
                    float[,] Mask3 = SpriteGenerator.I.CreateCustomMask(HiltSpritePool2[Random.Range(0, DecalSpritePool.Length)]);
                    SpriteGenerator.I.ApplyMaskToArray(Mask3, DecalMaskStrength);
                }
                SpriteGenerator.I.Automata(5, 1, 6, 6, 0);
                SpriteGenerator.I.CleanUpStrayPixels(CleanUp);
                SpriteGenerator.I.Mirror();
                SpriteGenerator.I.GenerateOutline(width, height, 1);
                int roll3 = Random.Range(0, 20);
                if (roll3 >= 1) {
                    SpriteGenerator.I.FillShapesFromGradient(DecalColourRange[Random.Range(0, DecalColourRange.Length)], OutlineValue, false,false);
                }
                else {
                    SpriteGenerator.I.FillShapesFromGradient(DecalColourRange[Random.Range(0, DecalColourRange.Length)], OutlineValue, true, false);
                }
                SpriteGenerator.I.AddLeftShadow(0.8f);
                Texture2D texture3 = SpriteHandler.I.CreateTexture(width, height, false, 0, 0);
                SpriteHandler.I.OverWriteSprite(width, height, NewSword);

                NewSword.transform.name = (WordGenerator.Instance.Adjective() + " " + WordGenerator.Instance.Sword());
                Color RarityColour = RarityRoll(SizeRoll, Roll, SuperRareRoll, GlowRoll, ColourLerpRoll,BrightRareRoll,height, MirrorY);
                GameObject text = SpriteHandler.I.CreateText(NewSword, -0.65f);
                text.GetComponent<Text>().color = RarityColour;
                if (RarityColour == RarityColours[2]) {
                    GameObject ParticleEffect = Instantiate(UnCommonParticles, NewSword.transform.position, Quaternion.identity);
                    GameObjectsToBeDeleted.Add(ParticleEffect);
                }
                if (RarityColour == RarityColours[3] || RarityColour == RarityColours[4]) {
                    GameObject ParticleEffect = Instantiate(RareParticles, NewSword.transform.position, Quaternion.identity);
                    GameObjectsToBeDeleted.Add(ParticleEffect);
                }
                SwordPhysics SP = NewSword.AddComponent<SwordPhysics>();
                SP.Set();
                GameObjectsToBeDeleted.Add(text);
            }
        }
    }

    public void SetCameraToRightPosition() {
        float position = (GridSize -1 * Spacing)/2;
        Camera.main.transform.position = new Vector3(position,position,-10);
    }

    public void SizeSlider(float size) {
        int Size = Mathf.FloorToInt(size);
        width = Size;
        height = Size;
    }
    public void MaskSlider(float Mask) {
        MaskStrength = Mask;
    }
    public void Fill(float fill) {
        FillPercent = fill;
    }

    public void PerleanToggle(bool toggle) {
        Perlean = toggle;
    }
}

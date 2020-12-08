using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

[System.Serializable]
public class Element
{
    /*
     * Fire: WK: Water STR: Nature
     * Water: WK: Electric STR: Fire
     * Nature: WK: Fire STR: Electric
     * Electric: WK: Nature STR: Water
     * Dark: WK: Light STR: Dark
     * Light: WK: Dark STR: Light
     */

    private static int FIRE = 0;
    private static int WATER = 1;
    private static int NATURE = 2;
    private static int ELECTRIC = 3;
    private static int DARK = 4;
    private static int LIGHT = 5;
    private static int NULL = 6;

    public int OwnElement, ResistantElement, WeakElement;

    public Element(int index)
    {
        switch (index)
        {
            case 0:
                OwnElement = FIRE;
                break;
            case 1:
                OwnElement = WATER;
                break;
            case 2:
                OwnElement = NATURE;
                break;
            case 3:
                OwnElement = ELECTRIC;
                break;
            case 4:
                OwnElement = DARK;
                break;
            case 5:
                OwnElement = LIGHT;
                break;
            default:
                OwnElement = NULL;
                break;
        }

        SetElement(index);
    }

    public void SetElement(int elem)
    {
        ResistantElement = Resistance(elem);
        WeakElement = Weakness(elem);
    }

    public int Weakness(int elem) {
        int wk;
        switch (elem)
        {
            case 0:
                wk = WATER;
                break;
            case 1:
                wk = ELECTRIC;
                break;
            case 2:
                wk = FIRE;
                break;
            case 3:
                wk = NATURE;
                break;
            case 4:
                wk = LIGHT;
                break;
            case 5:
                wk = DARK;
                break;
            default: 
                wk = NULL;
                break;
        }
        return wk;
    }

    public int Resistance(int elem)
    {
        int wk;
        switch (elem)
        {
            case 0:
                wk = NATURE;
                break;
            case 1:
                wk = FIRE;
                break;
            case 2:
                wk = ELECTRIC;
                break;
            case 3:
                wk = WATER;
                break;
            case 4:
                wk = NULL;
                break;
            case 5:
                wk = NULL;
                break;
            default:
                wk = NULL;
                break;
        }
        return wk;
    }

    /*public int returnElement(int n)
    {
        int thisElement;

        if (n >= 0 && n <= 22) n = 0;
        else if (n >= 23 && n <= 45) n = 1;
        else if (n >= 46 && n <= 68) n = 2;
        else if (n >= 69 && n <= 90) n = 3;
        else if (n >= 91 && n <= 95) n = 4;
        else if (n >= 96 && n <= 100) n = 5;

        switch (n)
        {
            case 0:
                thisElement = FIRE;
                break;
            case 1:
                thisElement = WATER;
                break;
            case 2:
                thisElement = NATURE;
                break;
            case 3:
                thisElement = ELECTRIC;
                break;
            case 4:
                thisElement = DARK;
                break;
            case 5:
                thisElement = LIGHT;
                break;
            default:
                thisElement = NULL;
                break;
        }

        return thisElement;
    }*/
}

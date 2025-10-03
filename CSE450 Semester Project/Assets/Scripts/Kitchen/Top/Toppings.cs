using System.Linq;
using UnityEngine;

public enum Topping
{
    RedSauce, OliveOil, // bases
    Mozzarella, Fontina, Spinach, // secondary bases

    Sausage, Pepperoni, Bacon, // meats
    Mushroom, GreenPepper, WhiteOnion, // veggies
    BlackOlive, BananaPepper, RedOnion,
    Gorgonzola, Feta, Parmesan // extra cheeses
}

static class ToppingMethods {
    private static Topping[] bases = new Topping[] { Topping.RedSauce, Topping.OliveOil };
    private static Topping[] secondaryBases = new Topping[] { Topping.Mozzarella, Topping.Fontina, Topping.Spinach };
    
    public static Topping[] GetBases() { return bases; }
    public static Topping[] GetSecondaryBases() { return secondaryBases; }
    
    public static Topping GetRandomBase() {
        return bases[Random.Range(0, bases.Length)];
    }
    public static Topping GetRandomSecondaryBase() {
        return secondaryBases[Random.Range(0, secondaryBases.Length)];
    }
    public static Topping GetRandomNonbaseTopping() {
        var ts = System.Enum.GetValues(typeof(Topping)).Cast<Topping>().ToList();
        foreach(Topping b in bases) { ts.Remove(b); }
        foreach(Topping b in secondaryBases) { ts.Remove(b);  }
        return ts[Random.Range(0, ts.Count)];
    }
    
    // basically just adding spaces...
    public static string ToString(Topping t) {
        switch (t) {
            case Topping.RedSauce: return "Red Sauce";
            case Topping.OliveOil: return "Olive Oil";
            case Topping.GreenPepper: return "Green Pepper";
            case Topping.WhiteOnion: return "White Onion";
            case Topping.BlackOlive: return "Black Olive";
            case Topping.BananaPepper: return "Banana Pepper";
            case Topping.RedOnion: return "Red Onion";
            default: return t.ToString();
        }
    }
}
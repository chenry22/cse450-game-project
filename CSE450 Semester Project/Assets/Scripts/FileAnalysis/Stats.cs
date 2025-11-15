using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace FileAnalysis
{
    /*
        DoughHandling:  Integer | 0-100 | Handling & preparing dough
        Toppings:       Integer | 0-100 | Adding toppings
        Cooking:        Integer | 0-100 | Cooking pizzas
        Cutting:        Integer | 0-100 | Cutting pizzas
        Speed:          Integer | 0-100 | Speed in performing tasks
        Stamina:        Integer | 0-100 | Stamina
    */
    [System.Serializable]
    public class Stats
    {
        // Kitchen Skills
        public int DoughHandling = 50;
        public int Toppings = 50;
        public int Cooking = 50;
        public int Cutting = 50;

        // Character Skills
        public int Speed = 50;
        public int Stamina = 50;

        // Behavioral Attributes (hidden)
        public int Morality = 50;
        public int Impulsiveness = 50;
        public int Extroversion = 50;
        public int Impressionability = 50;

        public Stats() {} // default constructor leaves default

        // a bit more interesting constructor using the file hash to compute stats
        // still deterministic like we want, but makes files we haven't explicitly created algorithms for a bit more variable
        public Stats(string fileName) {
            byte[] hash = GetMD5HashFromFile(fileName);
            if (BitConverter.IsLittleEndian){
                Array.Reverse(hash);
            }
            int val = Mathf.Abs(BitConverter.ToInt32(hash, 0));
            DoughHandling = val % 100;
            Toppings = val / 100 % 100;
            Cooking = val / 10000 % 100;
            Cutting = val / 1000000 % 100;

            Morality = val / 10 % 100;
            Impulsiveness = val / 1000 % 100;
            Extroversion = val / 100000 % 100;
            Impressionability = val / 10000000 % 100;
        }
        
        // taken basically straight from
        // https://stackoverflow.com/questions/16318087/calculate-the-hash-of-the-contents-of-a-file-in-c
        protected byte[] GetMD5HashFromFile(string fileName) {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(file);
            file.Close();
            return hash;
        }
    }
}
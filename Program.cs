using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Password_Cracking
{
    class Program
    {
        static int uyumluluk(String parola, String test_kelimesi)
        {
            int puan = 0;
            if (parola.Length != test_kelimesi.Length)
            {
                Console.WriteLine("Kelime uzunlukları uyuşmuyor");
                return puan;
            }
            else
            {
                for (int i = 0; i < parola.Length; i++)
                {
                    if (parola[i] == test_kelimesi[i])
                    {
                        puan++;
                    }
                }
                return puan * 100 / parola.Length;
            }
        }
        private static Random random = new Random((int)DateTime.Now.Ticks);
        public static String Yeni_Kelime(int uzunluk)
        {
            String kelime = "";
            String harfler = "ABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZabcçdefgğhıijklmnoöprsştuüvyz0123456789 ";
            for (int i = 0; i < uzunluk; i++)
            {
                kelime += harfler[random.Next(harfler.Length)];
            }
            return kelime;
        }

        public static List<String> Ilk_Nesli_Al(int Nesil_Buyuklugu, int uzunluk)
        {
            List<String> Nesil = new List<String>();
            for (int i = 0; i < Nesil_Buyuklugu; i++)
            {
                Nesil.Add(Yeni_Kelime(uzunluk));
            }
            return Nesil;
        }

        static Dictionary<string, int> Nesil_Uyumlulugu(List<String> Nesil, String parola)
        {
            Dictionary<string, int> Nesil_Uyumu = new Dictionary<string, int>();
            foreach (String element in Nesil)
            {
                if (Nesil_Uyumu.ContainsKey(element))
                {
                    continue;
                }
                else
                {
                    Nesil_Uyumu.Add(element, uyumluluk(parola, element));
                }
            }
            Nesil_Uyumu = Nesil_Uyumu.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            return Nesil_Uyumu;
        }
        //Sıkıntılı
        //Dönülecek
        static List<String> Jenerasyon_Al(Dictionary<string, int> SortedNesil, int en_iyi, int random_eş, int Nesil_Buyuklugu)
        {
            List<String> gelecek_nesil = new List<String>();
            for (int i = 0; i < en_iyi; i++)
            {
                if (i >= SortedNesil.Count)
                {
                    gelecek_nesil.Add(SortedNesil.ElementAt(random.Next(SortedNesil.Count)).Key);
                }
                else
                {
                    gelecek_nesil.Add(SortedNesil.ElementAt(i).Key);
                }
            }
            for (int i = en_iyi; i < random_eş; i++)
            {
                gelecek_nesil[i] = SortedNesil.ElementAt(random.Next(SortedNesil.Count)).Key;
            }
            gelecek_nesil = gelecek_nesil.OrderBy(x => random.Next()).ToList();
            return gelecek_nesil;
        }

        static String Yavru_Olustur(String ebeveyn1, String ebeveyn2)
        {
            String yavru = "";
            for (int i = 0; i < ebeveyn1.Length; i++)
            {
                if (random.NextDouble() > 0.5)
                {
                    yavru += ebeveyn1[i];
                }
                else
                {
                    yavru += ebeveyn2[i];
                }
            }
            return yavru;
        }
        static List<String> Yavrular_Olustur(List<String> ebeveynler)
        {
            List<String> Sonraki_Popülasyon = new List<String>();
            for (int i = 0; i < ebeveynler.Count / 2; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Sonraki_Popülasyon.Add(Yavru_Olustur(ebeveynler[i], ebeveynler[ebeveynler.Count - 1 - i]));
                }
            }
            return Sonraki_Popülasyon;
        }

        static List<String> Mutasyon(List<String> Nesil, int şans)
        {
            for (int i = 0; i < Nesil.Count; i++)
            {
                if (random.NextDouble() * 100 < şans)
                {
                    int k = random.Next(0, Nesil[0].Length - 1);
                    String kelime = "";
                    if (k != 0)
                    {
                        for (int j = 0; j < k; j++)
                        {
                            kelime += Nesil[i][j];
                        }
                    }
                }
            }
            return Nesil;
        }
        public static void Main(string[] args)
        {
            String parola = "Deep Learning 2022";
            int Nesil_Buyuklugu = 1000;
            int en_iyi_adaylar = 250;
            int sanslı_adaylar = 250;
            int mutasyon_sansı = 10;
            int max_gen = Int32.MaxValue;
            List<String> sonraki_populasyon = new List<String>();
            Dictionary<string, int> sorted_nesil = new Dictionary<string, int>();
            Stopwatch saat = new Stopwatch();
            saat.Start();
            List<String> Nesil = Ilk_Nesli_Al(Nesil_Buyuklugu, parola.Length);
            for (int i = 0; i < max_gen; i++)
            {
                sorted_nesil = Nesil_Uyumlulugu(Nesil, parola);
                Console.WriteLine("Nesil: " + (i + 1) + " Nesildeki en iyi Uyumluluk:" + sorted_nesil.ElementAt(0).Value + " Nesildeki En İyi Tahmin: " + sorted_nesil.ElementAt(0).Key);
                if (sorted_nesil.ElementAt(0).Value == 100)
                {
                    saat.Stop();
                    break;
                }
                List<String> bir_sonraki_ebeveynler = Jenerasyon_Al(sorted_nesil, en_iyi_adaylar, sanslı_adaylar, Nesil_Buyuklugu);
                sonraki_populasyon = Yavrular_Olustur(bir_sonraki_ebeveynler);
                sonraki_populasyon = Mutasyon(sonraki_populasyon, mutasyon_sansı);
                Nesil = sonraki_populasyon;
            }
            Console.WriteLine("Parola:" + sorted_nesil.ElementAt(0).Key + " Bulma Süresi: " + saat.ElapsedMilliseconds + " Milisaniye");
            Console.ReadKey();
        }
    }
}
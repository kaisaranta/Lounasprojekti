﻿// See https://aka.ms/new-console-template for more information
using ConsoleTools;
using Lounasprojekti.Models;
using System.Text;

static class TietojenNäyttäminen
{
    public static string RavintolaNimi { get; set; }
    public static int RavintolaID { get; set; }

    static LounasDBContext db = new LounasDBContext();

    public static void NäytäRavintolanTiedot(List<string> ravintolanTiedot)
    {
        PäivitäRavintolaId(ravintolanTiedot[0]);
        RavintolaNimi = ravintolanTiedot[0];
        foreach (var item in ravintolanTiedot)
            Console.WriteLine(item);
        Console.ReadLine();
    }


    public static void NäytäRavintolanTiedot(List<string> ravintolanTiedot, ConsoleMenu con)
    {
        PäivitäRavintolaId(ravintolanTiedot[0]);
        RavintolaNimi = ravintolanTiedot[0];
        Console.Clear();
        foreach (var item in ravintolanTiedot)
            Console.WriteLine(item);
        Console.ReadLine();
        con.Show();
    }

    public static void PäivitäRavintolaId(string ravintolanNimi)
    {
        var kysely = (from i in db.Ravintolas
                      where i.RavintolanNimi == ravintolanNimi
                      select i.RavintolaId).First();

        RavintolaID = kysely;

    }

    public static void NäytäRavintolanRuokailijat(int RavintolaID)
    {
        Console.Clear();
        Kyselyt kysely = new Kyselyt();
        var Ruokailijat = kysely.HaeRuokailijatTänään(RavintolaID);
        if (Ruokailijat.Count == 0)
            Console.WriteLine("Ei ruokailijoita");
        else
            foreach (var ruokailija in Ruokailijat)
            {
                Console.WriteLine(ruokailija);
            }
        Console.ReadLine();
    }

    public static List<Tuple<string, Action>> NäytäKommentitValikko( ConsoleMenu con)
    {

        List<Tuple<string, Action>> map = new List<Tuple<string, Action>>();
        var kysely = from i in db.Arvios
                     join i2 in db.Käyttäjäs on i.KäyttäjäId equals i2.KäyttäjäId
                     orderby i.Päivämäärä descending
                     select new { käyttäjänimi = i2.Käyttäjänimi, kommentti = i.Kommentti, päivämäärä = i.Päivämäärä };

        foreach (var item in kysely)
        {
            var valikkoNimi = $"{item.käyttäjänimi.PadRight(20)}{item.päivämäärä.ToString().PadRight(30)}{item.kommentti}";
            map.Add(Tuple.Create<string, Action>(valikkoNimi, () => con.Show()));

        }

        return map;
    }

    public static void NäytäRavintolanArvostelut(int ravintolaId)
    {
        var kommenttiLista = new List<string>();
        kommenttiLista.Add("*************************ARVOSTELUT*************************");

        var kysely = (from i in db.Ravintolas
                      where i.RavintolaId == ravintolaId
                      join i2 in db.Arvios on i.RavintolaId equals i2.RavintolaId
                      join i3 in db.Käyttäjäs on i2.KäyttäjäId equals i3.KäyttäjäId
                      orderby i2.Päivämäärä descending
                      select new
                      {
                          Pvm = i2.Päivämäärä,
                          Arvosana = i2.Arvosana,
                          Kommentti = i2.Kommentti,
                          Nimi = i3.Käyttäjänimi
                      });

        foreach (var item in kysely)
        {
            var kommentti = item.Kommentti;
            if (item.Kommentti.Length > 60)
            {
                kommentti = item.Kommentti.Substring(0, 60) + Environment.NewLine + item.Kommentti.Substring(60);
            }
            var sb = new StringBuilder();
            sb.AppendLine(String.Format("{0, 0}{1, 40}", item.Pvm.ToString(), item.Nimi));
            sb.Append("Arvosana: " + item.Arvosana.ToString() + "/5" + Environment.NewLine);
            sb.Append(String.Format("{0, 40}", kommentti + Environment.NewLine));
            sb.AppendLine("************************************************************");
            kommenttiLista.Add(sb.ToString());
        }
        Console.Clear();
        foreach (var item in kommenttiLista)
        {
            Console.WriteLine(item);
        }
        Console.ReadLine();
    }


}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace weka_result_split
{
    public partial class WekaSplit : Form
    {
        public WekaSplit()
        {
            InitializeComponent();
        }

        // <copyright file="WekaSplit.cs">
        // Copyright (c) 2012 All Rights Reserved
        // </copyright>
        // <author>Yakup Durmuş</author>
        // <date>18/11/2018 06:50 </date>
        // <summary>Weka Result Split - Veri madenciliği</summary>

        public static string dosyadanOku(string dosyaAdi = "1")
        {
            string dosya_yolu = Directory.GetCurrentDirectory()+"\\weka-result\\" + dosyaAdi;
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            string yazi = sw.ReadLine();
            string yaziTam = "";
            while (yazi != null)
            {
                yaziTam += yazi+"\n";
                yazi = sw.ReadLine();
            }
            sw.Close();
            fs.Close();
            return yaziTam;
        }


        List<Dictionary<string,string>> tumYazilar;
        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(Directory.GetCurrentDirectory()+"//index.html"))
            {
                File.Delete(Directory.GetCurrentDirectory() + "//index.html");
            }
            tumYazilar = new List<Dictionary<string, string>>();
            for (int k = 1; k <= 41; k++)
            {
                string yazi="";
                try
                {
                    yazi = dosyadanOku(k.ToString());
                }
                catch (Exception)
                {
                    MessageBox.Show("Dosya bulunamadı!");
                }
                
                var Parcali = yazi.Split(new string[] { "===" }, StringSplitOptions.None);
                var yaziParcali = new Dictionary<string, string>();
            
                for (int i = 0; i < Parcali.Length; i++)
                {
                    if (Parcali[i].IndexOf("Run information")>-1)
                    {
                        yaziParcali["bilgi"] = bilgi(Parcali[i + 1]);
                    }
                    else if (Parcali[i].IndexOf("Summary") > -1)
                    {
                        yaziParcali["ozet"] = ozet(Parcali[i + 1]);
                    }
                    else if (Parcali[i].IndexOf("Detailed Accuracy By Class") > -1)
                    {
                        yaziParcali["sinifDoygunluk"] = sinifDoygunluk(Parcali[i + 1]);
                    }
                    
                    else if (Parcali[i].IndexOf("Confusion Matrix") > -1)
                    {
                        yaziParcali["karisiklikMatrisi"] = karisikMatris(Parcali[i + 1]);
                    }

                }
                yaziParcali["htmlToplam"] = "<h1>" + yaziParcali["bilgi"] + "</h2>" + yaziParcali["ozet"] + "<br>" + yaziParcali["sinifDoygunluk"] + "<br>" + yaziParcali["karisiklikMatrisi"] + "<br><hr><br>";
                tumYazilar.Add(new Dictionary<string,string>(yaziParcali));

               
            }


            tumunuYazdir(tumYazilar);
            MessageBox.Show("Html dosyası .exe nin bulunduğu dizine kaydedildi.");
            System.Diagnostics.Process.Start(Directory.GetCurrentDirectory()+"//index.html");
            Application.Exit();


        }

        private void tumunuYazdir(List<Dictionary<string, string>> tumYazilar)
        {
            foreach (var yazi in tumYazilar)
            {
                string dosya_yolu = Directory.GetCurrentDirectory() + "\\index.html";
                FileStream fs = new FileStream(dosya_yolu, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(yazi["htmlToplam"]);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private static string bilgi(string veri)
        {
            return veri.Split(':')[1].Trim().Split(' ')[0];
        }
        private static List<string> Temizle(string[] parcalanmisVeri)
        {
            List<string> listVeri = new List<string>();
            for (int i = 0; i < parcalanmisVeri.Length; i++)
            {
                if (parcalanmisVeri[i].Trim() != "")
                {
                    listVeri.Add(parcalanmisVeri[i]);
                }
            }
            return listVeri;
        }
        private static string ozet(string veri)
        {
            var parcalanmisVeri = veri.Split(new string[] { "   ","\n"}, StringSplitOptions.None);
            var listVeri = Temizle(parcalanmisVeri);

            string htmlveri = "<h2>Özet</h2><table border=\"1\" style=\"border-collapse: collapse; width:100%; text-align:center;\">";
            htmlveri += "<tr>";
            htmlveri += "<td>"+listVeri[0]+"</td><td>"+listVeri[1]+"</td><td>"+listVeri[2]+"</td>";
            htmlveri += "</td>";
            for (int i = 3; i < listVeri.Count-1; i=i+2)
            {
                htmlveri += "<tr>";
                htmlveri += "<td>" + listVeri[i] + "</td><td>" + listVeri[i+1] + "</td><td></td>";
                htmlveri += "</td>";
            }
            htmlveri += "</table>";
            return htmlveri;

        }
        private static string sinifDoygunluk(string veri)
        {
            var parcalanmisVeri = veri.Split(new string[] { "  ", "\n" }, StringSplitOptions.None);
            var listVeri = Temizle(parcalanmisVeri);

            string htmlveri = "<h2>Sınıf Dougunluk</h2><table border=\"1\" style=\"border-collapse: collapse; width:100%; text-align:center; \">";
           
            for (int i = 0; i < listVeri.Count - 7; i = i + 9)
            {
                htmlveri += "<tr>";
                if (i==36)
                {
                    htmlveri += "<td>"+listVeri[i]+"</td>";
                    i++;
                }
                else
                {
                    htmlveri += "<td></td>";
                }
                htmlveri+="<td>" + listVeri[i] + "</td><td>" + listVeri[i+1] + "</td><td>" + listVeri[i+2] + "</td><td>" + listVeri[i+3] + "</td><td>" + listVeri[i+4] + "</td><td>" + listVeri[i+5] + "</td><td>" + listVeri[i+6] + "</td><td>" + listVeri[i+7] + "</td>";
                if (i!=37)
                {
                    htmlveri += "<td>" + listVeri[i+8] + "</td>";
                }
                else
                {
                    htmlveri += "<td></td>";
                }
                htmlveri += "</tr>";
            }
            htmlveri += "</table>";
            return htmlveri;

        }
        private static string karisikMatris(string veri)
        {
            var parcalanmisVeri = veri.Split(new string[] { "  ", "\n" }, StringSplitOptions.None);
            var listVeri = Temizle(parcalanmisVeri);
            for (int i = 0; i < listVeri.Count; i++)
            {
                listVeri[i] = listVeri[i].Replace("|", "").Replace("<--", "");
            }

            string htmlveri = "<h2>Katışık Matris</h2><table border=\"1\" style=\"border-collapse: collapse; width:100%; text-align:center;\">";

            for (int i = 0; i < listVeri.Count - 3; i = i + 4)
            {
                htmlveri += "<tr>";
                htmlveri += "<td>" + listVeri[i] + "</td><td>" + listVeri[i + 1] + "</td><td>" + listVeri[i + 2] + "</td><td>" + listVeri[i + 3] + "</td>";
                htmlveri += "</tr>";
            }
            htmlveri += "</table>";
            return htmlveri;
        }
       
       

    }
}

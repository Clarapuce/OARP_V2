using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using Accord.Math.Optimization;
using System.IO;

namespace OARP
{
    public partial class OARP : Form
    {
        string[] Eleves { get; set; }
        string[] Projets { get; set; }
        string[] ProjetsD { get; set; }
        public double[][] Matrice { get; set; }
        public double[][] MatriceBase { get; set; }
        public int NbEleves { set; get; }
        public int NbProjets { set; get; }
        public int NbMax { set; get; }
        public int NbMin { set; get; }
        public int[] NbParProjet { set; get; }
        string solutions;

        public bool SolutionValidee { get; set; }
        public OARP()
        {
            InitializeComponent();
            
        }

        private void lancer_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    ExtraireCsv(lienFichier.Text);
            //    NbMax = int.Parse(maxPlace.Text);
            //    NbMin = int.Parse(minPlace.Text);
            //    NbParProjet = new int[NbProjets];
            //    Eleves = new string[NbEleves];
            //    Projets = new string[NbProjets];
            //    ProjetsD = new string[NbProjets];
            //    SolutionValidee = false;
            //}
            //catch { MessageBox.Show("Veuillez renseigner des nombres supérieurs à zéro."); }
            //if (NbMax < NbMin)
            //{
            //    MessageBox.Show("Le nombre de place minimum par projet doit être inférieur au nombre de place maximum par projet.");
            //}
            //else
            //{
            //    if (NbEleves > NbProjets * NbMax)
            //    {
            //        MessageBox.Show("Il n'y a pas assez de places pour le nombre d'élèves indiqué.");

            //    }
            //    else
            //    {
            //        if (NbEleves < NbMin)
            //        {
            //            MessageBox.Show("Il n'y a pas assez d'élèves pour remplir un projet.");
            //        }
            //        else
            //        {
            //            try
            //            {
            //                LancerRepartition();

            //            }
            //            catch { MessageBox.Show("Il y a eu un problème dans la répartition. Veuillez vérifier que le fichier .csv est correct et qu'il est cohérent avec vos renseignements."); }

            //        }

            //    }
            //}

            LancerRepartition();


        }

        private void parcourir_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "csv files (*.csv)|*.csv";
            ofd.FilterIndex = 1;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lienFichier.Text = ofd.FileName;
            }

        }

        private void LancerRepartition()
        {
            //Initialisation des variables
            NbMax = int.Parse(maxPlace.Text);
            NbMin = int.Parse(minPlace.Text);
            Eleves = new string[NbEleves];
            Projets = new string[NbProjets];
            ProjetsD = new string[NbProjets];
            SolutionValidee = false;
            DateTime start = DateTime.Now;
            solutions = "";
            int[,] matrice = ExtraireCsv(lienFichier.Text);
            NbEleves = Eleves.Count();
            NbProjets = Projets.Count();
            NbParProjet = new int[NbProjets];

            //Creation de la matrice
            CreerMatrice(matrice);
            RetirerZero(matrice);
            MatriceBase = Conversion(matrice);
            bool suppression = true;
            int i = 1;

            Matrice = (double[][])MatriceBase.Clone();
            ProjetsD = (string[])Projets.Clone();
            EquilibrageProjetsMedian();
            MessageBox.Show("Equilibrage des projets : CHECK");
            while (suppression == true)
            {

                //AfficherNbParProjet();
                Matrice = DupliquerVoeux(Matrice);
                //Matrice = MatriceCarre(Matrice);
                //Repartition
                Munkres repartition;
                bool succes;
                repartition = new Munkres(Matrice);
                succes = repartition.Minimize();

                //Ecriture
                EcrireCsv(Matrice, repartition.Solution, i);


                suppression = SupprimerProjet(i);
                Matrice = (double[][])MatriceBase.Clone();
                ProjetsD = (string[])Projets.Clone();
                MessageBox.Show("Solution n°"+i+" : CHECK");
                i++;
            }
            TimeSpan dur = DateTime.Now - start;

            MessageBox.Show("La répartition a bien été effectuée ! Vous avez le choix entre " + (i - 1) + " solutions différentes.");

        }

        double[] CalculerPopularite()
        {
            double[] popularite = new double[NbProjets];
            for (int i = 0; i < NbProjets; i++)
            {
                for (int j = 0; j < NbEleves; j++)
                {
                    popularite[i] += MatriceBase[j][i];
                }
            }

            return popularite;
        }

        bool SupprimerProjet(int nbProjetsSup)
        {
            double[] notes = new double[NbProjets];
            double[] notes2 = new double[NbProjets];
            notes = CalculerPopularite();
            bool possibiliteSuppression = true;
            //On fait en sorte de pouvoir trouver le maximum avec une fonction trouvant le minimum
            for (int i = 0; i < NbProjets; i++) { notes2[i] = -notes[i]; }

            //Recherche du projet le moins côté qui ai des places attribuées
            int index = 0;
            //Retrait des projets qui sont déjà sortis de la liste
            for (int i = 0; i < NbProjets; i++) { if (NbParProjet[i] == 0) { notes2[i] = 0; } }

            //Recherche du plus petit qui n'ai pas déjà été supprimé
            for (int i = 0; i < nbProjetsSup; i++)
            {
                index = TrouverMinimum(notes2);
                notes2[index] = 0;
            }
            int indexPopularite = 0;
            int placesVacantes = 0;


            for (int i = 0; i < NbProjets; i++) { if (notes[i] < notes[index]) { placesVacantes += NbMax - NbParProjet[i]; } }
            if (placesVacantes < NbParProjet[index])
            {
                possibiliteSuppression = false;
            }
            else
            {
                //Ajouter l'ajout des personnes du projet supprimé aux projest un à un du plus plébicité au moins plébicité.

                for (int i = 0; i < NbProjets; i++)
                {
                    if (NbParProjet[index] > 0)
                    {
                        indexPopularite = TrouverMinimum(notes);
                        if (notes[indexPopularite] < notes[index])
                        {
                            notes[indexPopularite] = 100000000;
                            if (NbParProjet[indexPopularite] + NbParProjet[index] > NbMax)
                            {
                                NbParProjet[index] -= NbMax - NbParProjet[indexPopularite];
                                NbParProjet[indexPopularite] = NbMax;
                            }
                            else
                            {
                                NbParProjet[indexPopularite] += NbParProjet[index];
                                NbParProjet[index] = 0;
                            }
                        }


                    }
                    else { break; }
                }

            }

            return possibiliteSuppression;


        }

        int TrouverMinimum(double[] liste)
        {
            double min = 1000000;
            int indexMin = 0;
            for (int i = 0; i < NbProjets; i++)
            {
                if (liste[i] < min) { min = liste[i]; indexMin = i; }
            }
            return indexMin;
        }

        void EquilibrageProjetsMedian()
        {
            double[] notes = new double[NbProjets];
            notes = CalculerPopularite();
            
            int indexPopularite;
            int placeAttribue = NbEleves;
            bool placesRestantes = false;
            int classement = NbProjets;
            int projetsGardes = NbProjets;
            //On vérifie s'il y a assez pour tous les projets, sinon, on en retire.
            if (NbEleves / NbMin < NbProjets) { projetsGardes = NbEleves / NbMin; }
            //Permet de retirer les projets qui sont sous côtés, ajouter des places dans les projets surcotés etc...string affichage = "";
            
            while (!placesRestantes)
            {
                for (int i = 0; i < projetsGardes; i++)
                {
                    
                    indexPopularite = TrouverMinimum(notes);
                    notes[indexPopularite] = 1000000000;
                    NbParProjet[indexPopularite] += 1;
                    
                    placeAttribue = placeAttribue - 1;
                    if (placeAttribue == 0)
                    {
                        placesRestantes = true;
                        break;
                    }
                    
                }
                notes = CalculerPopularite();
                
            }

        }
        
        int VerifierProjetsComplets(double[] solution)
        {
            int index = 0;
            int compteur = 0;
            double[] somme = new double[NbProjets];
            int nbEleves = 0;
            //On selectionne les projets incomplets
            for (int i = 0; i < Projets.Length; i++)
            {
                index = 0;
                //Pour chaque projet, on compte le nombre d'élèves dedans
                foreach (double choix in solution)
                {
                    if ((Projets[Convert.ToInt16(choix)] == Projets[i]) && (index < Eleves.Length))
                    {
                        nbEleves++;
                    }
                    index++;

                }
                //On regarde s'il y a assez d'élèves

                if (nbEleves >= NbMin)
                {
                    somme[compteur] = 0;

                }
                else
                {
                    for (int j = 0; j < NbEleves; j++)
                    {
                        //MessageBox.Show( Projets[i] + " ---- " + Matrice[j][i]);
                        somme[compteur] += Matrice[j][i];
                    }
                }
                //affichage += Projets[i] + " : " + somme[compteur]+"\n";
                i += NbMax - 1;
                compteur++;
                nbEleves = 0;
            }
            //MessageBox.Show(affichage);
            int sMax = 0;
            int indexMax = 0;
            compteur = 0;
            //On détermine le projet le moins apprecié 
            foreach (int s in somme)
            {
                if ((s != 0) && (s > sMax)) { sMax = s; indexMax = compteur; }
                compteur++;
            }
            if (sMax != 0)
            {

                return indexMax;
            }
            else
            {
                return -1;
            }

        }
        void RetirerProjet(int indexProjet)
        {
            for (int i = 0; i < NbMax; i++)
            {
                for (int j = 0; j < NbEleves; j++)
                {
                    Matrice[j][indexProjet + i] = 90;
                }
            }
        }


        //Creation et modification de la matrice
        private double[][] Conversion(int[,] m)
        {
            double[][] r = new double[m.GetUpperBound(0) + 1][];
            for (int i = 0; i < m.GetUpperBound(0) + 1; i++)
            {
                r[i] = new double[m.GetUpperBound(1) + 1];
                for (int j = 0; j < m.GetUpperBound(1) + 1; j++)
                {
                    r[i][j] = m[i, j];
                }
            }
            return r;
        }
        public int[,] ExtraireCsv(string lien)
        ///Permet d'extraire d'un fichier csv une matrice int[,]
        {
            int i = 0;
            StreamReader reader = new StreamReader(lien);
            List<string> ligne = new List<string>(); ;

            //Récupération des projets
            ligne.Add(reader.ReadLine());
            Projets = ligne[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            //Récupération du nombre de places par projets
            //...

            //Récupération des voeux
            do
            {
                ligne.Add(reader.ReadLine());
                i++;
            }
            while (ligne[i] != null);


            int[,] matrice = new int[i-1, Projets.Count()];
            string[] voeux = new string[Projets.Count()];
            string[] eleves = new string[i-1];
            int nombre;
            for (int x = 1; x < i; x++)
            {
                voeux = ligne[x].Split(new string[] { "," }, StringSplitOptions.None);
                
                eleves[x-1] = voeux[0];
                for (int y = 1; y < Projets.Count()+1; y++)
                {
                    //MessageBox.Show(x+" "+y+" "+voeux[y]);
                    if (int.TryParse(voeux[y],out nombre))
                    {
                            
                        matrice[x-1, y-1] = nombre;
                    }
                        
                    else
                    {
                        matrice[x-1, y-1] = 0;
                    }
                }
                
                
            }
            Eleves = eleves;
            return matrice;
        }
        public void CreerMatrice(int[,] matrice)
        {
            int i = 0;
            int j = 0;
            TextFieldParser parser = new TextFieldParser(lienFichier.Text);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            bool projets = true;
            int c = 0;
            List<int> voeux = new List<int>();
            while (!parser.EndOfData)
            {

                //Processing row
                string[] fields = parser.ReadFields();

                foreach (string field in fields)
                {
                    if ((projets == true))
                    {
                        if (i != 0)
                        {
                            Projets[i - 1] = field;
                        }
                        i++;
                        if (i == NbProjets + 1) { projets = false; i = 0; }
                    }
                    else
                    {

                        //Console.WriteLine(i + "," + j);
                        if (i < NbEleves)
                        {

                            if (j == 0)
                            {
                                Eleves[c] = field; c++;
                                //Console.WriteLine(field + "=>Eleves");
                            }
                            else
                            {
                                try { matrice[i, j - 1] = int.Parse(field); }
                                catch { matrice[i, j - 1] = 0; }
                                //Console.WriteLine(field + "=>Voeux");
                            }
                            if (j == matrice.GetUpperBound(1) + 1) { j = 0; i++; }
                            else
                            {
                                j++;
                            }
                        }

                    }

                }
            }
        }
        void RetirerZero(int[,] m)
        {
            int row = m.GetUpperBound(0) + 1;
            int col = m.GetUpperBound(1) + 1;
            int index = 0;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (m[i, j] == 0)
                    {
                        m[i, j] = 90;
                    }
                }
                index = 0;
            }
        }

        double[][] DupliquerVoeux(double[][] m)
        {
            string[] projetsDupliques = new string[NbEleves];
            int index = 0;
            double[][] r = new double[NbEleves][];
            for (int i = 0; i < NbEleves; i++)
            {
                r[i] = new double[NbEleves];
                for (int j = 0; j < NbProjets; j++)
                {

                    for (int k = 0; k < NbParProjet[j]; k++)
                    {
                        if (i == 0)
                        {
                            projetsDupliques[index] = Projets[j];
                        }
                        if (m[i][j] == 0)
                        { r[i][index] = 90; }
                        else
                        { r[i][index] = m[i][j]; }
                        index = index + 1;
                    }

                }
                index = 0;
            }
            ProjetsD = projetsDupliques;
            return r;
            /*
            string[] projetsDupliques = new string[NbProjets*NbMax];
            int row = m.GetUpperBound(0) + 1;
            int col = m.GetUpperBound(1) + 1;
            int index = 0;
            int[,] r = new int[row, col * NbMax];
            for (int i = 0; i < row; i++)
            {

                for (int j = 0; j < col; j++)
                {
                    
                    for (int k = 0; k < NbMax; k++)
                    {
                        if (i == 0)
                        {
                            projetsDupliques[index] = Projets[j];
                        }   
                        if (m[i, j] == 0)
                        { r[i, index] = 90; }
                        else
                        { r[i, index] = m[i, j]; }
                        index = index + 1;
                    }

                }
                index = 0;
            }
            Projets = projetsDupliques;
            return r;
            */
        }
        double[][] MatriceCarre(double[][] m)
        {
            int taille = NbProjets * NbMax;
            double[][] r = new double[taille][];
            for (int i = 0; i < taille; i++)
            {
                r[i] = new double[taille];
                for (int j = 0; j < taille; j++)
                {
                    try { r[i][j] = m[i][j]; }
                    catch { }
                }

            }
            return r;
        }


        //Affichages
        public void AfficherTab(int[,] m)
        {
            string ligne = "";
            for (int i = 0; i < m.GetUpperBound(0) + 1; i++)
            {
                ligne += "|";
                for (int j = 0; j < m.GetUpperBound(1) + 1; j++)
                {

                    ligne += " " + m[i, j] + " ";
                    if (m[i, j] < 10)
                    {
                        ligne += " ";
                    }
                }
                ligne += "|\n";
            }
            MessageBox.Show(ligne);
        }
        public void AfficherMatrice(double[][] m)
        {
            string ligne = "";
            for (int i = 0; i < m.Length; i++)
            {
                ligne += "|";
                for (int j = 0; j < m[i].Length; j++)
                {

                    ligne += " " + m[i][j] + " ";
                    if (m[i][j] < 10)
                    {
                        ligne += " ";
                    }
                }
                ligne += "|\n";
            }
            MessageBox.Show(ligne);
        }
        public void AfficherListe(string titre, string[] liste)
        {
            string affichage = titre + "\n[";
            foreach (string item in liste)
            {
                affichage += item + " | ";
            }
            affichage += "]";
            MessageBox.Show(affichage);
        }
        public void AfficherSolution(double[] solution)
        {
            string affichage = "";
            int index = 0;
            for (int i = 0; i < Projets.Length; i++)
            {
                index = 0;
                affichage += Projets[i] + " : \n\n";
                foreach (double choix in solution)
                {
                    if (Projets[Convert.ToInt16(choix)] == Projets[i])
                    {
                        affichage += Eleves[index] + " (" + Matrice[index][Convert.ToInt16(choix)] + ")\n";
                        if (Matrice[index][Convert.ToInt16(choix)] > 10)
                        {
                            affichage += "   ";
                        }
                    }
                    index++;
                }
                affichage += "------------------------ \n";
                i += NbMax;
            }
            MessageBox.Show(affichage);
            /*
            foreach(double choix in solution)
            {
                if(index<Eleves.Length)
                {
                    affichage += Eleves[index] + " : " + Projets[Convert.ToInt16(choix)] + " (" + Matrice[index][Convert.ToInt16(choix)] + ")\n";
                    index++;
                }
            }*/
            //MessageBox.Show(affichage);
        }
        public void AfficherNbParProjet()
        {
            string affichage = "";
            for (int i = 0; i < NbProjets; i++)
            {
                affichage += Projets[i] + " : " + NbParProjet[i] + " places \n";
            }
            MessageBox.Show(affichage);
        }
        //Ecriture 
        public void EcrireCsv(double[][] m, double[] solution, int num)
        {
            string affichage = "";
            int index = 0;
            for (int i = 0; i < ProjetsD.Length; i++)
            {
                index = 0;
                affichage += ProjetsD[i] + " : \n\n";
                foreach (double choix in solution)
                {
                    if (ProjetsD[Convert.ToInt16(choix)] == ProjetsD[i])
                    {
                        affichage += Eleves[index] + "(" + m[index][Convert.ToInt16(choix)] + ")" + "\n";
                    }
                    index++;
                }
                affichage += "\n\n";
                i += NbMax;
            }
            if (Directory.Exists("../Solution") == false)
            {
                Directory.CreateDirectory("../Solution");
            }
            string nom = "Solution_" + num + ".csv";
            File.WriteAllText(Path.Combine("../Solution", nom), affichage, Encoding.UTF8);
            System.Diagnostics.Process.Start(Path.Combine("../Solution", nom));
        }

        private void boutonAide_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("instructions.txt");
        }
        
    }
}

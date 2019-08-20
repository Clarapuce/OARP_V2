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
        int[] ProjetsV { get; set; }
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
            
            //Initialisation des variables
            NbMax = int.Parse(maxPlace.Text);
            NbMin = int.Parse(minPlace.Text);
            Eleves = new string[NbEleves];
            Projets = new string[NbProjets];
            ProjetsD = new string[NbProjets];
            SolutionValidee = false;
            solutions = "";
            bool echec = false;

            //Extraction du CSV, modification de Eleves et Projets
            try
            {
                int[,] matrice = ExtraireCsv(lienFichier.Text);
            }
                
            catch
            {
                MessageBox.Show("Votre fichier csv n'a pas pu être extrait. Veuillez vérifier sa structure. Veuillez d'insérer aucun espace dans vos champs.");
                echec = true;
            }
            if(!echec)
            {
                int[,] matrice = ExtraireCsv(lienFichier.Text);
                NbEleves = Eleves.Count();
                NbProjets = Projets.Count();
                if (NbMax < NbMin)
                {
                    MessageBox.Show("Le nombre de place minimum par projet doit être inférieur au nombre de place maximum par projet.");
                }
                else
                {
                    if (NbEleves > NbProjets * NbMax)
                    {
                        MessageBox.Show("Il n'y a pas assez de places pour le nombre d'élèves indiqué.");

                    }
                    else
                    {
                        if (NbEleves < NbMin)
                        {
                            MessageBox.Show("Il n'y a pas assez d'élèves pour remplir un projet.");
                        }
                        else
                        {
                            try
                            {
                                LancerRepartition(matrice);

                            }
                            catch { MessageBox.Show("Il y a eu un problème dans la répartition. Veuillez vérifier que le fichier .csv est correct et qu'il est cohérent avec vos renseignements."); }

                        }

                    }
                }
            }

        }

        

        private void LancerRepartition(int[,] matrice)
        {
            DateTime start = DateTime.Now;
            NbParProjet = new int[NbProjets];
            //Creation de la matrice
            //CreerMatrice(matrice);
            RetirerZero(matrice);
            //AfficherMatrice(Conversion(matrice));
            MatriceBase = Conversion(matrice);
            bool suppression = true;
            bool echec = false;
            int i = 0;
            while (suppression)
            {
                //Initialisation
                NbParProjet = new int[NbProjets];
                suppression = EquilibrageProjetsMedian(i);
                //suppression = SupprimerProjet(i);
                Matrice = (double[][])MatriceBase.Clone();
                ProjetsD = (string[])Projets.Clone();
                Matrice = DupliquerVoeux(Matrice);

                //Repartition
                if (!suppression)
                {
                    if(i==1)
                    {
                        echec = true;
                        MessageBox.Show("Problème dans les paramètres de répartition. Vérifiez qu'il y a assez de place pour chaque élève, ou qu'il n'y a pas trop de projets obligatoires.");
                    }
                    break;
                }
                Munkres repartition;
                bool succes;
                repartition = new Munkres(Matrice);
                succes = repartition.Minimize();
                //AfficherSolution(repartition.Solution);
                //Ecriture
                EcrireCsv(Matrice, repartition.Solution, i);

                
                //MessageBox.Show("Solution n°"+(i+1)+" : CHECK");
                i++;
            }
            TimeSpan dur = DateTime.Now - start;
            
            MessageBox.Show("La répartition a bien été effectuée ! Vous avez le choix entre " + i + " solutions différentes.");

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

        //Répartition préalable du nombre de places dans un projet
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
        int TrouverMinimumVerouille(double[] notes, int[] projetsV)
        {
            double min = 1000000;
            int indexMin = 0;
            for (int i = 0; i < NbProjets; i++)
            {
                if ((notes[i] < min)&&(projetsV[i]==1)) { min = notes[i]; indexMin = i; }
            }
            return indexMin;
        }
        bool VerifierToutComplet(int[] liste, int max)
        {
            foreach(int i in liste)
            {
                if(i!=max)
                {
                    return false;
                }
            }
            return true;
        }
        bool EquilibrageProjetsMedian(int k)
        {

            //Variables pour prise en compte des projets ayant un nombre déjà fixé
            int nbProjetVerrouille = ProjetsV.Sum();
            bool suppression = true;
            bool complet = false;

            double[] notes = new double[NbProjets];
            notes = CalculerPopularite();
            
            int indexPopularite;
            bool placesRestantes = false;
            int projetsGardes = NbProjets;
            int placeAttribue = NbEleves;
            int nbProjetsComplets = 0;

            //Determiner le nombre de projets que l'on va garder
            if ((NbEleves) / NbMin < NbProjets) { projetsGardes = ((NbEleves) / NbMin); }
            projetsGardes -= k;
            //Permet de retirer les projets qui sont sous côtés, ajouter des places dans les projets surcotés etc...string affichage = "";
            //MessageBox.Show("Places attribuées : " + placeAttribue + "  Projets que l'ont garde : " + projetsGardes);
            //AfficherListe("Notes : ",notes.Select(x => x.ToString()).ToArray());
            //AfficherListe("Nombre par projet : ",NbParProjet.Select(x => x.ToString()).ToArray());
            while ((!placesRestantes)&&(nbProjetsComplets< projetsGardes))
            {
                for (int i = 0; i < projetsGardes; i++)
                {

                    indexPopularite = 0;
                    if (i >= projetsGardes - nbProjetVerrouille)
                    {
                        indexPopularite = TrouverMinimumVerouille(notes,ProjetsV);
                        if(notes[indexPopularite] == 1000000000)
                        {
                            indexPopularite = TrouverMinimum(notes);
                        }
                    }
                    else
                    {
                        indexPopularite = TrouverMinimum(notes);
                    }

                    //MessageBox.Show("Minimum : projet n°" + (indexPopularite + 1) + " avec " + notes[indexPopularite] + "avec i=" + i);

                    notes[indexPopularite] = 1000000000;

                    if (NbParProjet[indexPopularite]<NbMax)
                    {
                        if (placeAttribue == 0)
                        {
                            placesRestantes = true;
                            break;
                        }
                        NbParProjet[indexPopularite] += 1;
                        //MessageBox.Show("Projet n°" + (indexPopularite+1) + " : " + NbParProjet[indexPopularite]);
                        placeAttribue = placeAttribue - 1;
                        
                    }
                    else
                    {
                        //MessageBox.Show("Projet n°" + indexPopularite +" : Plus de places !");
                        nbProjetsComplets ++;
                    }

                }

                notes = CalculerPopularite();

            }
            if(placeAttribue!=0)
            {
                suppression = false;
            }
            //AfficherNbParProjet();
            return suppression;
            //Ajouter une erreur si jamais y a problème de répartition, pas asser de projets ou quoi
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
            ligne[0] = reader.ReadLine();
            string[] nbpp = new string[Projets.Count()];
            nbpp = ligne[0].Split(new string[] { "," }, StringSplitOptions.None);
           ProjetsV = new int[Projets.Count()];
            int nombre;

            for (int j = 1; j < Projets.Count() + 1; j++)
            {
                //MessageBox.Show("RAS a j=" + j+"  nbpp= "+ nbpp[j]);
                if (nbpp[j] != "")
                {
                    ProjetsV[j - 1] = 1;
                }

                

            }
            //AfficherNbParProjet();
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
                    //MessageBox.Show("RAS à x = " + x+" et y = "+y);
                }

                

            }
            //AfficherMatrice(Conversion(matrice));
           //AfficherListe("Eleves : ", eleves);
            //AfficherListe("Projets : ", Projets);
            //AfficherListe("Nombre par projet : ",ProjetsV.Select(x => x.ToString()).ToArray());
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
                        m[i, j] = 900;
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
                    if (ProjetsD[Convert.ToInt16(choix)] == Projets[i])
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
            for (int i = 0; i < NbParProjet.Count(); i++)
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
            for (int i = 0; i < Projets.Length; i++)
            {
                index = 0;
                affichage += Projets[i] + " : \n\n";
                foreach (double choix in solution)
                {
                    if (ProjetsD[Convert.ToInt16(choix)] == Projets[i])
                    {
                        affichage += Eleves[index] + "(" + m[index][Convert.ToInt16(choix)] + ")" + "\n";
                    }
                    index++;
                }
                affichage += "\n\n";
            }
            if (Directory.Exists(lienSave.Text) == false)
            {
                Directory.CreateDirectory(lienSave.Text );
            }
            string nom = "Solution_" + num + ".csv";
            File.WriteAllText(Path.Combine(lienSave.Text , nom), affichage, Encoding.UTF8);
            System.Diagnostics.Process.Start(Path.Combine(lienSave.Text , nom));
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
        private void boutonAide_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("instructions.txt");
        }
        private void parcourir_Click2(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd= new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                lienSave.Text = fbd.SelectedPath;
            }
        }
    }
}

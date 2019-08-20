namespace OARP
{
    partial class OARP
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lienFichier = new System.Windows.Forms.TextBox();
            this.parcourir = new System.Windows.Forms.Button();
            this.lancer = new System.Windows.Forms.Button();
            this.maxPlace = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.minPlace = new System.Windows.Forms.TextBox();
            this.boutonAide = new System.Windows.Forms.Button();
            this.lienSave = new System.Windows.Forms.TextBox();
            this.parcourir2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.ImageLocation = "./logo.png";
            this.pictureBox1.Location = new System.Drawing.Point(122, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(145, 145);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(50, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(292, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "Bienvenue sur OARP\r\nLe logiciel de répartition pour les  transdisciplinaires !";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 207);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(327, 39);
            this.label2.TabIndex = 2;
            this.label2.Text = "Pour débuter la répartition, veuillez charger un fichier csv\r\n(Le fichier doit co" +
    "ntenir les noms des élèves sur la première colonne,\r\net le nom des projets sur l" +
    "a première ligne)";
            // 
            // lienFichier
            // 
            this.lienFichier.Location = new System.Drawing.Point(12, 289);
            this.lienFichier.Name = "lienFichier";
            this.lienFichier.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.lienFichier.Size = new System.Drawing.Size(261, 20);
            this.lienFichier.TabIndex = 3;
            this.lienFichier.Text = "C:\\Users\\Clarapuce\\Desktop\\ENSC\\2A\\OARP_V2\\Test final - Feuille 1 (1).csv";
            // 
            // parcourir
            // 
            this.parcourir.Location = new System.Drawing.Point(301, 286);
            this.parcourir.Name = "parcourir";
            this.parcourir.Size = new System.Drawing.Size(75, 23);
            this.parcourir.TabIndex = 4;
            this.parcourir.Text = "Parcourir...";
            this.parcourir.UseVisualStyleBackColor = true;
            this.parcourir.Click += new System.EventHandler(this.parcourir_Click);
            // 
            // lancer
            // 
            this.lancer.Location = new System.Drawing.Point(138, 458);
            this.lancer.Name = "lancer";
            this.lancer.Size = new System.Drawing.Size(104, 37);
            this.lancer.TabIndex = 5;
            this.lancer.Text = "Lancer la répartition";
            this.lancer.UseVisualStyleBackColor = true;
            this.lancer.Click += new System.EventHandler(this.lancer_Click);
            // 
            // maxPlace
            // 
            this.maxPlace.Location = new System.Drawing.Point(265, 326);
            this.maxPlace.Name = "maxPlace";
            this.maxPlace.Size = new System.Drawing.Size(51, 20);
            this.maxPlace.TabIndex = 8;
            this.maxPlace.Text = "6";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 329);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(184, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Nombre maximum d\'élèves par projets";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(51, 355);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(181, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Nombre minimum d\'élèves par projets";
            // 
            // minPlace
            // 
            this.minPlace.Location = new System.Drawing.Point(265, 352);
            this.minPlace.Name = "minPlace";
            this.minPlace.Size = new System.Drawing.Size(51, 20);
            this.minPlace.TabIndex = 12;
            this.minPlace.Text = "4";
            // 
            // boutonAide
            // 
            this.boutonAide.Location = new System.Drawing.Point(305, 12);
            this.boutonAide.Name = "boutonAide";
            this.boutonAide.Size = new System.Drawing.Size(75, 23);
            this.boutonAide.TabIndex = 14;
            this.boutonAide.Text = "Instructions";
            this.boutonAide.UseVisualStyleBackColor = true;
            this.boutonAide.Click += new System.EventHandler(this.boutonAide_Click);
            // 
            // lienSave
            // 
            this.lienSave.Location = new System.Drawing.Point(12, 432);
            this.lienSave.Name = "lienSave";
            this.lienSave.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.lienSave.Size = new System.Drawing.Size(261, 20);
            this.lienSave.TabIndex = 15;
            this.lienSave.Text = "C:\\Users\\Clarapuce\\Desktop\\ENSC\\2A\\OARP_V2\\Test final - Feuille 1 (1).csv";
            // 
            // parcourir2
            // 
            this.parcourir2.Location = new System.Drawing.Point(301, 429);
            this.parcourir2.Name = "parcourir2";
            this.parcourir2.Size = new System.Drawing.Size(75, 23);
            this.parcourir2.TabIndex = 4;
            this.parcourir2.Text = "Parcourir...";
            this.parcourir2.UseVisualStyleBackColor = true;
            this.parcourir2.Click += new System.EventHandler(this.parcourir_Click2);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 270);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Lien de votre .csv  :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(135, 392);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "-------------------------------------";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 416);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(195, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Où voulez-vous enregistrer la solution ? ";
            // 
            // OARP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(392, 507);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lienSave);
            this.Controls.Add(this.boutonAide);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.minPlace);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.maxPlace);
            this.Controls.Add(this.lancer);
            this.Controls.Add(this.parcourir2);
            this.Controls.Add(this.parcourir);
            this.Controls.Add(this.lienFichier);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.MaximumSize = new System.Drawing.Size(408, 546);
            this.MinimumSize = new System.Drawing.Size(408, 546);
            this.Name = "OARP";
            this.Text = "OARP Transdisciplinaires";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox lienFichier;
        private System.Windows.Forms.Button parcourir;
        private System.Windows.Forms.Button lancer;
        private System.Windows.Forms.TextBox maxPlace;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox minPlace;
        private System.Windows.Forms.Button boutonAide;
        private System.Windows.Forms.TextBox lienSave;
        private System.Windows.Forms.Button parcourir2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
    }
}


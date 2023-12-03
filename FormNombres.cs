﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab6_Ajedrez
{
    public partial class FormNombres : Form
    {
        JuegoPpal formJuego;
        public FormNombres(JuegoPpal formJuego)
        {
            InitializeComponent();
            this.formJuego = formJuego;
        }

        /*
         * Metodo click del buttonIniciar
         * obtiene los nombres de los jugadores 1 y 2, valida si no estan vacios y que no superen los 10 caracteres
         * de ser asi asigna los nombres de los jugadores a las variables del form JurgoPpal y 
         */
        private void buttonIniciar_Click(object sender, EventArgs e)
        {
            // si alguno de los 2 textbox esta vacio da error
            if (textBoxJug1.Text.Replace(" ", String.Empty) == String.Empty || textBoxJug2.Text.Replace(" ", String.Empty) == String.Empty)
            {
                MessageBox.Show("Error: debe ingresar el nombre de los jugadores");
            }
            // si los nombres de los jugadores superan los 10 caracteres, da error para evitar problemas
            else if (textBoxJug1.Text.Length > 10 || textBoxJug2.Text.Length > 10) 
            {
                MessageBox.Show("Error: los nombres de los jugadores no pueden sobrepasar los 10 caracteres");
            }
            else
            {
                formJuego.nombreJugUno = textBoxJug1.Text;
                formJuego.nombreJugDos = textBoxJug2.Text;
                textBoxJug1.Text = string.Empty;
                textBoxJug2.Text = string.Empty;
                this.Close();
            }
        }
    }
}

﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdsbTranslatorGUI
{
    public partial class Form1 : Form
    {
        private int sbsPort;
        private int rawPort;
        private int receiveTimeout;
        private int aircraftTTL;
        private bool fixCRC;

        public Form1()
        {
            InitializeComponent();
            try
            {
                //Próbujemy wczytać dane z rejestru
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\ADSBTranslator",true);
                sbsPort = (int)key.GetValue("SBSOutput");
                rawPort = (int)key.GetValue("RAWInput");
                aircraftTTL = (int)key.GetValue("AircraftTTL");
                receiveTimeout = (int)key.GetValue("ReceiveTimeout");
                fixCRC = Convert.ToBoolean(key.GetValue("FixCRC"));
                key.Close();
            }
            catch
            {
                //Jeżeli z jakiś powodów nie jest to możliwe(brak uprawnień, klucze nie istnieją itp.) to ustawiamy domyślne wartości
                sbsPort = 30003;
                rawPort = 30001;
                receiveTimeout = 120;
                aircraftTTL = 20;
                fixCRC = false;
                MessageBox.Show("Odczytywanie z rejestru nie powiodło się. W pola zostały wpisane wartości domyślne.");
            }
            textBox1.Text = sbsPort.ToString();
            textBox2.Text = rawPort.ToString();
            textBox3.Text = receiveTimeout.ToString();
            textBox4.Text = aircraftTTL.ToString();
            
            if (fixCRC)
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();//Zamykamy bez niczego, nei ma po co pytać bo i tak nic ważnego się nei straci
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Parsujemy dane z aplikacji
                sbsPort = Int32.Parse(textBox1.Text);
                rawPort = Int32.Parse(textBox2.Text);
                receiveTimeout = Int32.Parse(textBox3.Text);
                aircraftTTL = Int32.Parse(textBox4.Text);

                if (checkBox1.Checked)
                {
                    fixCRC = true;
                }
                else
                {
                    fixCRC = false;
                }

                //Staramy się je zapisać do rejestru
                RegistryKey key;
                key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software", true);
                key = key.CreateSubKey("ADSBTranslator");
                key.SetValue("SBSOutput", sbsPort);
                key.SetValue("ReceiveTimeout", receiveTimeout); //timeout w sekundach
                key.SetValue("RAWInput", rawPort);
                key.SetValue("AircraftTTL", aircraftTTL);
                key.SetValue("FixCRC", fixCRC);
                key.Close();

                //Jezeli się udało to zamykamy aplikację
                Close();
            }
            catch
            {
                // Wyświetlamy komunikat gdyby parsowane dane nie miały sensu lub w razie problemów z zapisem do rejestru
                MessageBox.Show("Nie udało się poprawnie zapisać ustawień. Może to być problem z uprawnieniami bądź błędne dane");
            }
        }
    }
}

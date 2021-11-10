using Microsoft.Win32;
using MultiThreadTask.Commands;
using MultiThreadTask.Encrypt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiThreadTask.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public RelayCommands FromBtnCommand { get; set; }
        public RelayCommands ToBtnCommand { get; set; }
        public RelayCommands CopyBtnCommand { get; set; }
        public RelayCommands ResumeBtnCommand { get; set; }
        public RelayCommands PauseBtnCommand { get; set; }
        public string EncryptedFile { get; set; }
        string fileContentTo = string.Empty;
        string filePathTo = string.Empty;

        string fileContentFrom = string.Empty;
        string filePathFrom = string.Empty;
        public int TextLength { get; set; }
        Thread t;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            FromBtnCommand = new RelayCommands((sender) =>
            {
                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Get the path of specified file
                        filePathFrom = openFileDialog.FileName;

                        //Read the contents of the file into a stream
                        var fileStream = openFileDialog.OpenFile();

                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            fileContentFrom = reader.ReadToEnd();
                        }
                    }
                    mainWindow.fromTxtbx.Text = filePathFrom;
                }
                string text = File.ReadAllText(filePathFrom);
                EncryptedFile = EncryptClass.Encrypt(text);
                TextLength = EncryptedFile.Length / 100;
            });
            ToBtnCommand = new RelayCommands((sender) =>
            {

                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Get the path of specified file
                        filePathTo = openFileDialog.FileName;

                        //Read the contents of the file into a stream
                        var fileStream = openFileDialog.OpenFile();

                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            fileContentTo = reader.ReadToEnd();
                        }
                    }
                    mainWindow.toTxtbx.Text = filePathTo;
                }

            });
            CopyBtnCommand = new RelayCommands((sender) =>
            {
                t = new Thread(() => { File.AppendAllText(filePathTo, EncryptedFile); });
                t.Start();
                mainWindow.progressBar.Value = 100;
                
            });

            ResumeBtnCommand = new RelayCommands((sender) =>
            {
                try
                {
                    t.Resume();

                }
                catch (Exception)
                {

                }
            });

            PauseBtnCommand = new RelayCommands((sender) =>
            {
                try
                {
                    t.Suspend();

                }
                catch (Exception)
                {

                }

            });
        }
    }
}

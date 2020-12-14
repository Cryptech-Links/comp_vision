using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Diagnostics;

namespace comp_vision
{
    public partial class Main : Form
    {
        VideoCapture capture_Video;
        Image<Bgr, Byte> ImageFrame;
        CascadeClassifier Classifier;
        string xml_file = "haarcascade_frontalface_default.xml", path = @"C:\Users\" + Environment.UserName + @"\Documents\computer vision - image output\";
        string xml_folder = @"C:\Users\" + Environment.UserName + @"\Documents\computer vision - xml_files\";
        bool save_objs, files_set;

        public Main()
        {
            InitializeComponent();
            check_xml_files(); 
        }

        private void startCamera()
        {
            try
            {
                capture_Video = new VideoCapture(0);
                Application.Idle += ProcessFrame;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (files_set)
            {
                try
                {
                    imageBox1.Image = capture_Video.QueryFrame();
                    ImageFrame = capture_Video.QueryFrame().ToImage<Bgr, byte>();
                    Classifier = new CascadeClassifier(xml_folder + xml_file);

                    if (imageBox1.Image != null)
                    {
                        Image<Gray, byte> GrayFrame = ImageFrame.Convert<Gray, byte>().Clone();
                        //GrayFrame.Flip(Emgu.CV.CvEnum.FlipType.Vertical);
                        CvInvoke.EqualizeHist(GrayFrame, GrayFrame);

                        var faces = Classifier.DetectMultiScale(GrayFrame, 1.15, 10);

                        foreach (var face in faces)
                        {
                            ImageFrame.Draw(face, new Bgr(0, 255, 0), 2);
                            //processOutputTextBox.AppendText("eye detected");
                            if (save_objs)
                                ImageFrame.Save(path + set_file_Names(path) + ".bmp");
                        }
                        imageBox2.Image = ImageFrame;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                Application.Exit();
        }

        private void xml_selector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (xml_selector.SelectedIndex == 0)
                xml_file = "haarcascade_frontalface_default.xml";
            else
                xml_file = "haarcascade_eye.xml";
        }

        private void create_directory(string path_string)
        {
            if(!Directory.Exists(path_string))
                Directory.CreateDirectory(path_string);
        }

        private string set_file_Names(string folder_path)
        {
            int num_of_files;
            string file_name = "";

            num_of_files = Directory.GetFiles(folder_path).Length;
            num_of_files += 1;
            return file_name = "image" + num_of_files.ToString();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.Start(path);
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                save_objs = true;
            else
                save_objs = false;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            create_directory(path);
            create_directory(xml_folder);
            xml_selector.SelectedIndex = 0;
            startCamera();
        }

        void check_xml_files()
        {
            string[] xml = {"haarcascade_frontalface_default.xml", "haarcascade_eye.xml"};
            foreach(string file in xml)
            {
                try
                {
                    if (!File.Exists(xml_folder + file))
                    {
                        if (MessageBox.Show(@"xml files not found. Copy all needed xml files to Document\computer vision - xml_files\", "Files Not found", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                        {
                            files_set = false;
                            return;
                        }
                    }
                    else
                        files_set = true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using MigraDoc.DocumentObjectModel;
using System.Drawing.Imaging;
using MigraDoc.RtfRendering;
using PdfSharp.Pdf;
using MigraDoc.Rendering;
using System.Diagnostics;

namespace FurnaceController
{
    public partial class History : Form
    {
        string[] files;
      
        public History()
        {
            InitializeComponent();
            files = Directory.GetFiles(Application.StartupPath + "\\stats\\", "*.xml");
            for (int i = 0; i < files.Length; i++)
            {
                string nbr = files[i].Substring(files[i].LastIndexOf("\\") + 1);
                nbr = nbr.Substring(0, nbr.IndexOf("."));
                listBox1.Items.Insert(0, nbr);
            }
            if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = 0;
            }
            
            chart2.ChartAreas[0].AxisX.Title = "Time (Seconds)";
            chart2.ChartAreas[0].AxisY.Title = "Temperature (°C)";
            chart2.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 8);
            chart2.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 8);
            chart1.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 8);
            chart1.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 8);
            chart1.ChartAreas[0].AxisX.Title = "Time (Seconds)";
            chart1.ChartAreas[0].AxisY.Title = "Pressure (Bar)";
            chart1.ChartAreas[0].AxisY.IsLogarithmic = true;
            chart1.ChartAreas[0].AxisY.LogarithmBase = 10;
            chart1.ChartAreas[0].AxisX.Interval = 3600;
            chart2.ChartAreas[0].AxisX.Interval = 3600;
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart2.Series[0].Points.Clear();
            chart2.Series[1].Points.Clear();
            chart2.Series[2].Points.Clear();
            listView1.Items.Clear();
            XmlReader reader = XmlReader.Create(Application.StartupPath + "\\stats\\" + listBox1.SelectedItem.ToString() +".xml", settings);

            if (reader.ReadToFollowing("BrazeNumber"))
            {
                reader.MoveToAttribute(0);
                textBox1.Text = reader.Value;
            }
            if (reader.ReadToFollowing("Date"))
            {
                reader.MoveToAttribute(0);
                textBox2.Text = reader.Value;
            }
            if (reader.ReadToFollowing("Duration"))
            {
                reader.MoveToAttribute(0);
                textBox3.Text = reader.Value;
            }
            if (reader.ReadToFollowing("Status"))
            {
                reader.MoveToAttribute(0);
                textBox5.Text = reader.Value;
            }
            
            chart2.Titles[0].Text = "Temperatures - Braze # " + textBox1.Text + " " + textBox2.Text;
            chart1.Titles[0].Text = "Pressure - Braze #" + textBox1.Text + " " + textBox2.Text;

            if (reader.ReadToFollowing("Process"))
            {
                if (reader.ReadToDescendant("Log"))
                {
                    reader.MoveToAttribute(0);
                    string value = reader.Value;
                    reader.MoveToAttribute(1);
                    string error = reader.Value;
                    if (error.Equals("True"))
                    {
                        listView1.Items.Add(value).ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        listView1.Items.Add(value);
                    }
                    while (reader.ReadToNextSibling("Log"))
                    {
                        reader.MoveToAttribute(0);
                        value = reader.Value;
                        reader.MoveToAttribute(1);
                        error = reader.Value;
                        if (error.Equals("True"))
                        {
                            listView1.Items.Add(value).ForeColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            listView1.Items.Add(value);
                        }
                    }
                }
            }
            if (reader.ReadToFollowing("VacuumPressure"))
            {
                if (reader.ReadToDescendant("Measurement"))
                {
                    reader.MoveToAttribute(0);
                    double value = double.Parse(reader.Value);
                    if (value > 0)
                    {
                        chart1.Series[0].Points.AddY(value);
                    }
                    while (reader.ReadToNextSibling("Measurement"))
                    {
                        reader.MoveToAttribute(0);
                        value = double.Parse(reader.Value);
                        if (value > 0)
                        {
                            chart1.Series[0].Points.AddY(value);
                        }
                    }
                }
            }

            if (reader.ReadToNextSibling("NormalPressure"))
            {
                if (reader.ReadToDescendant("Measurement"))
                {
                    reader.MoveToAttribute(0);
                    double value = double.Parse(reader.Value);
                    if (value > 0)
                    {
                        chart1.Series[1].Points.AddY(value);
                    }
                    while (reader.ReadToNextSibling("Measurement"))
                    {
                        reader.MoveToAttribute(0);
                        value = double.Parse(reader.Value);
                        if (value < 0.1)
                            value = 0.001;
                        chart1.Series[1].Points.AddY(value);
                    }
                }
            }

            if (reader.ReadToFollowing("Temp1"))
            {
                if (reader.ReadToDescendant("Measurement"))
                {
                    reader.MoveToAttribute(0);
                    double value = double.Parse(reader.Value);
                    chart2.Series[2].Points.AddY(value);
                    while (reader.ReadToNextSibling("Measurement"))
                    {
                        reader.MoveToAttribute(0);
                        value = double.Parse(reader.Value);
                        chart2.Series[0].Points.AddY(value);
                    }
                }
            }

            if (reader.ReadToFollowing("Temp2"))
            {
                if (reader.ReadToDescendant("Measurement"))
                {
                    reader.MoveToAttribute(0);
                    double value = double.Parse(reader.Value);
                    chart2.Series[1].Points.AddY(value);
                    while (reader.ReadToNextSibling("Measurement"))
                    {
                        reader.MoveToAttribute(0);
                        value = double.Parse(reader.Value);
                        chart2.Series[1].Points.AddY(value);
                    }
                }
            }

            if (reader.ReadToFollowing("Temp3"))
            {
                if (reader.ReadToDescendant("Measurement"))
                {
                    reader.MoveToAttribute(0);
                    double value = double.Parse(reader.Value);
                    chart2.Series[0].Points.AddY(value);
                    while (reader.ReadToNextSibling("Measurement"))
                    {
                        reader.MoveToAttribute(0);
                        value = double.Parse(reader.Value);
                        chart2.Series[2].Points.AddY(value);                       
                    }
                }
            }

            chart2.ChartAreas[0].AxisY.IsStartedFromZero = false;
            chart2.ChartAreas[0].AxisY.Minimum = (int)chart2.Series[0].Points[0].YValues[0];
       }

        private void button3_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                renderPDF(saveFileDialog1.FileName);
            }
            
            //string filename = Guid.NewGuid().ToString() + ".pdf";

        }

        private void renderPDF(string filename)
        {
            Bitmap bp = new Bitmap(900, 600);
            int hh = chart2.Height; ;
            chart2.Dock = DockStyle.None;
            chart2.Size = new Size(900, 600);
            chart2.DrawToBitmap(bp, new Rectangle(0, 0, 900, 600));
            chart2.Dock = DockStyle.Bottom;
            chart2.Height = hh;
            bp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            bp.Save(Application.StartupPath + "\\temp\\testa.png", ImageFormat.Png);

            Bitmap bp2 = new Bitmap(900, 600);
            hh = chart1.Height; ;
            chart1.Dock = DockStyle.None;
            chart1.Size = new Size(900, 600);
            chart1.DrawToBitmap(bp2, new Rectangle(0, 0, 900, 600));
            chart1.Dock = DockStyle.Bottom;
            chart1.Height = hh;
            bp2.RotateFlip(RotateFlipType.Rotate90FlipNone);
            bp2.Save(Application.StartupPath + "\\temp\\testb.png", ImageFormat.Png);

            Document document = CreateDocument();
            string ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(document);
            bool unicode = true;

            PdfFontEmbedding embedding = PdfFontEmbedding.Always;  // Set to PdfFontEmbedding.None or PdfFontEmbedding.Always only

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);

            // Associate the MigraDoc document with a renderer
            pdfRenderer.Document = document;

            // Layout and render document to PDF
            pdfRenderer.RenderDocument();

            // Save the document...

            pdfRenderer.PdfDocument.Save(filename);

            Process.Start(filename);
            //  Bitmap bp = new Bitmap(1024, 768);
            // chart2.DrawToBitmap(bp, new Rectangle(0, 0, 1024, 768));
            // bp.Save(Application.StartupPath + "\\testc.png", ImageFormat.Png);
        }

         Document CreateDocument()
        {
            // Create a new MigraDoc document
            Document document = new Document();

            // Add a section to the document
            Section section = document.AddSection();

            // Add a paragraph to the section
            Paragraph paragraph = section.AddParagraph();

            // Add some text to the paragraph
            paragraph.AddImage(Application.StartupPath + "\\images\\logo.png");
            paragraph.AddFormattedText("Braze number: " + textBox1.Text + "\n", TextFormat.Bold);
            paragraph.AddFormattedText("Date: " + textBox2.Text + "\n", TextFormat.NotBold);
            paragraph.AddFormattedText("Duration: " + textBox3.Text + "\n", TextFormat.NotBold);
            paragraph.AddFormattedText("Status: " + textBox5.Text + "\n\n", TextFormat.NotBold);
            paragraph.AddFormattedText("Log: " +"\n", TextFormat.Bold);
            int loCount = 0;
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Text.Contains("The large Nitrogen valve is opened."))
                {
                    if (loCount < 2)
                    {
                        paragraph.AddFormattedText(listView1.Items[i].Text + "\n", TextFormat.NotBold);
                    }
                    loCount++;
                    if (loCount == 2)
                    {
                        paragraph.AddFormattedText("...\n", TextFormat.NotBold);
                    }
                }
                else if (listView1.Items[i].Text.Contains("The large Nitrogen valve is closed."))
                {
                    if (loCount < 2)
                    {
                        paragraph.AddFormattedText(listView1.Items[i].Text + "\n", TextFormat.NotBold);
                    }
                    
                }
                else
                {
                    paragraph.AddFormattedText(listView1.Items[i].Text + "\n", TextFormat.NotBold);
                }
            }
            //paragraph.AddImage("../../SomeImage.png");
            //paragraph.AddImage("../../Logo.pdf");
            //section.AddImage("../../Logo.pdf");
            section.AddImage(Application.StartupPath + "\\temp\\testa.png");
            section.AddImage(Application.StartupPath + "\\temp\\testb.png");

            return document;
        }

         private void button1_Click(object sender, EventArgs e)
         {
             this.Close();
         }
       
    }
}

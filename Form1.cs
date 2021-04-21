using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;

namespace Практика_3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            /*Первоначальные настройки*/
            chart1.Series.Clear();
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            if (dateTimePicker2.Value <= dateTimePicker1.Value) { 
                MessageBox.Show("Некорректно введена дата", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                return; 
            }
            if ((comboBox1.SelectedIndex == 1 && dateTimePicker1.Value.AddDays(7) > dateTimePicker2.Value) ||
               (comboBox1.SelectedIndex == 2 && dateTimePicker1.Value.AddMonths(1) > dateTimePicker2.Value) ||
               (comboBox1.SelectedIndex == 3 && dateTimePicker1.Value.AddYears(1) > dateTimePicker2.Value))
            {
                MessageBox.Show("Выбран некорректный шаг", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }            
                       
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    DayStep();
                    break;
                case 1:
                    CustomStep(7);
                    break;
                case 2:
                    CustomStep(31);
                    break;
                case 3:
                    CustomStep(365);
                    break;
            }
        }

        private void DayStep()
        {
            Series s = new Series("Температура в Москве");
            s.ChartType = SeriesChartType.Line;
            var el = GetInfo_FromServer();
            for (int i=0; i< el.Count(); i++)
            {
                s.Points.AddXY(DateTime.Parse(el[i].Element("date").Value), 
                                Double.Parse(el[i].Element("temperature").Value.Replace('.',',')));
            }
            chart1.Series.Add(s);
        }

        private void CustomStep(int step)
        {
            Series s = new Series("Температура в Москве");
            s.ChartType = SeriesChartType.Line;
            var el = GetInfo_FromServer();
            double sum = 0;
            for (int i = 0; i < el.Count(); i++)
            {
                sum += Double.Parse(el[i].Element("temperature").Value.Replace('.', ','));
                if (i%step == 0 && i != 0)
                {
                    s.Points.AddXY(DateTime.Parse(el[i].Element("date").Value), sum/step);
                    sum = 0;
                }                
            }
            chart1.Series.Add(s);
        }

        private List<XElement> GetInfo_FromServer()
        {
            String url = "https://api.meteostat.net/v1/history/daily?station=UUWW0&start=" + 
                dateTimePicker1.Value.ToString("yyyy-MM-dd") + "&end=" + 
                dateTimePicker2.Value.ToString("yyyy-MM-dd") + "&key=L23FNnUI";            
            XDocument xDoc = JsonConvert.DeserializeXNode(new WebClient().DownloadString(url), "Root");              
            return xDoc.Element("Root").Elements("data").ToList();
        }

    }
}

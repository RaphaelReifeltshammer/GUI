#region Copyright
///<remarks>
/// <GRAL Graphical User Interface GUI>
/// Copyright (C) [2019]  [Dietmar Oettl, Markus Kuntner]
/// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
/// the Free Software Foundation version 3 of the License
/// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
/// You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.
///</remarks>
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GralIO;
using Gral;
using GralData;

namespace GralDomForms
{
	/// <summary>
    /// Load meteo data for the matching process
    /// </summary>
    public partial class MeteoInput_Matchlocalobs: Form
    {
        public string MetFile1;
        public int FileLength1;
        public string DecSep1;
        public string DecSepUser=".";
        public char RowSep = ',';
        public GralItemData.MatchMeteoData MMOData;
        public string[] StartDate = new string[3];
        public string[] EndDate = new string[3];
        public double Anemometerheight;

        public MeteoInput_Matchlocalobs()
        {
            InitializeComponent();
        }       


        private void button1_Click(object sender, EventArgs e)
        {
            //hide calendars
            label1.Visible = false;
            label2.Visible = false;
            monthCalendar1.Visible = false;
            monthCalendar2.Visible = false;

            //fill Listbox
            listView1.Clear();
            listView1.GridLines = true;
            listView1.Columns.Add("Date", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Hour", 40, HorizontalAlignment.Center);
            listView1.Columns.Add("Vel. [m/s]", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Dir. [deg]", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("SC", 40, HorizontalAlignment.Center);

            
            //reading the file and storing the data in variables
			List<WindData> winddata = new List<WindData>();
            IO_ReadFiles readwindfile = new IO_ReadFiles
            {
                WindDataFile = MetFile1,
                WindData = winddata
            };
            readwindfile.ReadMeteoFiles(1000000, RowSep, DecSep1, DecSepUser);
			winddata = readwindfile.WindData;
			readwindfile = null;
			
			int index = Math.Min(50, winddata.Count);
			int n1 = 0;
			foreach(WindData wd in winddata)
			{
				MMOData.Date[n1] = wd.Date;
				
				if (n1 < index) // fill preview
				{
					//read met file finally
					MMOData.Time[n1] = wd.Time;
					MMOData.WindVel[n1] = wd.Vel;
					MMOData.WindDir[n1] = wd.Dir;
					MMOData.SC[n1] = wd.StabClass;
					
					ListViewItem item = new ListViewItem(wd.Date);
					//if (zeile.GetLength(0) > 4)
					try
					{
						item.SubItems.Add(wd.Time);
						item.SubItems.Add(wd.Vel.ToString());
						item.SubItems.Add(wd.Dir.ToString());
						item.SubItems.Add(wd.StabClass.ToString());
					}
					catch
					{
					}
					listView1.Items.Add(item);
				}
				
				n1++;
			}
			winddata = null;
			
          	//show month calender to select the desired time period of the time series
            try
            {
                StartDate = MMOData.Date[0].Split(new char[] { '.', ':', '-' });
                EndDate = MMOData.Date[FileLength1 - 1].Split(new char[] { '.', ':', '-' });
                if (Convert.ToInt16(StartDate[2]) < 1900)
                {
                    if (Convert.ToInt32(StartDate[2]) < 60)
                        StartDate[2] = "20" + StartDate[2];
                    else
                        StartDate[2] = "19" + StartDate[2];
                }
                if (Convert.ToInt32(EndDate[2]) < 1900)
                {
                    if (Convert.ToInt32(EndDate[2]) < 60)
                        EndDate[2] = "20" + EndDate[2];
                    else
                        EndDate[2] = "19" + EndDate[2];
                }
                monthCalendar1.MinDate = new System.DateTime(Convert.ToInt16(StartDate[2]), Convert.ToInt16(StartDate[1]), Convert.ToInt16(StartDate[0]), 0, 0, 0, 0);
                monthCalendar1.MaxDate = new System.DateTime(Convert.ToInt16(EndDate[2]), Convert.ToInt16(EndDate[1]), Convert.ToInt16(EndDate[0]), 0, 0, 0, 0);
                monthCalendar2.MinDate = new System.DateTime(Convert.ToInt16(StartDate[2]), Convert.ToInt16(StartDate[1]), Convert.ToInt16(StartDate[0]), 0, 0, 0, 0);
                monthCalendar2.MaxDate = new System.DateTime(Convert.ToInt16(EndDate[2]), Convert.ToInt16(EndDate[1]), Convert.ToInt16(EndDate[0]), 0, 0, 0, 0);
                label1.Visible = true;
                label2.Visible = true;
                monthCalendar1.Visible = true;
                monthCalendar2.Visible = true;
                monthCalendar1.SelectionStart = new System.DateTime(Convert.ToInt16(StartDate[2]), Convert.ToInt16(StartDate[1]), Convert.ToInt16(StartDate[0]), 0, 0, 0, 0);
				monthCalendar1.SetDate(monthCalendar1.MinDate);
				monthCalendar2.SetDate(monthCalendar2.MaxDate);
				#if __MonoCS__
				#else
				monthCalendar2.TodayDate = monthCalendar2.MaxDate;			
				#endif
            }
            catch
            {
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
        	button1_Click(null, null);
            //reading anemometerheight
            MMOData.AnemometerHeight = Convert.ToDouble(numericUpDown1.Value);
            
            MMOData.Meteo = true;
			
			List<WindData> winddata = new List<WindData>();
            IO_ReadFiles readwindfile = new IO_ReadFiles
            {
                WindDataFile = MetFile1,
                WindData = winddata
            };
            if (readwindfile.ReadMeteoFiles(1000000, RowSep, DecSep1, DecSepUser) == false)
			{
				MessageBox.Show(this, "Error when reading Meteo-File in line" + winddata.Count, "GRAL GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
				if (winddata.Count == 0)
					MMOData.Meteo = false;
			}
			
			winddata = readwindfile.WindData;
			//readwindfile = null;
			
			if (MMOData.Meteo)
			{
				int n1 = 0; int length = 0;
				string[] date = new string[3];
				int error_count = 0;
				foreach(WindData wd in winddata)
				{
					date = wd.Date.Split(new char[] { ':', '-', '.' });
					//check whether the date is within the chosen time interval
					if (Convert.ToInt16(date[2]) < 1900)
					{
						if (Convert.ToInt16(date[2]) < 60)
							date[2] = "20" + date[2];
						else
							date[2] = "19" + date[2];
					}
					
					if (((Convert.ToInt32(date[2]) * 10000 + Convert.ToInt16(date[1]) * 100 + Convert.ToInt16(date[0])) >= (Convert.ToInt32(StartDate[2]) * 10000 + Convert.ToInt16(StartDate[1]) * 100 + Convert.ToInt16(StartDate[0]))) &&
					    ((Convert.ToInt32(date[2]) * 10000 + Convert.ToInt16(date[1]) * 100 + Convert.ToInt16(date[0])) <= (Convert.ToInt32(EndDate[2]) * 10000 + Convert.ToInt16(EndDate[1]) * 100 + Convert.ToInt16(EndDate[0]))))
					{
						
						MMOData.Date[length] = wd.Date;
						MMOData.Time[length] = wd.Time;
						MMOData.WindVel[length] = wd.Vel;
						MMOData.WindDir[length] = wd.Dir;
						MMOData.SC[length] = wd.StabClass;
						MMOData.Hour[length] = wd.Hour;
						
						//check for unplausible values
						if(MMOData.WindVel[length]>55||MMOData.WindVel[length]<0)
						{
							if (error_count < 4)
								MessageBox.Show(this, "Wind speed implausible - check line number" + Convert.ToString(n1 + 1), "GRAL GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
							error_count++;
						}
						else if (MMOData.WindDir[length] > 360 || MMOData.WindDir[length] < 0)
						{
							if (error_count < 4)
								MessageBox.Show(this, "Wind direction implausible - check line number" + Convert.ToString(n1 + 1), "GRAL GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
							error_count++;
						}
						else if (MMOData.SC[length] > 7 || MMOData.SC[length] < 1)
						{
							if (error_count < 4)
								MessageBox.Show(this, "Stability class implausible - check line number" + Convert.ToString(n1 + 1), "GRAL GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
							error_count++;
						}
						else
							length++;
					}
					n1++;
				}
				
				if (error_count > 0)
					MessageBox.Show(this, error_count.ToString() + " errors at the wind file", "GRAL GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
				
				MMOData.FileLenght = length;
			}
			winddata = null;
		
			MMOData.MetColumnSeperator = RowSep;
			MMOData.MetDecSeperator = DecSepUser;

            Close();
        }

        //different row-and decimal separators defined by the user

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            DecSepUser = ".";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            DecSepUser = ",";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            RowSep = ',';
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            RowSep = ' ';
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            RowSep = ';';
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            RowSep = '\t';
        }

        private void Form2_Load(object sender, EventArgs e)
        {
        	if (Owner != null)
        		Location = new Point(Math.Max(0,Owner.Location.X + Owner.Width / 2 - Width / 2 - 100),
        		                    Math.Max(0, Owner.Location.Y + Owner.Height / 2 - Height / 2 -100));
            if (Anemometerheight > 0)
            {
                numericUpDown1.Value = (decimal) Anemometerheight;
            }
        }

        //select start date for met data input
        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            monthCalendar1.TodayDate = monthCalendar1.SelectionStart;
            DateTime startD = monthCalendar1.TodayDate;
            StartDate[2] = Convert.ToString(startD.Year);
            StartDate[1] = Convert.ToString(startD.Month);
            StartDate[0] = Convert.ToString(startD.Day);
        }

        //select end date for met data input
        private void monthCalendar2_DateChanged(object sender, DateRangeEventArgs e)
        {
            monthCalendar2.TodayDate = monthCalendar2.SelectionStart;
            DateTime startD = monthCalendar2.TodayDate;
            EndDate[2] = Convert.ToString(startD.Year);
            EndDate[1] = Convert.ToString(startD.Month);
            EndDate[0] = Convert.ToString(startD.Day);
        }
    }
}
﻿/*
Copyright (c) 2016, Los Alamos National Security, LLC
All rights reserved.
Copyright 2016. Los Alamos National Security, LLC. This software was produced under U.S. Government contract 
DE-AC52-06NA25396 for Los Alamos National Laboratory (LANL), which is operated by Los Alamos National Security, 
LLC for the U.S. Department of Energy. The U.S. Government has rights to use, reproduce, and distribute this software.
NEITHER THE GOVERNMENT NOR LOS ALAMOS NATIONAL SECURITY, LLC MAKES ANY WARRANTY, EXPRESS OR IMPLIED, 
OR ASSUMES ANY LIABILITY FOR THE USE OF THIS SOFTWARE. If software is modified to produce derivative works, 
such modified software should be clearly marked, so as not to confuse it with the version available from LANL.

Additionally, redistribution and use in source and binary forms, with or without modification, are permitted provided
that the following conditions are met:
•	Redistributions of source code must retain the above copyright notice, this list of conditions and the following 
disclaimer. 
•	Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following 
disclaimer in the documentation and/or other materials provided with the distribution. 
•	Neither the name of Los Alamos National Security, LLC, Los Alamos National Laboratory, LANL, the U.S. Government, 
nor the names of its contributors may be used to endorse or promote products derived from this software without specific 
prior written permission. 
THIS SOFTWARE IS PROVIDED BY LOS ALAMOS NATIONAL SECURITY, LLC AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL LOS ALAMOS NATIONAL SECURITY, LLC OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING 
IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using AnalysisDefs;
using NCCReporter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NewUI
{
    using Integ = NCC.IntegrationHelpers;
    using N = NCC.CentralizedState;

    public partial class IDDMeasurementList : Form
    {
        private List<Measurement> mlist;
        protected LMLoggers.LognLM ctrllog;
		bool allmea = false;

        public IDDMeasurementList(string filter = "")
        {
            InitializeComponent();
			bool LMOnly = (filter.CompareTo("unspecified") == 0);
			allmea =  filter == "";
            this.Text = allmea?"All Measurements":filter + " Measurements";
            Detector det = Integ.GetCurrentAcquireDetector();
			this.Text += " for Detector " + det.Id.DetectorId;
            mlist = N.App.DB.MeasurementsFor(det, AssaySelectorExtensions.SrcToEnum(filter));
			PrepList(LMOnly, filter, det.Id.DetectorId);
        }

		public IDDMeasurementList(List<INCCDB.IndexedResults> ilist, string filter = "", string inspnum = "")
        {
            InitializeComponent();
			allmea = filter == "";
			bool LMOnly = (filter.CompareTo("unspecified") == 0);
			this.Text = allmea ? "All Measurements" : filter + " Measurements";
            Detector det = Integ.GetCurrentAcquireDetector();
			this.Text += " for Detector " + det.Id.DetectorId;
			if (!string.IsNullOrEmpty(inspnum))
				this.Text += " for inspection #" + inspnum;
			mlist = N.App.DB.MeasurementsFor(ilist, LMOnly);
			PrepList(LMOnly, filter, det.Id.DetectorId);
        }

		void PrepList(bool LMOnly, string filter, string det)
		{
			if (LMOnly)  // LMOnly
				mlist.RemoveAll(EmptyCSVFile);    // cull those without LM CSV results
			else
				mlist.RemoveAll(EmptyINCC5File);  // cull those with traditional INCC5 results
			ctrllog = N.App.Loggers.Logger(LMLoggers.AppSection.Control);
			LoadList();
			if (!allmea)
				listView1.Columns[listView1.Columns.Count -1].Width = 0;
            if (mlist.Count == 0)
            {
                string msg = string.Format("No {0}measurements for {1} found.", TypeTextFragment(filter), det);
                MessageBox.Show(msg, "WARNING");
            }
		}

		void LoadList()
		{
 			listView1.ShowItemToolTips = true;
			foreach (Measurement m in mlist)
            {
                string ItemWithNumber = string.IsNullOrEmpty(m.MeasurementId.Item.item) ? "Empty" : m.AcquireState.ItemId.item;
                if (Path.GetFileName(m.MeasurementId.FileName).Contains("_"))
                    //scan file name to display subsequent reanalysis number...... hn 9.21.2015
                    ItemWithNumber += "(" + Path.GetFileName(m.MeasurementId.FileName).Substring(Path.GetFileName(m.MeasurementId.FileName).IndexOf('_') + 1, 2) + ")";
                ListViewItem lvi = new ListViewItem(new string[] { ItemWithNumber,
					string.IsNullOrEmpty(m.AcquireState.stratum_id.Name) ? "Empty" : m.AcquireState.stratum_id.Name, m.MeasDate.DateTime.ToString("MM.dd.yy"), m.MeasDate.DateTime.ToString("HH:mm:ss"),
					m.MeasOption.PrintName() });
                listView1.Items.Add(lvi);
				lvi.ToolTipText = GetMainFilePath(m.ResultsFiles, m.MeasOption);
				if (string.IsNullOrEmpty(lvi.ToolTipText))
					lvi.ToolTipText = "No results file available";
            }
		}

		string TypeTextFragment(string filter)
		{
			if (string.IsNullOrEmpty(filter))
				return filter;
			else if (filter.CompareTo("unspecified") == 0)
				return "List Mode ";
			else
				return (filter=="Rates"?"Rates Only": filter) + " ";
		}

        public static bool EmptyINCC5File(Measurement m)
        {
            return m.ResultsFiles.Count <= 0 || string.IsNullOrEmpty(m.ResultsFiles.PrimaryINCC5Filename.Path);
        }
		public static bool EmptyCSVFile(Measurement m)
        {
            return string.IsNullOrEmpty(m.ResultsFiles.CSVResultsFileName.Path);
        }
        private void PrintBtn_Click(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetTempFileName();
            FileStream f = new FileStream(path, FileMode.OpenOrCreate);
            StreamWriter s = new StreamWriter(f);
            s.AutoFlush = true;
            s.Write (String.Format ("{0,20}\t\t{1,20}\t\t{2,20}\t\t{3,20}\t\t{4,20}\r\n","Item id","Stratum id","Measurement Type","Measurement Date","Measurement Time"));
            foreach (ListViewItem lvi in listView1.Items)
            {
                s.Write(String.Format("{0,20}\t\t{1,20}\t\t{2,20}\t\t{3,20}\t\t{4,20}\r\n", lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[4].Text, lvi.SubItems[2].Text, lvi.SubItems[3].Text));
            }
            f.Close();
            PrintForm pf = new PrintForm(path, this.Text, "Print List");
            pf.ShowDialog();
            File.Delete(path);
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            ShowResults();
            Close();
        }

        private void HelpBtn_Click(object sender, EventArgs e)
        {
              Help.ShowHelp (null,".\\inccuser.chm");
        }

        private void ShowResults()
        {
            string notepadPath = Path.Combine(Environment.SystemDirectory, "notepad.exe");
            foreach (ListViewItem lvi in listView1.Items)
            {
                if (lvi.Selected)
                {
                    if (File.Exists(notepadPath))
                    {
						string path = GetMainFilePath(mlist[lvi.Index].ResultsFiles, mlist[lvi.Index].MeasOption);
                        if (File.Exists(path))
                            System.Diagnostics.Process.Start(notepadPath, path);
                        else if (!string.IsNullOrEmpty(path))
                            ctrllog.TraceEvent(LogLevels.Error, 22222, "The file path '" + path + "' cannot be accessed.");
                        else 
                            ctrllog.TraceEvent(LogLevels.Error, 22222, "No file path");
                    }
                    lvi.Selected = false;
                }
            }
        }

		private string GetMainFilePath(ResultFiles files, AssaySelector.MeasurementOption mo)
		{
			switch (mo)
			{
			case AssaySelector.MeasurementOption.unspecified:
				return files.CSVResultsFileName.Path;
			default:
				return files.PrimaryINCC5Filename.Path;
			}
		}

		private void CancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowResults();
        }

    }
}

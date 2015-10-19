﻿/*
Copyright (c) 2014, Los Alamos National Security, LLC
All rights reserved.
Copyright 2014. Los Alamos National Security, LLC. This software was produced under U.S. Government contract 
DE-AC52-06NA25396 for Los Alamos National Laboratory (LANL), which is operated by Los Alamos National Security, 
LLC for the U.S. Department of Energy. The U.S. Government has rights to use, reproduce, and distribute this software.  
NEITHER THE GOVERNMENT NOR LOS ALAMOS NATIONAL SECURITY, LLC MAKES ANY WARRANTY, EXPRESS OR IMPLIED, 
OR ASSUMES ANY LIABILITY FOR THE USE OF THIS SOFTWARE.  If software is modified to produce derivative works, 
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
using System;
using System.Windows.Forms;
using AnalysisDefs;
using DetectorDefs;
namespace NewUI
{

    using Integ = NCC.IntegrationHelpers;
    // todo: set range checking for fields,
    public partial class IDDAcquireInitSrc : Form
    {
        AcquireHandlers ah;
        public NormParameters np;
        public IDDAcquireInitSrc()
        {
            InitializeComponent();

            // Generate an instance of the generic acquire dialog event handlers object (this now includes the AcquireParameters object used for change tracking)
            ah = new AcquireHandlers();
            ah.mo = AssaySelector.MeasurementOption.initial;
            this.Text += " for detector " + ah.det.Id.DetectorName;
            np = Integ.GetCurrentNormParams(ah.det);  // a copy
            // Populate the UI fields with values from the local AcquireParameters object
            this.QCTestsCheckBox.Checked = ah.ap.qc_tests;
            this.PrintResultsCheckBox.Checked = ah.ap.print;
            this.CommentAtEndCheckBox.Checked = ah.ap.ending_comment;
            this.NumCyclesTextBox.Text = Format.Rend(ah.ap.num_runs);
            this.CommentTextBox.Text = ah.ap.comment;
            this.CountTimeTextBox.Text = Format.Rend(ah.ap.run_count_time);
            this.SourceIdTextBox.Text = ah.ap.item_id;
            this.MeasPrecisionTextBox.Text = ah.ap.meas_precision.ToString("F2");
            this.MinNumCyclesTextBox.Text = Format.Rend(ah.ap.min_num_runs);
            this.MaxNumCyclesTextBox.Text = Format.Rend(ah.ap.max_num_runs);
            
            this.UseAddASourceCheckBox.Checked = np.biasTestUseAddasrc;
            this.DistanceToMoveTextBox.Text = np.biasTestAddasrcPosition.ToString("F1");
            this.SourceIdTextBox.Text = np.sourceId;
            DistanceToMoveTextBox.Enabled = np.biasTestUseAddasrc;
            switch (np.biasMode)
            {
                case NormTest.AmLiSingles:
                    AmLiNormRadioButton.Checked = true;
                    break;
                case NormTest.Cf252Doubles:
                    Cf252DblsNormRadioButton.Checked = true;
                    break;
                case NormTest.Cf252Singles:
                    Cf252SinglesNormRadioButton.Checked = true;
                    break;
                case NormTest.Collar:
                    break;
            }

            this.DataSourceComboBox.Items.Clear();
            foreach (ConstructedSource cs in System.Enum.GetValues(typeof(ConstructedSource)))
            {
                if (cs.AcquireChoices() || cs.LMFiles(ah.det.Id.SRType))
                    DataSourceComboBox.Items.Add(cs.HappyFunName());
            }
            if (ah.ap.acquire_type == AcquireConvergence.CycleCount)
            {
                this.UseNumCyclesRadioButton.Checked = true;
            }
            else if (ah.ap.acquire_type == AcquireConvergence.DoublesPrecision)
            {
                this.UseDoublesRadioButton.Checked = true;
            }
            else if (ah.ap.acquire_type == AcquireConvergence.TriplesPrecision)
            {
                this.UseTriplesRadioButton.Checked = true;
            }
            DataSourceComboBox.SelectedItem = ah.ap.data_src.HappyFunName();
        }


        private void Cf252DblsNormRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            np.biasMode = NormTest.Cf252Doubles;
        }

        private void Cf252SinglesNormRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            np.biasMode = NormTest.Cf252Singles;
        }

        private void AmLiNormRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            np.biasMode = NormTest.AmLiSingles;
        }

        private void UseAddASourceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            np.biasTestUseAddasrc = ((CheckBox)sender).Checked;
            DistanceToMoveTextBox.Enabled = np.biasTestUseAddasrc;
        }

        private void DistanceToMoveTextBox_Leave(object sender, EventArgs e)
        {
            // Try to convert the text to a positive, non-zero number            
            np.modified = (Format.ToNN(((TextBox)sender).Text, ref np.biasTestAddasrcPosition));

            // Auto-format or reset the textbox value, depending on whether the entered value was different/valid
            ((TextBox)sender).Text = np.biasTestAddasrcPosition.ToString("F1");
        }

        private void UseNumCyclesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ah.NumberOfCyclesRadioButton_CheckedChanged(sender, e);
        }

        private void UseDoublesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ah.DoublesMeasurementPrecisionRadioButton_CheckedChanged(sender, e);
        }

        private void UseTriplesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ah.TriplesMeasurementPrecisionRadioButton_CheckedChanged(sender, e);
        }

        private void QCTestsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ah.QCTestsCheckbox_CheckedChanged(sender, e);
        }

        private void PrintResultsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ah.PrintResultsCheckbox_CheckedChanged(sender, e);
        }

        private void CommentAtEndCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ah.CommentCheckbox_CheckedChanged(sender, e);
        }

        private void DataSourceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ah.DataSourceComboBox_SelectedIndexChanged(sender, e);
            CountTimeTextBox.Enabled = true;
            UseNumCyclesRadioButton.Enabled = true;
            UseDoublesRadioButton.Enabled = true;
            UseTriplesRadioButton.Enabled = true;
            UseNumCyclesRadioButton.Enabled = true;

            //only verification uses these
            MeasPrecisionTextBox.Enabled = false;
            MinNumCyclesTextBox.Enabled = false;
            MaxNumCyclesTextBox.Enabled = false;

            CommentAtEndCheckBox.Enabled = true;
            PrintResultsCheckBox.Enabled = true;

            //enable/disable selected controls here
            switch (ah.ap.data_src)
            {
                case ConstructedSource.Live:
                    // every set as above
                    break;
                case ConstructedSource.DB:
                    CountTimeTextBox.Enabled = false;
                    UseNumCyclesRadioButton.Enabled = false;
                    UseDoublesRadioButton.Enabled = false;
                    UseTriplesRadioButton.Enabled = false;
                    UseNumCyclesRadioButton.Enabled = false;
                    break;
                case ConstructedSource.CycleFile:
                    CountTimeTextBox.Enabled = false;
                    UseNumCyclesRadioButton.Enabled = false;
                    UseDoublesRadioButton.Enabled = false;
                    UseTriplesRadioButton.Enabled = false;
                    UseNumCyclesRadioButton.Enabled = false;
                    CommentAtEndCheckBox.Enabled = false;
                    break;
                case ConstructedSource.Manual:
                    CountTimeTextBox.Enabled = false;
                    UseNumCyclesRadioButton.Enabled = false;
                    UseDoublesRadioButton.Enabled = false;
                    UseTriplesRadioButton.Enabled = false;
                    UseNumCyclesRadioButton.Enabled = false;
                    CommentAtEndCheckBox.Enabled = false;
                    break;
                case ConstructedSource.ReviewFile:
                default:
                    CountTimeTextBox.Enabled = false;
                    UseNumCyclesRadioButton.Enabled = false;
                    UseDoublesRadioButton.Enabled = false;
                    UseTriplesRadioButton.Enabled = false;
                    UseNumCyclesRadioButton.Enabled = false;
                    CommentAtEndCheckBox.Enabled = false;
                    //PrintResultsCheckbox.Enabled = false;
                    break;
            }
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            if (ah.OKButton_Click(sender, e) == System.Windows.Forms.DialogResult.OK)
            {
                // todo: save off any norm params changes too
                //user can cancel in here during LM set-up, account for it.
                this.Visible = false;
                UIIntegration.Controller.SetAssay();  // tell the controller to do an assay operation using the current measurement state
                UIIntegration.Controller.Perform();  // start the measurement file or DAQ thread
                this.Close();
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void HelpBtn_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Help.ShowHelp(null, ".\\inccuser.chm", HelpNavigator.Topic, "/WordDocuments/initialsourcemeasurements.htm");
        }

        private void SourceIdTextBox_Leave(object sender, EventArgs e)
        {
            ah.ItemIdTextBox_Leave(sender, e);
        }

        private void CommentTextBox_Leave(object sender, EventArgs e)
        {
            ah.CommentTextBox_Leave(sender, e);
        }

        private void CountTimeTextBox_Leave(object sender, EventArgs e)
        {
            ah.CountTimeTextBox_Leave(sender, e);
        }

        private void NumCyclesTextBox_Leave(object sender, EventArgs e)
        {
            ah.NumCyclesTextBox_Leave(sender, e);
        }

        private void MeasPrecisionTextBox_Leave(object sender, EventArgs e)
        {
            ah.MeasurementPrecisionTextBox_Leave(sender, e);
        }

        private void MinNumCyclesTextBox_Leave(object sender, EventArgs e)
        {
            ah.MinNumCyclesTextBox_Leave(sender, e);
        }

        private void MaxNumCyclesTextBox_Leave(object sender, EventArgs e)
        {
            ah.MaxNumCyclesTextBox_Leave(sender, e);
        }

    }
}

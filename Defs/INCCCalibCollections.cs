﻿/*
Copyright (c) 2015, Los Alamos National Security, LLC
All rights reserved.
Copyright 2015. Los Alamos National Security, LLC. This software was produced under U.S. Government contract 
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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NCCReporter;
namespace AnalysisDefs
{

    using NC = NCC.CentralizedState;


    public partial class INCCDB
    {

        INCCAnalysisMethodMap dmam;
        /// <summary>
        /// The map of INCCSelector(detectors, materials) ->  AnalysisMethods
        /// </summary>
        public INCCAnalysisMethodMap DetectorMaterialAnalysisMethods
        {
            get
            {
                if (dmam == null)
                {
                    DataTable dt = NC.App.Pest.GetACollection(DB.Pieces.AnalysisMethodSpecifiers);
                    dmam = new INCCAnalysisMethodMap();

                    // column 0 is mat name, 1 is det name, 2 is mat id, 3 is det id, 4 is first choice boolean
                    foreach (DataRow dr in dt.Rows)
                    {
                        INCCSelector sel = new INCCSelector((string)dr["detector_name"], (string)dr["name"]); //use the names, not ids, gotta look em up
                        AnalysisMethods ams = new AnalysisMethods(sel);
                        ams.choices[(int)AnalysisMethod.KnownA] = DB.Utils.DBBool(dr["known_alpha"]);
                        ams.choices[(int)AnalysisMethod.KnownM] = DB.Utils.DBBool(dr["known_m"]);
                        ams.choices[(int)AnalysisMethod.Multiplicity] = DB.Utils.DBBool(dr["multiplicity"]);
                        ams.choices[(int)AnalysisMethod.CalibrationCurve] = DB.Utils.DBBool(dr["cal_curve"]);
                        ams.choices[(int)AnalysisMethod.AddASource] = DB.Utils.DBBool(dr["add_a_source"]);
                        ams.choices[(int)AnalysisMethod.Active] = DB.Utils.DBBool(dr["active"]);
                        ams.choices[(int)AnalysisMethod.ActivePassive] = DB.Utils.DBBool(dr["active_passive"]);
                        ams.choices[(int)AnalysisMethod.ActiveMultiplicity] = DB.Utils.DBBool(dr["active_mult"]);
                        ams.choices[(int)AnalysisMethod.Collar] = DB.Utils.DBBool(dr["collar"]);
                        ams.choices[(int)AnalysisMethod.CuriumRatio] = DB.Utils.DBBool(dr["curium_ratio"]);
                        ams.choices[(int)AnalysisMethod.TruncatedMultiplicity] = DB.Utils.DBBool(dr["truncated_mult"]);
                        ams.Normal = (AnalysisMethod)DB.Utils.DBInt32(dr["normal_method"]);
                        ams.Backup = (AnalysisMethod)DB.Utils.DBInt32(dr["backup_method"]);
                        dmam.Add(sel, ams);
                        //if (ams.AnySelected()) ams.choices[(int)AnalysisMethod.None] = false;
                    }

                    using (DB.AnalysisMethodSpecifiers db = new DB.AnalysisMethodSpecifiers())
                    {
                        foreach (INCCSelector sel in dmam.Keys)
                        {
                            IngestAnalysisMethodSpecificsFromDB(sel, dmam[sel], db);
                        }
                    }

                }
                return dmam;
            }
        }

        public void UpdateAnalysisMethods()
        {
            DB.AnalysisMethodSpecifiers db = new DB.AnalysisMethodSpecifiers();
            foreach (KeyValuePair<INCCSelector, AnalysisMethods> kv in DetectorMaterialAnalysisMethods)
            {
                if (kv.Value.modified)
                {
                    UpdateAnalysisMethods(kv.Key, kv.Value, db);
                    UpdateAnalysisMethodSpecifics(kv.Key.detectorid, kv.Key.material, db);
                }
            }
        }

        public void UpdateAnalysisMethod(INCCSelector sel, AnalysisMethods ams)
        {
            DB.AnalysisMethodSpecifiers db = new DB.AnalysisMethodSpecifiers();

            if (ams.modified)
            {
                UpdateAnalysisMethods(sel, ams, db);
            }
            UpdateAnalysisMethodSpecifics(sel.detectorid, sel.material,db);
        }


        public void UpdateAnalysisMethods(Detector det, string mat)
        {
            DB.AnalysisMethodSpecifiers db = new DB.AnalysisMethodSpecifiers();
            AnalysisDefs.AnalysisMethods sam = null;  
            var res =   // this finds the am for the given detector and acquire type
                    from am in this.DetectorMaterialAnalysisMethods
                    where (am.Key.detectorid.Equals(det.Id.DetectorId,StringComparison.OrdinalIgnoreCase) 
                    && am.Key.material == mat)
                    select am;
            foreach (KeyValuePair<INCCSelector, AnalysisMethods> kv in res)
            {
                sam = kv.Value;
                if (sam.modified)
                    UpdateAnalysisMethods(kv.Key, sam, db);
            }
        }

        public void UpdateAnalysisMethods(INCCSelector sel, AnalysisMethods am, DB.AnalysisMethodSpecifiers db = null)
        {
            if (db == null)
                db = new DB.AnalysisMethodSpecifiers();
            DB.ElementList saParams;
            saParams = am.ToDBElementList();       
            if (!db.Update(sel.detectorid, sel.material, saParams)) // am not there, so add it
            {
                NC.App.Pest.logger.TraceEvent(LogLevels.Verbose, 34027, "Failed to update analysis method spec for " + sel.ToString());
            }
            else
            {
                NC.App.Pest.logger.TraceEvent(LogLevels.Verbose, 34026, "Updated/created analysis method spec for " + sel.ToString());
            }
            am.modified = false;

        }

        // todo: collar/k5 detached params (no explicit detector mapping)
        /// the details
        public void UpdateAnalysisMethodSpecifics(string detname, string mat, DB.AnalysisMethodSpecifiers db = null)
        {
            if (db == null)
                db = new DB.AnalysisMethodSpecifiers();
            var res =   // this finds the am for the given detector and acquire type
                    from am in this.DetectorMaterialAnalysisMethods
                    where (am.Key.detectorid.Equals(detname, StringComparison.OrdinalIgnoreCase) && 
                           am.Key.material.Equals(mat, StringComparison.OrdinalIgnoreCase))
                    select am;
            if (res.Count() > 0)  // now execute the select expression and test the result for existence
            {
                KeyValuePair<INCCSelector, AnalysisMethods> kv = res.First();
                AnalysisMethods sam = kv.Value;  // the descriptor instance
                
                IEnumerator iter = kv.Value.GetMethodEnumerator();
                while (iter.MoveNext())
                {
                    System.Tuple<AnalysisMethod, INCCAnalysisParams.INCCMethodDescriptor> md = (System.Tuple<AnalysisMethod, INCCAnalysisParams.INCCMethodDescriptor>)iter.Current;
                    if (md.Item2 == null) // case from INCC5 transfer missing params, reflects file write bugs in INCC5 code
                    {
                        NC.App.Pest.logger.TraceEvent(LogLevels.Warning, 34029, "Missing {0}'s INCC {1} {2} method parameters, adding default values", detname, kv.Key.material, md.Item1.FullName());
                        //OK, there is probably smarter way of doing ths, but for now, does find the nulls, then add default params where necessary. hn 9.23.2015
                        if (md.Item2 == null)
                        {
                            INCCAnalysisParams.INCCMethodDescriptor rec = new INCCAnalysisParams.INCCMethodDescriptor();
                            switch (md.Item1)
                            {
                                case AnalysisMethod.Active:
                                    rec = (INCCAnalysisParams.INCCMethodDescriptor)new INCCAnalysisParams.active_rec();
                                    break;
                                case AnalysisMethod.ActiveMultiplicity:
                                    rec = (INCCAnalysisParams.INCCMethodDescriptor)new INCCAnalysisParams.active_mult_rec();
                                    break;
                                case AnalysisMethod.ActivePassive:
                                    rec = (INCCAnalysisParams.INCCMethodDescriptor)new INCCAnalysisParams.active_passive_rec();
                                    break;
                                case AnalysisMethod.AddASource:
                                    rec = (INCCAnalysisParams.INCCMethodDescriptor)new INCCAnalysisParams.add_a_source_rec();
                                    break;
                                case AnalysisMethod.CalibrationCurve:
                                    rec = (INCCAnalysisParams.INCCMethodDescriptor)new INCCAnalysisParams.cal_curve_rec();
                                    break;
                                case AnalysisMethod.Collar:
                                    //This may not be enough for collar params creation. hn 9.23.2015
                                    rec = (INCCAnalysisParams.INCCMethodDescriptor)new INCCAnalysisParams.collar_combined_rec();
                                    break;
                                case AnalysisMethod.CuriumRatio:
                                    rec = (INCCAnalysisParams.INCCMethodDescriptor)new INCCAnalysisParams.cm_pu_ratio_rec();
                                    break;
                                case AnalysisMethod.KnownA:
                                    rec = (INCCAnalysisParams.INCCMethodDescriptor)new INCCAnalysisParams.known_alpha_rec();
                                    break;
                                case AnalysisMethod.KnownM:
                                    rec = (INCCAnalysisParams.INCCMethodDescriptor)new INCCAnalysisParams.known_m_rec();
                                    break;
                                case AnalysisMethod.Multiplicity:
                                    rec = (INCCAnalysisParams.INCCMethodDescriptor)new INCCAnalysisParams.multiplicity_rec();
                                    break;
                                case AnalysisMethod.TruncatedMultiplicity:
                                    rec = (INCCAnalysisParams.INCCMethodDescriptor)new INCCAnalysisParams.truncated_mult_rec();
                                    break;
                                default:
                                    break;
                            }
                            sam.AddMethod(md.Item1, rec);
                        }
                        continue;
                    }

                    NC.App.Pest.logger.TraceEvent(LogLevels.Verbose, 34030, "Updating {0},{1} {2}", detname, mat, md.Item2.GetType().Name);
                    DB.ElementList parms = null;
                    switch (md.Item1)
                    {
                        case AnalysisMethod.KnownA:

                        case AnalysisMethod.CalibrationCurve:
                        case AnalysisMethod.KnownM:
                        case AnalysisMethod.Multiplicity:
                        case AnalysisMethod.TruncatedMultiplicity:
                        case AnalysisMethod.AddASource:
                        case AnalysisMethod.CuriumRatio:
                        case AnalysisMethod.Active:
                        case AnalysisMethod.ActivePassive:
                        case AnalysisMethod.Collar:
                        case AnalysisMethod.ActiveMultiplicity:
                            parms = ((ParameterBase)md.Item2).ToDBElementList();
                            break;
                        default:
                            break;
                    }
                    if (parms != null)
                        db.UpdateCalib(detname, mat, md.Item2.GetType().Name, parms);  // det, mat, amid, params
                    //Something amiss and sometimes not storing. Could this be it?
                    else
                    {
                        //Didn't exist, so create and store. hn 9.22.2015
                        sam.AddMethod(md.Item1, md.Item2);
                    }
                }
            }
        }


        /// <summary>
        /// Get specific parameter sets for the given detector, material type pair.
        /// Returns default values if database entry not found
        /// </summary>
        /// <param name="detname"></param>
        /// <param name="mat"></param>
        /// <param name="db"></param>
        public void IngestAnalysisMethodSpecificsFromDB(INCCSelector sel, AnalysisMethods ams, DB.AnalysisMethodSpecifiers db)
        {

            foreach (AnalysisMethod am in System.Enum.GetValues(typeof(AnalysisMethod)))
            {
                if (!ams.choices[(int)am])
                    continue;
                if (!(am > AnalysisMethod.None && am <= AnalysisMethod.TruncatedMultiplicity && (am != AnalysisMethod.INCCNone)))
                {
                    if (!am.IsNone())
                        NC.App.Pest.logger.TraceEvent(LogLevels.Warning, 34061, "Skipping DB ingest of {0} {1} calib params", sel, am);
                    continue;
                }
                string current = String.Format("{0} {1} parameters", sel, am.FullName());
                int logid = 34170 + (int)am;
                LogLevels lvl = LogLevels.Verbose;
                DataRow dr;
                switch (am)
                {
                    case AnalysisMethod.KnownA:
                        INCCAnalysisParams.known_alpha_rec ks = new INCCAnalysisParams.known_alpha_rec();
                        dr = db.Get(sel.detectorid, sel.material, "known_alpha_rec");
                        if (dr != null)
                        {
                            ks.rho_zero = DB.Utils.DBDouble(dr["rho_zero"]);
                            ks.alpha_wt = DB.Utils.DBDouble(dr["alpha_wt"]);
                            ks.k = DB.Utils.DBDouble(dr["k"]);
                            ks.cev.a = DB.Utils.DBDouble(dr["a"]);
                            ks.cev.b = DB.Utils.DBDouble(dr["b"]);
                            ks.cev.var_a = DB.Utils.DBDouble(dr["var_a"]);
                            ks.cev.var_b = DB.Utils.DBDouble(dr["var_b"]);
                            ks.cev.setcovar(Coeff.a, Coeff.b, DB.Utils.DBDouble(dr["covar_ab"]));
                            ks.cev.sigma_x = DB.Utils.DBDouble(dr["sigma_x"]);
                            ks.known_alpha_type = (INCCAnalysisParams.KnownAlphaVariant)(DB.Utils.DBInt32(dr["known_alpha_type"]));
                            ks.ring_ratio.cal_curve_equation = (INCCAnalysisParams.CurveEquation)(DB.Utils.DBInt32(dr["ring_ratio_equation"]));
                            ks.ring_ratio.a = DB.Utils.DBDouble(dr["ring_ratio_a"]);
                            ks.ring_ratio.b = DB.Utils.DBDouble(dr["ring_ratio_b"]);
                            ks.ring_ratio.c = DB.Utils.DBDouble(dr["ring_ratio_c"]);
                            ks.ring_ratio.d = DB.Utils.DBDouble(dr["ring_ratio_d"]);
                            ks.dcl_mass = DB.Utils.ReifyDoubles((string)dr["dcl_mass"]);
                            ks.doubles = DB.Utils.ReifyDoubles((string)dr["doubles"]);
                            ks.heavy_metal_reference = DB.Utils.DBDouble(dr["heavy_metal_reference"]);
                            ks.heavy_metal_corr_factor = DB.Utils.DBDouble(dr["heavy_metal_corr_factor"]);
                            ks.cev.upper_mass_limit = DB.Utils.DBDouble(dr["upper_mass_limit"]);
                            ks.cev.lower_mass_limit = DB.Utils.DBDouble(dr["lower_mass_limit"]);
                        }
                        else
                            lvl = LogLevels.Info;
                        ams.AddMethod(am, ks);
                        break;
                    case AnalysisMethod.CalibrationCurve:
                        INCCAnalysisParams.cal_curve_rec cs = new INCCAnalysisParams.cal_curve_rec();
                        dr = db.Get(sel.detectorid, sel.material, "cal_curve_rec");
                        if (dr != null)
                        {
                            CalCurveDBSnock(cs.cev, dr);
                            cs.CalCurveType = (INCCAnalysisParams.CalCurveType)DB.Utils.DBInt32(dr["cal_curve_type"]);
                            cs.dcl_mass = DB.Utils.ReifyDoubles((string)dr["dcl_mass"]);
                            cs.doubles = DB.Utils.ReifyDoubles((string)dr["doubles"]);
                            cs.percent_u235 = DB.Utils.DBDouble(dr["percent_u235"]);
                            cs.heavy_metal_reference = DB.Utils.DBDouble(dr["heavy_metal_reference"]);
                            cs.heavy_metal_corr_factor = DB.Utils.DBDouble(dr["heavy_metal_corr_factor"]);
                        }
                        else
                            lvl = LogLevels.Info;
                        ams.AddMethod(am, cs);
                        break;
                    case AnalysisMethod.KnownM:
                        INCCAnalysisParams.known_m_rec ms = new INCCAnalysisParams.known_m_rec();
                        dr = db.Get(sel.detectorid, sel.material, "known_m_rec");
                        if (dr != null)
                        {
                            ms.sf_rate = DB.Utils.DBDouble(dr["sf_rate"]);
                            ms.vs1 = DB.Utils.DBDouble(dr["vs1"]);
                            ms.vs2 = DB.Utils.DBDouble(dr["vs2"]);
                            ms.vi1 = DB.Utils.DBDouble(dr["vi1"]);
                            ms.vi2 = DB.Utils.DBDouble(dr["vi2"]);
                            ms.b = DB.Utils.DBDouble(dr["b"]);
                            ms.c = DB.Utils.DBDouble(dr["c"]);
                            ms.sigma_x = DB.Utils.DBDouble(dr["sigma_x"]);
                            ms.lower_mass_limit = DB.Utils.DBDouble(dr["lower_mass_limit"]);
                            ms.upper_mass_limit = DB.Utils.DBDouble(dr["upper_mass_limit"]);
                        }
                        else
                            lvl = LogLevels.Info;
                        ams.AddMethod(am, ms);
                        break;
                    case AnalysisMethod.Multiplicity:
                        INCCAnalysisParams.multiplicity_rec mu = new INCCAnalysisParams.multiplicity_rec();
                        dr = db.Get(sel.detectorid, sel.material, "multiplicity_rec");
                        if (dr != null)
                        {
                            mu.solve_efficiency = (INCCAnalysisParams.MultChoice)DB.Utils.DBInt32(dr["solve_efficiency"]);
                            mu.sf_rate = DB.Utils.DBDouble(dr["sf_rate"]);
                            mu.vs1 = DB.Utils.DBDouble(dr["vs1"]);
                            mu.vs2 = DB.Utils.DBDouble(dr["vs2"]);
                            mu.vs3 = DB.Utils.DBDouble(dr["vs3"]);
                            mu.vi1 = DB.Utils.DBDouble(dr["vi1"]);
                            mu.vi2 = DB.Utils.DBDouble(dr["vi2"]);
                            mu.vi3 = DB.Utils.DBDouble(dr["vi3"]);
                            mu.a = DB.Utils.DBDouble(dr["a"]);
                            mu.b = DB.Utils.DBDouble(dr["b"]);
                            mu.c = DB.Utils.DBDouble(dr["c"]);
                            mu.sigma_x = DB.Utils.DBDouble(dr["sigma_x"]);
                            mu.alpha_weight = DB.Utils.DBDouble(dr["alpha_weight"]);
                            mu.multEffCorFactor = DB.Utils.DBDouble(dr["eff_cor"]);
                        }
                        else
                            lvl = LogLevels.Info;
                        ams.AddMethod(am, mu);
                        break;
                    case AnalysisMethod.TruncatedMultiplicity:
                        INCCAnalysisParams.truncated_mult_rec tm = new INCCAnalysisParams.truncated_mult_rec();
                        dr = db.Get(sel.detectorid, sel.material, "truncated_mult_rec");
                        if (dr != null)
                        {
                            tm.known_eff = DB.Utils.DBBool(dr["known_eff"]);
                            tm.solve_eff = DB.Utils.DBBool(dr["vs1"]);
                            tm.a = DB.Utils.DBDouble(dr["a"]);
                            tm.b = DB.Utils.DBDouble(dr["b"]);
                        }
                        else
                            lvl = LogLevels.Info;
                        ams.AddMethod(am, tm);
                        break;
                    case AnalysisMethod.CuriumRatio:
                        INCCAnalysisParams.curium_ratio_rec cm = new INCCAnalysisParams.curium_ratio_rec();
                        dr = db.Get(sel.detectorid, sel.material, "curium_ratio_rec");
                        if (dr != null)
                        {
                            cm.curium_ratio_type = (INCCAnalysisParams.CuriumRatioVariant)DB.Utils.DBInt32(dr["curium_ratio_type"]);
                            CalCurveDBSnock(cm.cev, dr);
                        }
                        else
                            lvl = LogLevels.Info;
                        ams.AddMethod(am, cm);
                        break;
                    case AnalysisMethod.Active:
                        INCCAnalysisParams.active_rec ar = new INCCAnalysisParams.active_rec();
                        dr = db.Get(sel.detectorid, sel.material, "active_rec");
                        if (dr != null)
                        {
                            CalCurveDBSnock(ar.cev, dr);
                            ar.dcl_mass = DB.Utils.ReifyDoubles((string)dr["dcl_mass"]);
                            ar.doubles = DB.Utils.ReifyDoubles((string)dr["doubles"]);
                        }
                        else
                            lvl = LogLevels.Warning;
                        ams.AddMethod(am, ar);
                        break;
                    case AnalysisMethod.AddASource:
                        INCCAnalysisParams.add_a_source_rec aas = new INCCAnalysisParams.add_a_source_rec();
                        dr = db.Get(sel.detectorid, sel.material, "add_a_source_rec");
                        if (dr != null)
                        {
                            CalCurveDBSnock(aas.cev, dr);
                            aas.dcl_mass = DB.Utils.ReifyDoubles((string)dr["dcl_mass"]);
                            aas.doubles = DB.Utils.ReifyDoubles((string)dr["doubles"]);

                            aas.cf.a = DB.Utils.DBDouble(dr["cf_a"]);
                            aas.cf.b = DB.Utils.DBDouble(dr["cf_b"]);
                            aas.cf.c = DB.Utils.DBDouble(dr["cf_c"]);
                            aas.cf.d = DB.Utils.DBDouble(dr["cf_d"]);

                            aas.dzero_avg = DB.Utils.DBDouble(dr["dzero_avg"]);
                            aas.num_runs = DB.Utils.DBUInt16(dr["num_runs"]);
                            aas.position_dzero = DB.Utils.ReifyDoubles((string)dr["position_dzero"]);
                            aas.dzero_ref_date = DB.Utils.DBDateTime(dr["dzero_ref_date"]);
                            aas.use_truncated_mult = DB.Utils.DBBool(dr["use_truncated_mult"]);
                            aas.tm_dbls_rate_upper_limit = DB.Utils.DBDouble(dr["tm_dbls_rate_upper_limit"]);
                            aas.tm_weighting_factor = DB.Utils.DBDouble(dr["tm_weighting_factor"]);
                        }
                        else
                            lvl = LogLevels.Info;
                        ams.AddMethod(am, aas);
                        break;
                    case AnalysisMethod.ActiveMultiplicity:
                        INCCAnalysisParams.active_mult_rec amr = new INCCAnalysisParams.active_mult_rec();
                        dr = db.Get(sel.detectorid, sel.material, "active_mult_rec");
                        if (dr != null)
                        {
                            amr.vf1 = DB.Utils.DBDouble(dr["vf1"]);
                            amr.vf2 = DB.Utils.DBDouble(dr["vf2"]);
                            amr.vf3 = DB.Utils.DBDouble(dr["vf3"]);
                            amr.vt1 = DB.Utils.DBDouble(dr["vt1"]);
                            amr.vt2 = DB.Utils.DBDouble(dr["vt2"]);
                            amr.vt3 = DB.Utils.DBDouble(dr["vt3"]);
                        }
                        else
                            lvl = LogLevels.Info;
                        ams.AddMethod(am, amr);
                        break;
                    case AnalysisMethod.ActivePassive:
                        INCCAnalysisParams.active_passive_rec acp = new INCCAnalysisParams.active_passive_rec();
                        dr = db.Get(sel.detectorid, sel.material, "active_passive_rec");
                        if (dr != null)
                        {
                            CalCurveDBSnock(acp.cev, dr);
                        }
                        else
                            lvl = LogLevels.Info;
                        ams.AddMethod(am, acp);
                        break;
                    case AnalysisMethod.Collar:
                        INCCAnalysisParams.collar_rec cr = new INCCAnalysisParams.collar_rec();
                        dr = db.Get(sel.detectorid, sel.material, "collar_rec");
                        if (dr != null)
                        {
                            CalCurveDBSnock(cr.cev, dr);
                            cr.collar_mode = DB.Utils.DBBool(dr["collar_mode"]);
                            cr.number_calib_rods = DB.Utils.DBInt32(dr["number_calib_rods"]);
                            cr.sample_corr_fact.v = DB.Utils.DBDouble(dr["sample_corr_fact"]);
                            cr.sample_corr_fact.err = DB.Utils.DBDouble(dr["sample_corr_fact_err"]);
                            cr.u_mass_corr_fact_a.v = DB.Utils.DBDouble(dr["u_mass_corr_fact_a"]);
                            cr.u_mass_corr_fact_a.err = DB.Utils.DBDouble(dr["u_mass_corr_fact_a_err"]);
                            cr.u_mass_corr_fact_b.v = DB.Utils.DBDouble(dr["u_mass_corr_fact_b"]);
                            cr.u_mass_corr_fact_b.err = DB.Utils.DBDouble(dr["u_mass_corr_fact_b_err"]);
                            cr.poison_absorption_fact = DB.Utils.ReifyDoubles((string)dr["poison_absorption_fact"]);
                            cr.poison_rod_type = DB.Utils.ReifyStrings((string)dr["poison_rod_type"]);

                            cr.poison_rod_a = TupleArraySlurp(ref cr.poison_rod_a, "poison_rod_a", dr);
                            cr.poison_rod_b = TupleArraySlurp(ref cr.poison_rod_b, "poison_rod_b", dr);
                            cr.poison_rod_c = TupleArraySlurp(ref cr.poison_rod_c, "poison_rod_c", dr);
                        }
                        else
                            lvl = LogLevels.Info;
                        ams.AddMethod(am, cr);
                        break;
                    default:
                        lvl = LogLevels.Error; logid = 34181; current = "Choosing to not construct" + current;
                        break;
                }
                switch (lvl)
                {
                    case LogLevels.Info:
                        current = "Using default for " + current;
                        lvl = LogLevels.Verbose;
                        break;
                    case LogLevels.Verbose:
                        current = "Retrieved for " + current;
                        break;
                    default:
                        break;
                }
                NC.App.Pest.logger.TraceEvent(lvl, logid, current);
            } // for

        }

        static VTuple[] TupleArraySlurp(ref VTuple[] dest, string field, DataRow dr)
        {
            double[] v = DB.Utils.ReifyDoubles((string)dr[field]);
            double[] err = DB.Utils.ReifyDoubles((string)dr[field+"_err"]);

            for (int i = 0; i < dest.Length; i++)
                dest[i] = new VTuple(v[i], err[i]);
            return dest;
        }

        void CalCurveDBSnock(INCCAnalysisParams.CurveEquationVals cev, DataRow dr)
        {
            
            if (dr == null) return;
            cev.cal_curve_equation = (INCCAnalysisParams.CurveEquation)(DB.Utils.DBInt32(dr["cal_curve_equation"]));
            cev.a = DB.Utils.DBDouble(dr["a"]);
            cev.b = DB.Utils.DBDouble(dr["b"]);
            cev.c = DB.Utils.DBDouble(dr["c"]);
            cev.d = DB.Utils.DBDouble(dr["d"]);
            cev.var_a = DB.Utils.DBDouble(dr["var_a"]);
            cev.var_b = DB.Utils.DBDouble(dr["var_b"]);
            cev.var_c = DB.Utils.DBDouble(dr["var_c"]);
            cev.var_d = DB.Utils.DBDouble(dr["var_d"]);
            cev.setcovar(Coeff.a, Coeff.b, DB.Utils.DBDouble(dr["covar_ab"]));
            cev.setcovar(Coeff.a, Coeff.c, DB.Utils.DBDouble(dr["covar_ac"]));
            cev.setcovar(Coeff.a, Coeff.d, DB.Utils.DBDouble(dr["covar_ad"]));
            cev.setcovar(Coeff.b, Coeff.c, DB.Utils.DBDouble(dr["covar_bc"]));
            cev.setcovar(Coeff.b, Coeff.d, DB.Utils.DBDouble(dr["covar_bd"]));
            cev.setcovar(Coeff.c, Coeff.d, DB.Utils.DBDouble(dr["covar_cd"]));
            cev.sigma_x = DB.Utils.DBDouble(dr["sigma_x"]);
            cev.upper_mass_limit = DB.Utils.DBDouble(dr["upper_mass_limit"]);
            cev.lower_mass_limit = DB.Utils.DBDouble(dr["lower_mass_limit"]);
        }


#region results
        // stub, TBD
        public void UpdateAnalysisMethodResults(string detname, string mat, DB.AnalysisMethodSpecifiers db = null)
        {
            if (db == null)
                db = new DB.AnalysisMethodSpecifiers();
            var res =   // this finds the am for the given detector and acquire type
                    from am in this.DetectorMaterialAnalysisMethods
                    where (am.Key.detectorid.Equals(detname, StringComparison.OrdinalIgnoreCase) && 
                           am.Key.material.Equals(mat, StringComparison.OrdinalIgnoreCase))
                    select am;
            if (res.Count() > 0)  // now execute the select expression and test the result for existence
            {
                KeyValuePair<INCCSelector, AnalysisMethods> kv = res.First();
                AnalysisMethods sam = kv.Value;  // the descriptor instance

                IEnumerator iter = kv.Value.GetMethodEnumerator();
                while (iter.MoveNext())
                {
                    System.Tuple<AnalysisMethod, INCCAnalysisParams.INCCMethodDescriptor> md = (System.Tuple<AnalysisMethod, INCCAnalysisParams.INCCMethodDescriptor>)iter.Current;
                    if (md.Item2 == null) // case from INCC5 transfer missing params, reflects file write bugs in INCC5 code
                    {
                        NC.App.Pest.logger.TraceEvent(LogLevels.Warning, 34029, "Missing {0}'s INCC {1} method parameters, skipping to next entry", detname, md.Item1.ToString());
                        continue;
                    }

                    NC.App.Pest.logger.TraceEvent(LogLevels.Verbose, 34030, "Updating <{0},{1}>: {2}", detname, mat, md.Item2.GetType().Name);

                   DB.ElementList parms = null;
                    switch (md.Item1)
                    {
                        case AnalysisMethod.KnownA:
                        case AnalysisMethod.CalibrationCurve:
                        case AnalysisMethod.KnownM:
                        case AnalysisMethod.Multiplicity:
                        case AnalysisMethod.TruncatedMultiplicity:
                        case AnalysisMethod.AddASource:
                        case AnalysisMethod.CuriumRatio:
                        case AnalysisMethod.Active:
                        case AnalysisMethod.ActivePassive:
                        case AnalysisMethod.Collar:
                        case AnalysisMethod.ActiveMultiplicity:
                            parms = ((ParameterBase)md.Item2).ToDBElementList();
                            break;
                        default:
                            break;
                    }
                    if (parms != null)
                        db.UpdateCalib(detname, mat, md.Item2.GetType().Name, parms);  // det, mat, amid, params
                }
            }
        }

#endregion results

    }
}

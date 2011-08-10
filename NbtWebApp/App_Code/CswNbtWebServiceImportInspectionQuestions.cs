using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Exceptions;
using System.Xml.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web.Services;
using System.Xml;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Config;
using ChemSW.Nbt.PropTypes;
using ChemSW.NbtWebControls;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceImportInspectionQuestions
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceImportInspectionQuestions(CswNbtResources CswNbtResources)
        {
            _CswNbtResources = CswNbtResources;
        }

        private enum ImportColumns
        {
            Section,
            Question,
            Allowed_Answers,
            Compliant_Answers,
            Help_Text
        }

        public string GetExcelTemplate()
        {
            CswDelimitedString CSVTemplate = new CswDelimitedString('\t');
            foreach (ImportColumns Col in Enum.GetValues(typeof(ImportColumns)))
            {
                CSVTemplate.Add(ImportColumnsToDisplayString(Col));
            }

            return CSVTemplate.ToString();
        }

        private static string ImportColumnsToDisplayString(ImportColumns Column)
        {
            return Column.ToString().Replace('_', ' ');
        }

        private static ImportColumns ImportColumnsFromDisplayString(string Column)
        {
            return (ImportColumns)Enum.Parse(typeof(ImportColumns), Column.Replace(' ', '_'), true);
        }
    }

}
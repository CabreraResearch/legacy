using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ImportExport
{

    public class CswNbtImportOptions
    {
        public CswNbtImportOptions()
        {
        }//ctor


        public ImportStartPoint StartPoint = ImportStartPoint.NukeAndStartOver;
        public ImportProcessPhase ProcessPhase = ImportProcessPhase.NothingDoneYet;
        public AbsentImportNodeHandling AbsentNodeHandling = AbsentImportNodeHandling.DeduceAndCreate;

        public Int32 MaxInsertRecordsPerTransaction = 500;
        public Int32 MaxInsertRecordsPerDisplayUpdate = 1000;

        public Int32 NodeCreatePageSize = 25; //number of nodes to create per cycle
        public Int32 NodeAddPropsPageSize = 25; //Number of nodes to create properties for per cycle

        public string NameOfDefaultRoleForUserNodes = "Equipment User";


    }//class CswNbtImportOptions

}//namespace ChemSW.Nbt.ImportExport

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.DB;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.ImportExport
{


    public class CswImporterFactory
    {

        public static ICswImporter make( ImportAlgorithm ImportAlgorithm, CswNbtResources CswNbtResources, CswNbtImportExportFrame CswNbtImportExportFrame, StatusUpdateHandler OnStatusUpdate )
        {
            ICswImporter ReturnVal = null;

            if( ImportAlgorithm.Legacy == ImportAlgorithm )
            {
                ReturnVal = new CswImporterLegacy( CswNbtResources, CswNbtImportExportFrame, OnStatusUpdate );
            }

            return ( ReturnVal );
        }


    } // ICswImporter
} // namespace ChemSW.Nbt

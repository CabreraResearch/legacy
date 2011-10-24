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

        public static ICswImporter make( ImportAlgorithm ImportAlgorithm, CswNbtResources CswNbtResources, CswNbtImportExportFrame CswNbtImportExportFrame, StatusUpdateHandler OnStatusUpdate, ImportPhaseHandler OnImportPhaseChange )
        {
            ICswImporter ReturnVal = null;


            switch( ImportAlgorithm )
            {

                case ImportAlgorithm.Legacy:
                    ReturnVal = new CswImporterLegacy( CswNbtResources, CswNbtImportExportFrame, OnStatusUpdate );
                    break;

                case ImportAlgorithm.Experimental:
                    ReturnVal = new CswImporterExperimental( CswNbtResources, CswNbtImportExportFrame, new CswImportExportStatusReporter( OnStatusUpdate, OnImportPhaseChange ,CswNbtResources.CswLogger ) );
                    break;

                default:
                    throw ( new CswDniException( "Unknown Import altorithm: " + ImportAlgorithm.ToString() ) );
            }//switch

            return ( ReturnVal );
        }


    } // ICswImporter
} // namespace ChemSW.Nbt

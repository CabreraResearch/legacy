using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Log;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.DB;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.ImportExport
{


    public class CswImportExportStatusReporter
    {

        private StatusUpdateHandler _WriteToGui = null;
        private ICswLogger _CswLogger = null;


        public CswImportExportStatusReporter( StatusUpdateHandler StatusUpdateHandler, ICswLogger CswLogger )
        {
            _WriteToGui = StatusUpdateHandler;
            _CswLogger = CswLogger;
        }//ctor

        public void reportException( Exception Exception )
        {
            _CswLogger.reportError( Exception );

            _WriteToGui( Exception.ToString() );
        }//reportException()

        public void reportError( string ErrorMessage )
        {
            _CswLogger.reportError( new CswDniException( ErrorType.Error, ErrorMessage, null ) );
            _WriteToGui( ErrorMessage );
        }//reportError()

        public void reportStatus( string StatusMessage )
        {
            _CswLogger.reportAppState( StatusMessage );
            _WriteToGui( StatusMessage );
        }//reportStatus()

    } // class CswImportExportStatusReporter
} // namespace ChemSW.Nbt

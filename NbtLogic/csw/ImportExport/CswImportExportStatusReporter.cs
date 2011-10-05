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

    public enum ImportExportMessageType { Progress, Stat }
    public class CswImportExportStatusReporter
    {

        public List<ImportExportMessageType> MessageTypesToBeLogged = new List<ImportExportMessageType>();
        private StatusUpdateHandler _WriteToGui = null;
        private ICswLogger _CswLogger = null;

        private string _ImportExportLogFilter = "importexport";

        public CswImportExportStatusReporter( StatusUpdateHandler StatusUpdateHandler, ICswLogger CswLogger )
        {
            MessageTypesToBeLogged.Add( ImportExportMessageType.Progress );
            _WriteToGui = StatusUpdateHandler;
            _CswLogger = CswLogger;
            _CswLogger.addFilter( _ImportExportLogFilter );
            _CswLogger.RestrictByFilter = true;
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

        public void reportProgress( string StatusMessage )
        {
            if( MessageTypesToBeLogged.Contains( ImportExportMessageType.Stat ) )
            {
                _CswLogger.reportAppState( StatusMessage, _ImportExportLogFilter );
                _WriteToGui( StatusMessage );
            }
        }//reportStatus()

        public void reportTiming( CswTimer CswTimer, string Action )
        {
            if( MessageTypesToBeLogged.Contains( ImportExportMessageType.Stat ) )
            {
                _CswLogger.reportAppState( "Total time to " + Action + ": " + CswTimer.ElapsedDurationInMilliseconds.ToString() + " ms" );
            }
        }//

    } // class CswImportExportStatusReporter
} // namespace ChemSW.Nbt

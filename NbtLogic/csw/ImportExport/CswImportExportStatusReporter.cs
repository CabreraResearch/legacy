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

    public enum ImportExportMessageType { Progress, Timing, Error }
    public class CswImportExportStatusReporter
    {

        public List<ImportExportMessageType> MessageTypesToBeLogged = new List<ImportExportMessageType>();
        private StatusUpdateHandler _WriteToGui = null;
        private ImportPhaseHandler _ReportPhaseChange = null;
        private CswNbtResources _CswNbtResources = null; 

        private string _ImportExportLogFilter = "importexport";

        public CswImportExportStatusReporter( StatusUpdateHandler StatusUpdateHandler, ImportPhaseHandler ImportPhaseHandler, CswNbtResources CswNbtResources )
        {
            MessageTypesToBeLogged.Add( ImportExportMessageType.Progress );
            _WriteToGui = StatusUpdateHandler;
            _ReportPhaseChange = ImportPhaseHandler;
            _CswNbtResources = CswNbtResources;
            
            _CswNbtResources.CswLogger.addFilter( _ImportExportLogFilter );
            _CswNbtResources.CswLogger.RestrictByFilter = true;
        }//ctor

        public void reportException( Exception Exception )
        {
            if( MessageTypesToBeLogged.Contains( ImportExportMessageType.Error ) )
            {
                _CswNbtResources.CswLogger.reportError( Exception );
            }

            _WriteToGui( Exception.ToString() );
        }//reportException()

        public void reportError( string ErrorMessage )
        {
            if( MessageTypesToBeLogged.Contains( ImportExportMessageType.Error ) )
            {
                _CswNbtResources.CswLogger.reportError( new CswDniException( ErrorType.Error, ErrorMessage, null ) );
            }

            _WriteToGui( ErrorMessage );
        }//reportError()

        public void reportProgress( string StatusMessage )
        {
            if( MessageTypesToBeLogged.Contains( ImportExportMessageType.Timing ) )
            {
                _CswNbtResources.CswLogger.reportAppState( StatusMessage, _ImportExportLogFilter );
                _WriteToGui( StatusMessage );
            }
        }//reportProgress()


        public void updateProcessPhase( ImportProcessPhase ProcessPhase, Int32 TotalObjects, Int32 ObjectsSofar, ProcessStates ProcessState = ProcessStates.InProcess )
        {
            _ReportPhaseChange( new CswNbtImportStatus(_CswNbtResources, ProcessPhase, TotalObjects, ObjectsSofar, ProcessState ) );
        }

        public void reportTiming( CswTimer CswTimer, string Action )
        {
            if( MessageTypesToBeLogged.Contains( ImportExportMessageType.Timing ) )
            {
                _CswNbtResources.CswLogger.reportAppState( "Total time to " + Action + ": " + CswTimer.ElapsedDurationInMilliseconds.ToString() + " ms", _ImportExportLogFilter );
            }
        }//

    } // class CswImportExportStatusReporter
} // namespace ChemSW.Nbt

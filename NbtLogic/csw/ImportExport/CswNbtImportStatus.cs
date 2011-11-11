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

    public enum ImportProcessPhase { NothingDoneYet, LoadingInputFile, PopulatingTempTableNodes, PopulatingTempTableProps, PopulatingNbtNodes, VerifyingNbtTargetNodes, CreatingMissingNbtTargetNodes, PopulatingNbtProps, PostProcessingNbtNodes, Completed };
    public enum ImportProcessStati { Unprocessed, Imported, PropsAdded, RedundancyChecked, Error };
    public enum ImportSource { ImportData, Deduced }
    public enum PhaseTypes { Incremental, Monolithic }
    public enum ProcessStates { Unknown, InProcess, Complete }
    public class CswNbtImportStatus
    {

        private Int32 _ObjectsTotal = 0;
        private Int32 _ObjectsCompletedSofar = 0;
        CswNbtResources _CswNbtResources = null;


        CswNbtPersistedImportState _CswNbtPersistedImportState = null;
        public PhaseTypes PhaseType { get { return ( ( ( _ObjectsTotal > 0 ) && ( _ObjectsCompletedSofar > 0 ) ) ? PhaseTypes.Incremental : PhaseTypes.Monolithic ); } }
        public CswNbtImportStatus( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtPersistedImportState = new CswNbtPersistedImportState( CswNbtResources );
        }//ctor


        public void reset()
        {
            _ObjectsTotal = 0;
            _ObjectsCompletedSofar = 0;

            ProcessState = ProcessStates.Unknown;
            _TargetProcessPhase = ImportProcessPhase.NothingDoneYet;

            _CswNbtPersistedImportState.reset(); 
        }

        public void setStatus( ImportProcessPhase TargetProcessPhase, Int32 TotalObjects, Int32 ObjectsSofar, ProcessStates ProcessStateIn )
        {
            _TargetProcessPhase = TargetProcessPhase;
            _ObjectsTotal = TotalObjects;
            _ObjectsCompletedSofar = ObjectsSofar;
            ProcessState = ProcessStateIn;

            if( ProcessStates.Complete == ProcessStateIn )
            {
                _CswNbtPersistedImportState.CompletedProcessPhase = _TargetProcessPhase;
            }

        }//setStatus() 

        public ProcessStates ProcessState = ProcessStates.InProcess;
        private ImportProcessPhase _TargetProcessPhase = ImportProcessPhase.NothingDoneYet;
        public ImportProcessPhase TargetProcessPhase { get { return ( _TargetProcessPhase ); } }

        public string TargetPhaseDescription
        {
            get
            {
                return ( _makePhaseDescription( _TargetProcessPhase ) );

            }//get

        }//ProcessPhase


        public string CompletedPhaseDescription
        {
            get
            {
                return ( _makePhaseDescription( CompletedProcessPhase ) );
            }//get

        }//ProcessPhase

        private string _makePhaseDescription( ImportProcessPhase ImportProcessPhase )
        {
            return ( Regex.Replace( ImportProcessPhase.ToString(), "([A-Z])", " $1" ) );
        }

        public ImportProcessPhase CompletedProcessPhase
        {

            set
            {
                _CswNbtPersistedImportState.CompletedProcessPhase = value;
            }//

            get
            {
                return ( _CswNbtPersistedImportState.CompletedProcessPhase );
            }
        }//


        public ImportMode Mode
        {

            set
            {
                _CswNbtPersistedImportState.Mode = value;
            }


            get
            {
                return ( _CswNbtPersistedImportState.Mode );
            }

        }//Mode

        //public string FilePath
        //{
        //    set
        //    {
        //        _CswNbtPersistedImportState.FilePath = value;
        //    }

        //    get
        //    {
        //        return ( _CswNbtPersistedImportState.FilePath );
        //    }

        //}

        public string PhaseStatus
        {
            get
            {
                string ReturnVal = string.Empty;

                switch( _TargetProcessPhase )
                {


                    case ImportProcessPhase.LoadingInputFile:
                        ReturnVal = "Loading import file";
                        break;

                    case ImportProcessPhase.PopulatingTempTableNodes:
                        ReturnVal = _count + " node records inserted in temporary table";
                        break;

                    case ImportProcessPhase.PopulatingTempTableProps:
                        ReturnVal = _count + " property records inserted in temporary table";
                        break;

                    case ImportProcessPhase.PopulatingNbtNodes:
                        ReturnVal = _count + " NBT nodes created";
                        break;

                    case ImportProcessPhase.VerifyingNbtTargetNodes:
                        ReturnVal = _count + " target nodes verified";
                        break;

                    case ImportProcessPhase.CreatingMissingNbtTargetNodes:
                        ReturnVal = _count + " target nodes created";
                        break;

                    case ImportProcessPhase.PopulatingNbtProps:
                        ReturnVal = _count + " NBT properties nodes created";
                        break;

                    case ImportProcessPhase.PostProcessingNbtNodes:
                        ReturnVal = _count + " nodes post-processed";
                        break;

                    case ImportProcessPhase.Completed:
                        ReturnVal = "Import is complete. congratulations!";
                        break;

                    default:
                        ReturnVal = "Unknown process phase";
                        break;
                }//switch

                return ( ReturnVal );

            }//get

        }//Objects_Name



        private string _count
        {
            get
            {
                return ( _ObjectsCompletedSofar.ToString() + " of " + _ObjectsTotal );
            }
        }//_count


    }//class CswNbtImportStatus

}//namespace ChemSW.Nbt.ImportExport

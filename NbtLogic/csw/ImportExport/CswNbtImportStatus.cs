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

    public enum ImportProcessPhase { NothingDoneYet, LoadingInputFile, PopulatingTempTableNodes, PopulatingTempTableProps, PopulatingNbtNodes, VerifyingNbtTargetNodes, CreatingMissingNbtTargetNodes, PopulatingNbtProps, PostProcessingNbtNodes };
    public enum ImportProcessStati { Unprocessed, Imported, PropsAdded, RedundancyChecked, Error };
    public enum ImportSource { ImportData, Deduced }
    public enum PhaseTypes { Incremental, Monolithic }
    public enum ProcessStates { InProcess, Complete }
    public class CswNbtImportStatus
    {

        private Int32 _ObjectsTotal = 0;
        private Int32 _ObjectsCompletedSofar = 0;
        CswNbtResources _CswNbtResources = null;

        public PhaseTypes PhaseType { get { return ( ( ( _ObjectsTotal > 0 ) && ( _ObjectsCompletedSofar > 0 ) ) ? PhaseTypes.Incremental : PhaseTypes.Monolithic ); } }
        public CswNbtImportStatus( CswNbtResources CswNbtResources, ImportProcessPhase TargetProcessPhase, Int32 TotalObjects, Int32 ObjectsSofar, ProcessStates ProcessStateIn )
        {
            _TargetProcessPhase = TargetProcessPhase;
            _ObjectsTotal = TotalObjects;
            _ObjectsCompletedSofar = ObjectsSofar;
            ProcessState = ProcessStateIn;
            _CswNbtResources = CswNbtResources;

        }//ctor



        public ProcessStates ProcessState = ProcessStates.InProcess;
        private ImportProcessPhase _TargetProcessPhase = ImportProcessPhase.NothingDoneYet;
        public ImportProcessPhase TargetProcessPhase { get { return ( _TargetProcessPhase ); } }

        public string PhaseDescription
        {
            get
            {
                string ReturnVal = Regex.Replace( _TargetProcessPhase.ToString(), "([A-Z])", " $1" );

                return ( ReturnVal );
            }//get

        }//ProcessPhase

        public ImportProcessPhase PreviousProcessPhase
        {
            
            set
            {
                // fsif( false == _CswNbtResources.ConfigVariables
            }//

            /*
            get
            {
            }
             */
        }//

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

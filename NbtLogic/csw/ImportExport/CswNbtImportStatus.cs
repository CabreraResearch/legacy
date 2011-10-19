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

    public class CswNbtImportStatus
    {
        public enum PhaseTypes { Incremental, Monolithic }
        public enum ProcessStates { InProcess, Complete }
        private Int32 _ObjectsTotal = 0;
        private Int32 _ObjectsCompletedSofar = 0;

        public PhaseTypes PhaseType { get { return ( ( ( _ObjectsTotal > 0 ) && ( _ObjectsCompletedSofar > 0 ) ) ? PhaseTypes.Incremental : PhaseTypes.Monolithic ); } }
        public CswNbtImportStatus( ProcessPhase TargetProcessPhase, Int32 TotalObjects, Int32 ObjectsSofar, ProcessStates ProcessStateIn )
        {
            _TargetProcessPhase = TargetProcessPhase;
            _ObjectsTotal = TotalObjects;
            _ObjectsCompletedSofar = ObjectsSofar;
            ProcessState = ProcessStateIn;

        }//ctor

        public ProcessStates ProcessState = ProcessStates.InProcess;
        private ProcessPhase _TargetProcessPhase = ProcessPhase.NothingDoneYet;
        public ProcessPhase ProcessPhase { get { return ( _TargetProcessPhase ); } }

        public string PhaseDescription
        {
            get
            {
                string ReturnVal = Regex.Replace( _TargetProcessPhase.ToString(), "([A-Z])", " $1" );

                return ( ReturnVal );
            }//get

        }//ProcessPhase



        public string PhaseStatus
        {
            get
            {
                string ReturnVal = string.Empty;

                switch( _TargetProcessPhase )
                {


                    case ProcessPhase.LoadingInputFile:
                        ReturnVal = "Loading import file";
                        break;

                    case ProcessPhase.PopulatingTempTableNodes:
                        ReturnVal = _count + " node records inserted in temporary table";
                        break;

                    case ProcessPhase.PopulatingTempTableProps:
                        ReturnVal = _count + " property records inserted in temporary table";
                        break;

                    case ProcessPhase.PopulatingNbtNodes:
                        ReturnVal = _count + " NBT nodes created";
                        break;

                    case ProcessPhase.VerifyingNbtTargetNodes:
                        ReturnVal = _count + " target nodes verified";
                        break;

                    case ProcessPhase.CreatingMissingNbtTargetNodes:
                        ReturnVal = _count + " target nodes created";
                        break;

                    case ProcessPhase.PopulatingNbtProps:
                        ReturnVal = _count + " NBT properties nodes created";
                        break;

                    case ProcessPhase.PostProcessingNbtNodes:
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

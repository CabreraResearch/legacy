using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using ChemSW.TblDn;

namespace ChemSW.Nbt.Sched
{
    /// <summary>
    /// Update Generator Targets according to Vertical business logic
    /// </summary>
    public class CswNbtSchdItemUpdateInspectionStatus : CswNbtSchdItem
    {
        private CswNbtResources _CswNbtResources = null;
        private CswNbtNode _CswNbtNodeGenerator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CswNbtResources"></param>
        /// <param name="CswNbtNodeGenerator"></param>
        public CswNbtSchdItemUpdateInspectionStatus( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            SchedItemName = "UpdateInspectionStatus";
        }//ctor

        /// <summary>
        /// Reset the schedule item
        /// </summary>
        public override void reset()
        {
            _Succeeded = true;
            _StatusMessage = string.Empty;
            _thisInspectionOverdue = false;
            _anyInspectionOverdue = false;
        }//

        private bool _doesItemRunNow = true;
        /// <summary>
        /// Determines whether the Generator is due for running
        /// </summary>
        /// <remarks>This always runs. Always true.</remarks>
        override public bool doesItemRunNow()
        {
            return _doesItemRunNow;
        }//doesItemRunNow() 

        private bool _thisInspectionOverdue = false;
        private bool _anyInspectionOverdue = false;
        /// <summary>
        /// Mark Inspection status overdue if inspection status is pending and is past due date
        /// </summary>
        override public void run()
        {
            try
            {
                DateTime DueDate = DateTime.MinValue;
                string InspectionStatus = string.Empty;
                string Pending = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Pending );
                string Overdue = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Overdue );

                CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );

                foreach( CswNbtMetaDataNodeType NT in InspectionDesignOC.NodeTypes )
                {
                    if( NT.NodeTypeName == "Physical Inspection" )
                    {
                        foreach( CswNbtNode PINode in NT.getNodes( true, true ) )
                        {
                            InspectionStatus = PINode.Properties[CswNbtObjClassInspectionDesign.StatusPropertyName].AsList.Value;
                            if( InspectionStatus == Pending )
                            {
                                DueDate = PINode.Properties[CswNbtObjClassInspectionDesign.DatePropertyName].AsDate.DateValue;
                                _thisInspectionOverdue = ( DueDate <= DateTime.Today );
                                _anyInspectionOverdue = ( _anyInspectionOverdue || _thisInspectionOverdue );
                            }
                            if( _thisInspectionOverdue )
                                InspectionStatus = Overdue;
                        }
                    }
                    //else: No else for now. Hard coding on Node Type.
                }
            }
            catch( Exception Exception )
            {
                _CswNbtResources.logError( new CswDniException( "Schedule encountered the following exception: " + Exception.Message ) );
            }


        }//run()


        /// <summary>
        /// Name of the schedule
        /// </summary>
        override public string Name
        {
            get
            {
                return "Update Inspection Status";
            }//get

        }//Name

        private bool _Succeeded = true;
        /// <summary>
        /// Success status. Currently always true.
        /// </summary>
        override public bool Succeeded
        {
            get
            {
                return ( _Succeeded );
            }//

        }//Succeeded

        private string _StatusMessage = string.Empty;
        
        /// <summary>
        /// Schedule item status
        /// </summary>
        override public string StatusMessage
        {
            get
            {
                if( _anyInspectionOverdue )
                    _StatusMessage = "Inspections were marked overdue.";
                else
                    _StatusMessage = "No inspections meeting criteria were found.";
                return ( _StatusMessage );
            }//

        }//StatusMessage

    }//CswNbtSchdItemGenerateNode

}//namespace ChemSW.Nbt.Sched

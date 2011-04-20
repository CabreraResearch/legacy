using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Reflection;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Mail;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Sched
{
    /// <summary>
    /// Schedule items for node based tasks. For non-node based tasks, add to _AlwaysRunItems in SchdItemRunner
    /// </summary>
    public class CswNbtSchdItemFactory
    {

        private CswNbtResources _CswNbtResources = null;

        public CswNbtSchdItemFactory( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        public CswNbtSchdItem makeSchdItem( CswNbtNode CswNbtNode )
        {
            CswNbtSchdItem ReturnVal = null;

            // These classes should implement ICswNbtPropertySetScheduler
            switch( CswNbtNode.ObjectClass.ObjectClass )
            {
                case CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass:
                    ReturnVal = new CswNbtSchdItemGenerateNode( _CswNbtResources, CswNbtNode );
                    break;
                case CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass:
                    ReturnVal = new CswNbtSchdItemUpdateInspectionStatus( _CswNbtResources, CswNbtNode );
                    break;
                case CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass:
                    ReturnVal = new CswNbtSchdItemGenerateEmailReport( _CswNbtResources, CswNbtNode );
                    break;
                default:
                    throw new CswDniException( "CswNbtSchdItemFactory.makeSchdItem() found an unsupported Object Class: " + CswNbtNode.ObjectClass.ObjectClass.ToString() );
            }

            CswNbtDbBasedSchdEvents CswNbtDbBasedSchdEvents = new CswNbtDbBasedSchdEvents();
            ReturnVal.OnScheduleItemWasRun = new CswNbtSchdItem.CswNbtSchedEventHdlr( CswNbtDbBasedSchdEvents.handleOnSchdItemWasRun );

            return ( ReturnVal );

        }//makeSchdItem()

    }//CswSchdItemFactory

}//namespace ChemSW.Nbt.Sched

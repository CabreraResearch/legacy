using System;
using System.Collections;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.Sched
{

    public class CswNbtSchdItemUpdatePropertyValues : CswNbtSchdItem
    {
        private CswNbtResources _CswNbtResources = null;


        public override void reset()
        {
            _Succeeded = true;
            _StatusMessage = string.Empty;
        }//


        public CswNbtSchdItemUpdatePropertyValues( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            SchedItemName = "UpdatePropertyValues";
        }//ctor

        override public bool doesItemRunNow()
        {
            return ( true );
        }//runNow()

        //override public DueType DueType
        //{
        //    get
        //    {
        //        return ( DueType.Always );
        //    }//
        //}//DueType

        //override public Int32 WarnDays
        //{
        //    get
        //    {
        //        return ( Int32.MinValue );
        //    }
        //}//WarnDays


        override public void run()
        {
            try
            {
                if ( _CswNbtResources == null )
                    throw new CswDniException( "_CswNbtResources is null" );

                // Find which nodes are out of date
                CswStaticSelect OutOfDateNodesQuerySelect = _CswNbtResources.makeCswStaticSelect( "OutOfDateNodes_select", "ValuesToUpdate" );
                DataTable OutOfDateNodes = OutOfDateNodesQuerySelect.getTable( false, false, 0, 25 );

                if ( OutOfDateNodes.Rows.Count > 0 )
                {
                    // Update one of them at random (which will keep us from encountering errors which gum up the queue)
                    Random rand = new Random();
                    Int32 index = rand.Next( 0, OutOfDateNodes.Rows.Count );
                    CswPrimaryKey nodeid = new CswPrimaryKey( "nodes", CswConvert.ToInt32( OutOfDateNodes.Rows[index]["nodeid"].ToString() ) );
                    //Int32 propid = CswConvert.ToInt32(OutOfDateNodes.Rows[index]["nodetypepropid"].ToString());
                    //Int32 jctnodepropid = CswConvert.ToInt32(OutOfDateNodes.Rows[index]["jctnodepropid"].ToString());
                    CswNbtNode Node = _CswNbtResources.Nodes[ nodeid ];
                    if( Node == null )
                        throw new CswDniException( "Node not found (" + nodeid.ToString() + ")" );
                    // Don't update nodes of disabled nodetypes
                    if( Node.NodeType != null )
                    {
                        CswNbtActUpdatePropertyValue CswNbtActUpdatePropertyValue = new CswNbtActUpdatePropertyValue( _CswNbtResources );
                        CswNbtActUpdatePropertyValue.UpdateNode( Node );
                        Node.postChanges( false );
                    }

                }//if there were out of date nodes


            }//try

            catch ( Exception Exception )
            {
                _Succeeded = false;
                _StatusMessage = "Error running '" + Name + "': " + Exception.Message;
            }//catch

        }//run()

        override public string Name
        {
            get
            {
                return ( "Update Property Values" );
            }
        }

        private bool _Succeeded = true;
        override public bool Succeeded
        {
            get
            {
                return ( _Succeeded );
            }

        }//Succeeded

        private string _StatusMessage = "";
        override public string StatusMessage
        {
            get
            {
                return ( _StatusMessage );
            }

        }//StatusMessage

    }//CswNbtSchdItemUpdatePropertyValues

}//namespace ChemSW.Nbt.Sched

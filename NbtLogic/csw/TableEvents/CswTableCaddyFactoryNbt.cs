//using System;
//using System.Collections.Generic;
//using System.Collections;
//using System.Text;
//using System.Data;
//using ChemSW.DB;
//using ChemSW.Core;
//using ChemSW.Audit;

//namespace ChemSW.Nbt.TableEvents
//{

//    public class CswTableCaddyFactoryNbt : ICswTableCaddyFactory
//    {
//        private CswNbtResources _CswNbtResources;
//        public CswTableCaddyFactoryNbt( CswNbtResources CswNbtResources )
//        {
//            _CswNbtResources = CswNbtResources;
//        }

//        public CswTableCaddy makeCswTableCaddy(string TableName)
//        {
//            CswTableCaddy ReturnVal = new CswTableCaddy( _CswNbtResources.CswResources, TableName );

//            if ("nodetypes" == TableName.ToLower())
//            {
//                CswNbtTableEventBeforeUpdateNodeTypes CswNbtTableEventBeforeUpdateNodeTypes = new CswNbtTableEventBeforeUpdateNodeTypes();
//                ReturnVal.onBeforeUpdate += new CswTableCaddy.CswNbtTableModificationHandler(CswNbtTableEventBeforeUpdateNodeTypes.BeforeUpdateHandler);
//            }
//            if ("nodetype_props" == TableName.ToLower())
//            {
//                CswNbtTableEventBeforeUpdateNodeTypeProps CswNbtTableEventBeforeUpdateNodeTypeProps = new CswNbtTableEventBeforeUpdateNodeTypeProps();
//                ReturnVal.onBeforeUpdate += new CswTableCaddy.CswNbtTableModificationHandler(CswNbtTableEventBeforeUpdateNodeTypeProps.BeforeUpdateHandler);
//            }

//            if( "sequences" == TableName.ToLower() )
//            {
//                CswNbtTableEventBeforeUpdateSequences CswNbtTableEventBeforeUpdateSequences = new CswNbtTableEventBeforeUpdateSequences();
//                ReturnVal.onBeforeUpdate += new CswTableCaddy.CswNbtTableModificationHandler( CswNbtTableEventBeforeUpdateSequences.BeforeUpdateHandler );
//            }

//            //CswNbtTableEventAfterUpdateAuditing CswNbtTableEventAfterUpdateAuditing = new CswNbtTableEventAfterUpdateAuditing( _Resources.CswDataObjects.CswAuditRecorder );
//            //ReturnVal.onAfterUpdate += new CswTableCaddy.CswNbtTableModificationHandler( CswNbtTableEventAfterUpdateAuditing.AfterUpdateHandler );

//            return( ReturnVal );

//        }//makeCswTableCaddy()

//    }//ICswTableCaddyFactory

//}//namespace ChemSW.Nbt.TableEvents

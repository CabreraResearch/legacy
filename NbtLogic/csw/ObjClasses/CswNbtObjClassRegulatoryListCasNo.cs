using System;
using System.Data;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRegulatoryListCasNo : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string RegulatoryList = "Regulatory List";
            public const string CASNo = "CAS No";
            public const string TPQ = "TPQ";
            //public const string IsValid = "Is Valid";
            public const string ErrorMessage = "Error Message";
        }

        public CswNbtObjClassRegulatoryListCasNo( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListCasNoClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassRegulatoryListCasNo
        /// </summary>
        public static implicit operator CswNbtObjClassRegulatoryListCasNo( CswNbtNode Node )
        {
            CswNbtObjClassRegulatoryListCasNo ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.RegulatoryListCasNoClass ) )
            {
                ret = (CswNbtObjClassRegulatoryListCasNo) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void afterDeleteNode()
        {
            _setChemicalsPendingUpdate();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            CASNo.SetOnPropChange( _CasNo_OnChange );
        }//afterPopulateProps()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship RegulatoryList { get { return _CswNbtNode.Properties[PropertyName.RegulatoryList]; } }
        
        public CswNbtNodePropCASNo CASNo { get { return _CswNbtNode.Properties[PropertyName.CASNo]; } }

        public void _CasNo_OnChange( CswNbtNodeProp Prop, bool Creating )
        {
            string error;
            if( false == CASNo.Validate( out error ) )
            {
                ErrorMessage.Text = error;
            }
            else
            {
                ErrorMessage.Text = string.Empty;
            }
            _setChemicalsPendingUpdate();
        } // _CasNo_OnChange()

        public CswNbtNodePropQuantity TPQ { get { return _CswNbtNode.Properties[PropertyName.TPQ]; } }
        //public CswNbtNodePropLogical IsValid { get { return _CswNbtNode.Properties[PropertyName.IsValid]; } }
        public CswNbtNodePropText ErrorMessage { get { return _CswNbtNode.Properties[PropertyName.ErrorMessage]; } }
        
        #endregion

        private void _setChemicalsPendingUpdate()
        {
            // Not ideal, but... set all chemicals to refresh their reg lists
            // We do this directly, not using a view, for performance
            CswTableUpdate ChemicalNodesUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtObjClassRegulatoryListCasNo_casno_update", "nodes" );
            DataTable NodesTable = ChemicalNodesUpdate.getTable( @"where pendingupdate = '0' 
                                                                     and nodetypeid in (select nodetypeid from nodetypes 
                                                                                         where objectclassid = (select objectclassid from object_class 
                                                                                                                 where objectclass = '" + CswEnumNbtObjectClass.ChemicalClass.ToString() + "'))" );
            foreach( DataRow NodesRow in NodesTable.Rows )
            {
                NodesRow["pendingupdate"] = "1";
            }
            ChemicalNodesUpdate.update( NodesTable );
        } // _setChemicalsPendingUpdate()

    }//CswNbtObjClassRegulatoryListCasNo

}//namespace ChemSW.Nbt.ObjClasses

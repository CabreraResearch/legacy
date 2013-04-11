using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29243
    /// </summary>
    public class CswUpdateSchema_02A_Case29243A : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29243; }
        }

        private CswPrimaryKey SetNameNodeId;
        private CswNbtMetaDataNodeType FireClassExemptAmountNT;
        private CswNbtMetaDataNodeType FireClassExemptAmountSetNT;
        //private CswNbtMetaDataObjectClass FireClassExemptAmountOC;
        private CswNbtMetaDataObjectClass FireClassExemptAmountSetOC;

        public override void update()
        {
            CswCommaDelimitedString NewFireHazardClasses = new CswCommaDelimitedString {"Corr (liquified gas)",
                "CRY-NFG",
                "Exp-1.1",
                "Exp-1.2",
                "Exp-1.3", 
                "Exp-1.4", 
                "Exp-1.4G", 
                "Exp-1.5",
                "Exp-1.6", 
                "H.T. (liquified gas)", 
                "N/R",
                "NFG", 
                "NFG (liquified)", 
                "NFS", 
                "Tox (liquified gas)"};

            FireClassExemptAmountNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Fire Class Exempt Amount" );
            if( null != FireClassExemptAmountNT )
            {
                FireClassExemptAmountSetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.FireClassExemptAmountSetClass );
                if( null != FireClassExemptAmountSetOC )
                {
                    FireClassExemptAmountSetNT = FireClassExemptAmountSetOC.FirstNodeType;
                    if( null != FireClassExemptAmountSetNT )
                    {
                        foreach( KeyValuePair<CswPrimaryKey, string> NodeIdAndName in FireClassExemptAmountSetNT.getNodeIdAndNames( false, true ) )
                        {
                            if( NodeIdAndName.Value.Equals( "Default" ) )
                            {
                                SetNameNodeId = NodeIdAndName.Key;
                            }
                        }

                        #region FireClassExemptAmount Nodes

                        _createFireClassExemptAmountNode( 301.2, NewFireHazardClasses[0], "(liquified gas)", "Health", "Corrosives" );
                        _createFireClassExemptAmountNode( 212.2, NewFireHazardClasses[1], "NFG", "Physical", "Cryogenic" );
                        _createFireClassExemptAmountNode( 213.1, NewFireHazardClasses[2], "1.1", "Physical", "Explosive" );
                        _createFireClassExemptAmountNode( 213.2, NewFireHazardClasses[3], "1.2", "Physical", "Explosive" );
                        _createFireClassExemptAmountNode( 213.3, NewFireHazardClasses[4], "1.3", "Physical", "Explosive" );
                        _createFireClassExemptAmountNode( 213.4, NewFireHazardClasses[5], "1.4", "Physical", "Explosive" );
                        _createFireClassExemptAmountNode( 213.5, NewFireHazardClasses[6], "1.4G", "Physical", "Explosive" );
                        _createFireClassExemptAmountNode( 213.6, NewFireHazardClasses[7], "1.5", "Physical", "Explosive" );
                        _createFireClassExemptAmountNode( 213.7, NewFireHazardClasses[8], "1.6", "Physical", "Explosive" );
                        _createFireClassExemptAmountNode( 301.4, NewFireHazardClasses[9], "(liquified gas)", "Health", "Highly Toxic" );
                        _createFireClassExemptAmountNode( 1000.0, NewFireHazardClasses[10], "", "", "Non-Regulated" );
                        _createFireClassExemptAmountNode( 1000.0, NewFireHazardClasses[11], "", "", "Non-Flammable Gas" );
                        _createFireClassExemptAmountNode( 1000.0, NewFireHazardClasses[12], "(liquified)", "", "Non-Flammable Gas" );
                        _createFireClassExemptAmountNode( 1000.0, NewFireHazardClasses[13], "", "", "Non-Flammable Solid" );
                        _createFireClassExemptAmountNode( 308.1, NewFireHazardClasses[14], "(liquified gas)", "Health", "Toxic" );

                        #endregion FireClassExemptAmount Nodes

                    }//if( null != FireClassExemptAmountSetNT )
                }
            }//if( null != FireClassExemptAmountNT )

        } // update()

        #region Private Helper Functions

        private void _createFireClassExemptAmountNode( double SortOrder, string HazardClass, string Class, string HazardType, string HazardCategory )
        {
            CswNbtObjClassFireClassExemptAmount FireClassExemptAmountNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( FireClassExemptAmountNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            FireClassExemptAmountNode.SetName.RelatedNodeId = SetNameNodeId;
            FireClassExemptAmountNode.HazardCategory.Text = HazardCategory;
            FireClassExemptAmountNode.HazardClass.Value = HazardClass;
            FireClassExemptAmountNode.Class.Text = Class;
            FireClassExemptAmountNode.HazardType.Value = HazardType;
            FireClassExemptAmountNode.SortOrder.Value = SortOrder;

            FireClassExemptAmountNode.postChanges( false );
        }

        #endregion Private Helper Functions

    }//class CswUpdateSchema_02A_Case29243A

}//namespace ChemSW.Nbt.Schema
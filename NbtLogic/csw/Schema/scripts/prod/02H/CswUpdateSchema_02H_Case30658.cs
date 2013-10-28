using System;
using System.Linq;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30658 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30658; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "02H_Case30658"; }
        }

        public override void update()
        {
            // Add Regulatory List and Chemical to the add layout of RegulatoryListMember
            CswNbtMetaDataObjectClass RegListMemberOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass );
            foreach( CswNbtMetaDataNodeType RegListMemberNT in RegListMemberOC.getNodeTypes() )
            {
                foreach( CswNbtMetaDataNodeTypeProp PropNTP in RegListMemberNT.getNodeTypeProps().Where( p => Int32.MinValue != p.ObjectClassPropId ) )
                {
                    switch( PropNTP.getObjectClassPropName() )
                    {
                        case CswNbtObjClassRegulatoryListMember.PropertyName.RegulatoryList:
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
                            break;
                        case CswNbtObjClassRegulatoryListMember.PropertyName.Chemical:
                            PropNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
                            break;
                    }
                }
            }

            // Remove ability to add Regulatory List Members from Chemicals tab on Regulatory List
            CswNbtMetaDataObjectClass RegListMemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass );
            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            foreach( CswNbtMetaDataNodeType Nodetype in RegListOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ChemicalsNTP = Nodetype.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.Chemicals );
                CswNbtView ChemicalsView = _CswNbtSchemaModTrnsctn.restoreView( ChemicalsNTP.ViewId );
                CswNbtViewRoot.forEachRelationship EachRelationship = delegate( CswNbtViewRelationship Relationship )
                {
                    if( Relationship.SecondId == RegListMemOC.ObjectClassId )
                    {
                        Relationship.AllowAdd = false;
                    }
                };
                ChemicalsView.Root.eachRelationship( EachRelationship, null );
                ChemicalsView.save();
            }

        } // update()
    }

}//namespace ChemSW.Nbt.Schema
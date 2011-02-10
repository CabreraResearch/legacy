using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-20
    /// </summary>
    public class CswUpdateSchemaTo01H20 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 20 ); } }
        public CswUpdateSchemaTo01H20( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // case 20951
            CswNbtMetaDataObjectClass InspectionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            foreach( CswNbtMetaDataNodeType InspectionNT in InspectionOC.NodeTypes )
            {
                CswNbtMetaDataNodeType LatestVersionInspectionNT = InspectionNT.LatestVersionNodeType;
                if( !LatestVersionInspectionNT.IsLocked )
                {
                    CswNbtMetaDataNodeTypeTab ActionTab = LatestVersionInspectionNT.getNodeTypeTab( "Action" );
                    if( ActionTab == null )
                    {
                        ActionTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( LatestVersionInspectionNT, "Action", 2 );
                    }

                    CswNbtMetaDataNodeTypeProp FinishedProp = LatestVersionInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.FinishedPropertyName );
                    FinishedProp.NodeTypeTab = ActionTab;
                    FinishedProp.DisplayRow = 1;
                    FinishedProp.DisplayColumn = 1;

                    CswNbtMetaDataNodeTypeProp CancelledProp = LatestVersionInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.CancelledPropertyName );
                    CancelledProp.NodeTypeTab = ActionTab;
                    CancelledProp.DisplayRow = 2;
                    CancelledProp.DisplayColumn = 1;

                    CswNbtMetaDataNodeTypeProp CancelReasonProp = LatestVersionInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.CancelReasonPropertyName );
                    CancelReasonProp.NodeTypeTab = ActionTab;
                    CancelReasonProp.DisplayRow = 3;  // even though webapp interprets this independently, Mobile needs this to be 3
                    CancelReasonProp.DisplayColumn = 1;

                } // if( !InspectionNT.IsLocked )
            } // foreach( CswNbtMetaDataNodeType InspectionNT in InspectionOC.NodeTypes )


        } // update()

    }//class CswUpdateSchemaTo01H20

}//namespace ChemSW.Nbt.Schema



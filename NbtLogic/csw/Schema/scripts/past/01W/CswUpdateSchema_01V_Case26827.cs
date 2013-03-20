using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26827
    /// </summary>
    public class CswUpdateSchema_01V_Case26827 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 26827; }
        }

        public override void update()
        {
            // Set default value for SearchDeferPropId for all nodetypes and object classes
            CswNbtMetaDataObjectClass SynonymOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialSynonymClass );
            CswNbtMetaDataObjectClass ComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClass DispTransOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerDispenseTransactionClass );
            CswNbtMetaDataObjectClass ContLocOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerLocationClass );

            CswNbtMetaDataObjectClassProp SynonymMaterialOCP = SynonymOC.getObjectClassProp( CswNbtObjClassMaterialSynonym.PropertyName.Material );
            CswNbtMetaDataObjectClassProp ComponentMaterialOCP = ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );
            CswNbtMetaDataObjectClassProp SizeMaterialOCP = SizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
            CswNbtMetaDataObjectClassProp DispTransContainerOCP = DispTransOC.getObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.DestinationContainer );
            CswNbtMetaDataObjectClassProp ContLocContainerOCP = ContLocOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Container );


            StringCollection NotSearchableOCs = new StringCollection
                                                    {
                                                        NbtObjectClass.BatchOpClass,
                                                        NbtObjectClass.ContainerDispenseTransactionClass,
                                                        NbtObjectClass.InventoryGroupPermissionClass
                                                    };
            Dictionary<NbtObjectClass, Int32> DeferOcToOcpDict = new Dictionary<NbtObjectClass, Int32>
                                                    {
                                                        { NbtObjectClass.MaterialSynonymClass, SynonymMaterialOCP.PropId },
                                                        { NbtObjectClass.MaterialComponentClass, ComponentMaterialOCP.PropId },
                                                        { NbtObjectClass.SizeClass, SizeMaterialOCP.PropId },
                                                        { NbtObjectClass.ContainerDispenseTransactionClass, DispTransContainerOCP.PropId },
                                                        { NbtObjectClass.ContainerLocationClass, ContLocContainerOCP.PropId }
                                                    };

            CswTableUpdate OCUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "26827_OC_update", "object_class" );
            DataTable OCTable = OCUpdate.getTable();
            foreach( DataRow OCRow in OCTable.Rows )
            {
                NbtObjectClass thisOC = CswConvert.ToString( OCRow["objectclass"] );
                Int32 value = Int32.MinValue;
                if( NotSearchableOCs.Contains( thisOC ) )
                {
                    value = CswNbtMetaDataObjectClass.NotSearchableValue;
                }
                else if( DeferOcToOcpDict.Keys.Contains( thisOC ) )
                {
                    value = DeferOcToOcpDict[thisOC];
                }

                if( Int32.MinValue != value )
                {
                    OCRow["searchdeferpropid"] = CswConvert.ToDbVal( value );
                    foreach( CswNbtMetaDataNodeType nodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes( thisOC ) )
                    {
                        Int32 ntvalue = value;
                        if( CswNbtMetaDataObjectClass.NotSearchableValue != ntvalue )
                        {
                            ntvalue = nodeType.getNodeTypePropByObjectClassProp( ntvalue ).PropId;
                        }
                        nodeType.SearchDeferPropId = ntvalue;
                    }
                }
            } // foreach( DataRow OCRow in OCTable.Rows )
            OCUpdate.update( OCTable );

        } //Update()

    }//class CswUpdateSchema_01V_Case26827

}//namespace ChemSW.Nbt.Schema
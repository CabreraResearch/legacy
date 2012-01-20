using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-08
    /// </summary>
    public class CswUpdateSchemaTo01L08 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 08 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24434

            CswNbtMetaDataObjectClass AssemblyOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass );

            CswNbtMetaDataObjectClassProp AssemblyPartsOcp = AssemblyOc.getObjectClassProp( CswNbtObjClassEquipmentAssembly.AssemblyPartsPropertyName );
            if( null == AssemblyPartsOcp )
            {
                AssemblyPartsOcp = AssemblyOc.getObjectClassProp( "Parts" );
                if( null != AssemblyPartsOcp )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( AssemblyPartsOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, CswNbtObjClassEquipmentAssembly.AssemblyPartsPropertyName );
                }
            }
        
            foreach( CswNbtMetaDataNodeType AssemblyNt in AssemblyOc.NodeTypes )
            {
                CswNbtMetaDataNodeTypeProp PartsNtp = AssemblyNt.getNodeTypeProp( "Parts" );
                if( null != PartsNtp && Int32.MinValue != PartsNtp.ObjectClassPropId )
                {
                    PartsNtp.PropName = "Assembly Parts";
                }
            }

            #endregion Case 24434

        }//Update()

    }//class CswUpdateSchemaTo01L08

}//namespace ChemSW.Nbt.Schema



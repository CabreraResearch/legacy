using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections.Generic;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25118
    /// </summary>
    public class CswUpdateSchemaCase25118 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //Create Time Unit view and apply it to every Expiration Interval ObjectClassprop
            
            //Create the new view
            CswNbtMetaDataNodeType TimeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType("Time Unit");

            CswNbtView TimeView = _CswNbtSchemaModTrnsctn.makeView();
            TimeView.makeNew( "CswNbtNodePropExpirationInterval()", NbtViewVisibility.Property );
            TimeView.AddViewRelationship( TimeNT, true );
            TimeView.save();

            //Set ExpirationInterval ObjectClassProp default FKValue to Time Unit NodeType (so that new material NodeTypes will start with this Unit NodeType restriction)
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp ExpirationInterval = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( MaterialOC.ObjectClassId, CswNbtObjClassMaterial.ExpirationIntervalPropName );
            
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ExpirationInterval, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isfk, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ExpirationInterval, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.NodeTypeId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ExpirationInterval, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, TimeNT.NodeTypeId );

            //Update existing ExpirationInterval NodeTypeProps to use new Time view
            foreach( CswNbtMetaDataNodeType NodeType in MaterialOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ExpInt = NodeType.getNodeTypeProp("Expiration Interval");
                ExpInt.ViewId = TimeView.ViewId;
            }

        }//Update()

    }//class CswUpdateSchemaCase25118

}//namespace ChemSW.Nbt.Schema
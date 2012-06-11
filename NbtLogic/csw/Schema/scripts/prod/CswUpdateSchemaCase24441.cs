using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24441
    /// </summary>
    public class CswUpdateSchemaCase24441 : CswUpdateSchemaTo
    {
        public override void update()
        {

            // add some objclass calls
            CswNbtMetaDataObjectClassProp locCodeOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass, "Location Code",
                CswNbtMetaDataFieldType.NbtFieldType.Text, IsUnique: true );
            CswNbtMetaDataObjectClassProp allowInvOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass, "Allow Inventory",
                CswNbtMetaDataFieldType.NbtFieldType.Logical );
            CswNbtMetaDataObjectClassProp storCompatOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass, "Storage Compatability",
                CswNbtMetaDataFieldType.NbtFieldType.ImageList );
            CswDelimitedString listOpts = new CswDelimitedString( '\n' );
            listOpts.FromArray( new string[] { "None", "Inorganic Acids", "Organic Acids", 
                "Bases", "Oxidizing Inorganic Acids",
                "Oxidizers", "Toxics", "Flammables" } );
            //            storCompatOcp.ListOptions = listOpts.ToString();
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( storCompatOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, listOpts.ToString() );
            CswDelimitedString valOpts = new CswDelimitedString( '\n' );
            valOpts.FromArray( new string[] { "Images/cispro/0w.gif", "Images/cispro/1o.gif", "Images/cispro/2y.gif", 
                "Images/cispro/3g.gif", "Images/cispro/4b.gif", 
                "Images/cispro/5l.gif", "Images/cispro/6p.gif", "Images/cispro/7r.gif" } );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( storCompatOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.valueoptions, valOpts.ToString() );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( storCompatOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.textarearows, "32" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( storCompatOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.textareacols, "32" );

        }//Update()

    }//class CswUpdateSchemaCase24441

}//namespace ChemSW.Nbt.Schema
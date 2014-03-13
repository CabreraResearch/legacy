using System;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52285 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52285; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );

            // For the existing vendor nodetype(s), set the default value of Internal to false.
            foreach( CswNbtMetaDataNodeType VendorNT in VendorOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp VendorInternalNTP = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.Internal );
                VendorInternalNTP.DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.DefaultValue].AsLogical.Checked = CswEnumTristate.False;
                VendorInternalNTP.DesignNode.postChanges( false );

                // For all existing vendor nodes, set Internal to false.
                foreach( CswNbtObjClassVendor VendorNode in VendorNT.getNodes( false, true ) )
                {
                    VendorNode.Internal.Checked = CswEnumTristate.False;
                    VendorNode.postChanges( false );
                }

            }

            // Duplicate the existing Vendor nodetype, and name the new Vendor nodetype "Internal Vendor"
            CswNbtMetaDataNodeType FirstVendorNT = VendorOC.getNodeTypes().FirstOrDefault();
            if( null != FirstVendorNT )
            {
                CswNbtObjClassDesignNodeType NewVendorNTNode = FirstVendorNT.DesignNode.CopyNode();
                NewVendorNTNode.NodeTypeName.Text = "Internal Vendor";
                NewVendorNTNode.postChanges( false );

                // For the new vendor nodetype, set the default value of Internal to true.
                CswNbtMetaDataNodeTypeProp VendorInternalNTP = NewVendorNTNode.RelationalNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.Internal );
                VendorInternalNTP.DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.DefaultValue].AsLogical.Checked = CswEnumTristate.True;
                VendorInternalNTP.DesignNode.postChanges( false );
            }

            // Fix the default 'Vendors' view to use the Vendor object class instead of the nodetype.
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                CswNbtView View = _CswNbtSchemaModTrnsctn.restoreView( "Vendors", CswEnumNbtViewVisibility.Global );
                if( null != View )
                {
                    View.Root.ChildRelationships.Clear();
                    CswNbtViewRelationship rel1 = View.AddViewRelationship( VendorOC, true );
                    View.save();
                }
            }

        } // update()

    } // class CswUpdateSchema_02L_Case52285

}//namespace ChemSW.Nbt.Schema
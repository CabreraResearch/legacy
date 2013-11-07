using System;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case31079 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31079; }
        }

        public override string Title
        {
            get { return "Fix container size visibility "; }
        }

        //  This is a repeat of case 02F_Case30647.  See case 31079.
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp SizeOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Size );

            // To resolve case 30647, clear out .Hidden flag on 'Size'
            foreach( CswNbtMetaDataNodeTypeProp SizeNTP in SizeOCP.getNodeTypeProps() )
            {
                SizeNTP.Hidden = false;
            }

            //  getNodes() truncates!!!!!!
            //foreach( CswNbtObjClassContainer ContainerNode in ContainerOC.getNodes( false, true, false, true ) )
            //{
            //    ContainerNode.Size.setHidden( value: false, SaveToDb: true );
            //    ContainerNode.postChanges( false );
            //}
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update jct_nodes_props
                                                                          set hidden = '0'
                                                                        where nodetypepropid in (select p.nodetypepropid
                                                                                                   from nodetype_props p
                                                                                                   join object_class_props op on p.objectclasspropid = op.objectclasspropid
                                                                                                   join object_class oc on op.objectclassid = oc.objectclassid
                                                                                                  where oc.objectclass = 'ContainerClass'
                                                                                                    and op.propname = 'Size')
                                                                          and hidden = '1'" );

        } // update()

    } // class CswUpdateSchema_02H_Case31079

}//namespace ChemSW.Nbt.Schema
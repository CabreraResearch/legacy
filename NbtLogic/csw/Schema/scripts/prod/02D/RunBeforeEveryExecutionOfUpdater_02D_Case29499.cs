using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29499
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case29499B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29499; }
        }

        public override void update()
        {
            CswTableUpdate JctPSOCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29499_jctpsocp_update", "jct_propertyset_ocprop" );
            DataTable JctPSOCPTable = JctPSOCPUpdate.getEmptyTable();

            CswNbtMetaDataPropertySet DocumentPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.DocumentSet );
            foreach( CswNbtMetaDataObjectClass DocOC in DocumentPS.getObjectClasses() )
            {
                _createProps( DocOC, JctPSOCPTable, DocumentPS.PropertySetId );
            }

            JctPSOCPUpdate.update( JctPSOCPTable );
        } // update()

        private void _createProps( CswNbtMetaDataObjectClass DocumentOC, DataTable JctPSOCPTable, Int32 PropSetId )
        {
            CswNbtMetaDataObjectClassProp LastModifiedOnOCP = DocumentOC.getObjectClassProp( CswNbtPropertySetDocument.PropertyName.LastModifiedOn );
            if( null == LastModifiedOnOCP )
            {
                LastModifiedOnOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( DocumentOC, new CswNbtWcfMetaDataModel.ObjectClassProp( DocumentOC )
                {
                    PropName = CswNbtPropertySetDocument.PropertyName.LastModifiedOn,
                    FieldType = CswEnumNbtFieldType.DateTime,
                    ServerManaged = true,
                    Extended = CswEnumNbtDateDisplayMode.DateTime.ToString(),
                } );

                DataRow NewJctPSOCPRow = JctPSOCPTable.NewRow();
                NewJctPSOCPRow["objectclasspropid"] = LastModifiedOnOCP.PropId;
                NewJctPSOCPRow["propertysetid"] = CswConvert.ToDbVal( PropSetId );
                JctPSOCPTable.Rows.Add( NewJctPSOCPRow );
            }

            CswNbtMetaDataObjectClassProp LastModifiedByOCP = DocumentOC.getObjectClassProp( CswNbtPropertySetDocument.PropertyName.LastModifiedBy );
            if( null == LastModifiedByOCP )
            {
                CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
                LastModifiedByOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( DocumentOC, new CswNbtWcfMetaDataModel.ObjectClassProp( DocumentOC )
                {
                    PropName = CswNbtPropertySetDocument.PropertyName.LastModifiedBy,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    ServerManaged = true,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOC.ObjectClassId
                } );

                DataRow NewJctPSOCPRow = JctPSOCPTable.NewRow();
                NewJctPSOCPRow["objectclasspropid"] = LastModifiedByOCP.PropId;
                NewJctPSOCPRow["propertysetid"] = CswConvert.ToDbVal( PropSetId );
                JctPSOCPTable.Rows.Add( NewJctPSOCPRow );
            }
        }

    }//class CswUpdateSchema_02C_Case29499

}//namespace ChemSW.Nbt.Schema
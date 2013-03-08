using System;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01OC: CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: OC";

        #region Blame Logic

        private void _acceptBlame( CswDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswDeveloper.NBT;
            _CaseNo = 0;
        }

        private CswDeveloper _Author = CswDeveloper.NBT;

        public override CswDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        #endregion Blame Logic

        private CswNbtMetaDataNodeTypeProp _createNewProp( CswNbtMetaDataNodeType Nodetype, string PropName, CswNbtMetaDataFieldType.NbtFieldType PropType, bool SetValOnAdd = true )
        {
            CswNbtMetaDataNodeTypeProp Prop = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( Nodetype, PropType, PropName, Nodetype.getFirstNodeTypeTab().TabId );
            if( SetValOnAdd )
            {
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                    CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add,
                    Nodetype.NodeTypeId,
                    Prop.PropId,
                    true,
                    Nodetype.getFirstNodeTypeTab().TabId
                    );
            }
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit,
                Nodetype.NodeTypeId,
                Prop.PropId,
                true,
                Nodetype.getFirstNodeTypeTab().TabId
                );

            return Prop;
        }

        private static string _makeNodeTypePermissionValue( Int32 FirstVersionNodeTypeId, CswNbtPermit.NodeTypePermission Permission )
        {
            return "nt_" + FirstVersionNodeTypeId.ToString() + "_" + Permission.ToString();
        }

        #region Yorick Metods

        private void _updateUserFormats( CswDeveloper Dev, Int32 Case )
        {
            _acceptBlame( Dev, Case );

            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp DateFormatOcp = UserOc.getObjectClassProp( CswNbtObjClassUser.PropertyName.DateFormat );
            string ValidFormats = CswDateFormat.Mdyyyy + "," + CswDateFormat.dMyyyy + "," + CswDateFormat.yyyyMMdd_Dashes + "," + CswDateFormat.yyyyMd;
            ValidFormats += "," + CswDateFormat.ddMMMyyyy;

            if( DateFormatOcp.ListOptions != ValidFormats )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DateFormatOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, ValidFormats );
                foreach( CswNbtObjClassUser User in UserOc.getNodes( forceReInit : true, includeSystemNodes : false, IncludeDefaultFilters : false ) )
                {
                    if( false == string.IsNullOrEmpty( User.DateFormatProperty.Value ) &&
                        CswResources.UnknownEnum == (CswDateFormat) User.DateFormatProperty.Value )
                    {
                        User.DateFormatProperty.Value = CswDateTime.DefaultDateFormat.ToString();
                    }
                }
            }

            _resetBlame();
        }

        #region Case 28671
        
        private void _makeUnCode( CswDeveloper Dev, Int32 Case )
        {
            _acceptBlame( Dev, Case );

            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );

            if( null != MaterialOC )
            {

                //first remove existing prop which is of type relationship
                CswNbtMetaDataObjectClassProp UNCodeOCPOld = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( MaterialOC.ObjectClassId, CswNbtObjClassMaterial.PropertyName.UNCode );
                if( null != UNCodeOCPOld )
                {

                    _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( UNCodeOCPOld, true );

                }//if we have a un ocp


                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MaterialOC )
                {
                    PropName = CswNbtObjClassMaterial.PropertyName.UNCode,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    IsRequired = false
                } );

                //now add new prop which is of type text

            }//if we have a material oc

            _resetBlame();
        }

        #endregion Case 28671

        #region Case 29039

        private void _correctGeneratorTargetTypeProps( CswDeveloper Dev, Int32 Case )
        {
            _acceptBlame( Dev, Case );

            CswNbtMetaDataObjectClass GeneratorOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GeneratorClass );
            CswNbtMetaDataObjectClassProp TargetTypeOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassGenerator.PropertyName.TargetType );
            
            //This prop is already server managed, but I think this makes the intention explicit for the reader
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TargetTypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );
            
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TargetTypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TargetTypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, false );

            //To prevent the various behaviors associated with changing Owner, make it readonly
            CswNbtMetaDataObjectClassProp OwnerOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( OwnerOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( OwnerOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( OwnerOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );

            _resetBlame();
        }

        #endregion Case 29039

        #endregion Yorick Metods

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region YORICK

            //YORICK OC changes go here.

            
            _makeUnCode( CswDeveloper.PG, 28671 );
            _updateUserFormats( CswDeveloper.CF, 26574 );
            _correctGeneratorTargetTypeProps( CswDeveloper.CF, 29039 );

            #endregion YORICK

            //THIS GOES LAST!
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
        } //Update()
    }//class RunBeforeEveryExecutionOfUpdater_01OC
}//namespace ChemSW.Nbt.Schema



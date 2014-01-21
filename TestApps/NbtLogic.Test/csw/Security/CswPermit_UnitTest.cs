﻿using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Test.ServiceDrivers;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.Security
{
    [TestFixture]
    public class CswPermit_UnitTest : IDisposable
    {
        private TestData _TestData;

        private CswNbtMetaDataObjectClass _GenericOc;

        private CswNbtMetaDataNodeType _SprocketNt;
        private CswNbtMetaDataNodeTypeTab _FirstTab;
        private CswNbtMetaDataNodeTypeTab _SecondTab;
        private CswNbtMetaDataNodeTypeProp _NoAttributeNtp;
        private CswNbtMetaDataNodeTypeProp _ServerManagedNtp;
        private CswNbtMetaDataNodeTypeProp _ReadOnlyNtp;
        private CswNbtMetaDataNodeTypeProp _RequiredReadOnlyNtp;

        private CswNbtObjClassGeneric _SprocketNode;
        //private CswNbtObjClassRole _AdminRole;
        //private CswNbtObjClassRole _ReadOnlyRole;
        //private CswNbtObjClassRole _NodeTypeEditRole;
        //private CswNbtObjClassRole _NodeTypeCreateRole;
        //private CswNbtObjClassRole _NodeTypeViewRole;
        //private CswNbtObjClassRole _NodeTypeDeleteRole;

        /// <summary>
        /// Create the NodeType, the Tabs and the Props
        /// </summary>
        private void _InitMetaData()
        {
            _GenericOc = _TestData.CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GenericClass );

            _SprocketNt = _TestData.CswNbtResources.MetaData.getNodeTypeFirstVersion( "CswPermit Sprocket" );
            if( null == _SprocketNt )
            {
                _SprocketNt = _TestData.CswNbtResources.MetaData.makeNewNodeTypeNew( new CswNbtWcfMetaDataModel.NodeType( _GenericOc )
                {
                    NodeTypeName = "CswPermit Sprocket",
                    Category = "CswPermit"
                } );

                _FirstTab = _SprocketNt.getNodeTypeTab( "CswPermit Sprocket" );
                _SecondTab = _SprocketNt.getNodeTypeTab( "Identity" );

                _NoAttributeNtp = _TestData.CswNbtResources.MetaData.makeNewPropNew(
                    new CswNbtWcfMetaDataModel.NodeTypeProp( _SprocketNt, _TestData.CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Text ), "Name" ) );
                _NoAttributeNtp.updateLayout( CswEnumNbtLayoutType.Add, DoMove: false );
                _NoAttributeNtp.updateLayout( CswEnumNbtLayoutType.Edit, DoMove: false, TabId: _FirstTab.TabId );
                _NoAttributeNtp.updateLayout( CswEnumNbtLayoutType.Edit, DoMove: false, TabId: _SecondTab.TabId );

                _ServerManagedNtp = _TestData.CswNbtResources.MetaData.makeNewPropNew(
                    new CswNbtWcfMetaDataModel.NodeTypeProp( _SprocketNt, _TestData.CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Text ), "Description" ) );
                _ServerManagedNtp.DesignNode.ServerManaged.Checked = CswEnumTristate.True;
                _ServerManagedNtp.updateLayout( CswEnumNbtLayoutType.Add, DoMove: false );
                _ServerManagedNtp.updateLayout( CswEnumNbtLayoutType.Edit, DoMove: false, TabId: _FirstTab.TabId );
                _ServerManagedNtp.updateLayout( CswEnumNbtLayoutType.Edit, DoMove: false, TabId: _SecondTab.TabId );

                _ReadOnlyNtp = _TestData.CswNbtResources.MetaData.makeNewPropNew(
                    new CswNbtWcfMetaDataModel.NodeTypeProp( _SprocketNt, _TestData.CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Text ), "Status" ) );
                _ReadOnlyNtp.DesignNode.ReadOnly.Checked = CswEnumTristate.True;
                _ReadOnlyNtp.updateLayout( CswEnumNbtLayoutType.Add, DoMove: false );
                _ReadOnlyNtp.updateLayout( CswEnumNbtLayoutType.Edit, DoMove: false, TabId: _FirstTab.TabId );
                _ReadOnlyNtp.updateLayout( CswEnumNbtLayoutType.Edit, DoMove: false, TabId: _SecondTab.TabId );

                _RequiredReadOnlyNtp = _TestData.CswNbtResources.MetaData.makeNewPropNew(
                    new CswNbtWcfMetaDataModel.NodeTypeProp( _SprocketNt, _TestData.CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Text ), "Type" ) );
                _RequiredReadOnlyNtp.DesignNode.ReadOnly.Checked = CswEnumTristate.True;
                _RequiredReadOnlyNtp.DesignNode.Required.Checked = CswEnumTristate.True;
                _RequiredReadOnlyNtp.updateLayout( CswEnumNbtLayoutType.Add, DoMove: false );
                _RequiredReadOnlyNtp.updateLayout( CswEnumNbtLayoutType.Edit, DoMove: false, TabId: _FirstTab.TabId );
                _RequiredReadOnlyNtp.updateLayout( CswEnumNbtLayoutType.Edit, DoMove: false, TabId: _SecondTab.TabId );

                _ServerManagedNtp.DesignNode.postChanges( false );
                _ReadOnlyNtp.DesignNode.postChanges( false );
                _RequiredReadOnlyNtp.DesignNode.postChanges( false );
            }
        }

        [SetUp]
        public void Init()
        {
            _TestData = new TestData { FinalizeNodes = true };
            _InitMetaData();

            _SprocketNode = _TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( _SprocketNt.NodeTypeId, OverrideUniqueValidation: true );
            //_SprocketNode.postChanges( ForceUpdate: false );
        }

        [TearDown]
        public void Destroy()
        {
            if( null != _TestData )
            {
                _TestData.Destroy();
                _TestData = null;
            }
        }

        public void Dispose()
        {
            Destroy();
        }

        private CswNbtResources _testInit( string RoleName, string UserName, CswEnumTristate IsAdmin, CswEnumNbtNodeTypePermission Permission, bool PermissionValue )
        {
            CswNbtSdResourcesMgr ResourcesMgr = new CswNbtSdResourcesMgr( _TestData.CswNbtResources );
            CswNbtObjClassRole Role = _TestData.CswNbtResources.Nodes[ResourcesMgr.makeNewRole( RoleName )];
            CswNbtResources Ret = null;
            if( null != Role )
            {
                Role.Administrator.Checked = IsAdmin;
                _TestData.CswNbtResources.Permit.set( Permission, _SprocketNt, Role, PermissionValue );
                Role.postChanges( ForceUpdate : false );
                _TestData.CswNbtResources.finalize();

                Ret = ResourcesMgr.makeNewUserResources( UserName, Role.NodeId );
            }
            return Ret;
        }

        /// <summary>
        /// Test NodeType Edit permission for a particular User
        /// </summary>
        [TestCase( "CswPermitAdminRole", "CswPermitAdminUser", true, Result = true)]
        //NOTE: you can have as many of these [TestCase]s as you want. Simply add them to generate a new iteration of the test with different parameters.
        public bool CanEditWithNodeTypePermission( string RoleName, string UserName, bool PermissionValue )
        {
            CswNbtResources NewResources = _testInit( RoleName, UserName, CswEnumTristate.True, CswEnumNbtNodeTypePermission.Edit, PermissionValue );
            Assert.NotNull( NewResources );
            //Assert.That( NewResources.Permit.canAnyTab );
            return NewResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, _SprocketNt, NewResources.CurrentNbtUser );
        }

        
    }
}

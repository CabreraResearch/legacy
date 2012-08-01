using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27423
    /// </summary>
    public class CswUpdateSchemaCase27423Part2 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass LocationOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtObjClassLocation DefaultLocation = null;
            
            foreach( CswNbtObjClassLocation Location in LocationOc.getNodes( true, false ) )
            {
                if( null != Location )
                {
                    if( Location.Name.Text.ToLower() == "center hall" )
                    {
                        DefaultLocation = Location;
                    }
                        
                }
            }
            
            CswNbtMetaDataObjectClass WorkUnitOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass );
            CswNbtMetaDataNodeType WorkUnitNt = WorkUnitOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null != WorkUnitNt )
            {
                CswNbtObjClassWorkUnit DefaultWorkUnit = null;
                foreach (CswNbtObjClassWorkUnit WuNode in WorkUnitNt.getNodes(true, false))
                {
                    if( null != WuNode && WuNode.Name.Text == "Default Work Unit" )
                    {
                        DefaultWorkUnit = WuNode;
                    }
                }
                if( null == DefaultWorkUnit )
                {
                    DefaultWorkUnit = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId(WorkUnitNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, OverrideUniqueValidation: false);
                    DefaultWorkUnit.Name.Text = "Default Work Unit";
                    DefaultWorkUnit.IsDemo = true;
                    DefaultWorkUnit.postChanges(ForceUpdate: false);
                }

                CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                Collection<Int32> RoleIdsWithUsers = new Collection<Int32>();
                CswNbtMetaDataNodeType UserNt = null;
                foreach( CswNbtObjClassUser User in UserOc.getNodes( true, false ) )
                {
                    if( null != User )
                    {
                        UserNt = UserNt ?? User.NodeType;
                        RoleIdsWithUsers.Add( User.RoleId.PrimaryKey );
                    }
                }

                CswNbtMetaDataObjectClass RoleOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
                if( null == UserNt )
                {
                    UserNt = UserOc.getLatestVersionNodeTypes().FirstOrDefault();
                }
                if( null != UserNt )
                {
                    foreach( CswNbtObjClassRole Role in RoleOc.getNodes( true, false ) )
                    {
                        if( null != Role && false == RoleIdsWithUsers.Contains( Role.NodeId.PrimaryKey ) )
                        {
                            string ValidUserName = CswNbtObjClassUser.getValidUserName( Role.Name.Text.ToLower() );
                            if( ValidUserName != CswNbtObjClassUser.ChemSWAdminUsername )
                            {
                                if( CswNbtObjClassUser.IsUserNameUnique( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources, ValidUserName ) )
                                {
                                    CswNbtObjClassUser NewUser = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UserNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, OverrideUniqueValidation: false );
                                    NewUser.IsDemo = false;
                                    NewUser.Role.RelatedNodeId = Role.NodeId;
                                    NewUser.UsernameProperty.Text = ValidUserName;
                                    if( null != DefaultWorkUnit )
                                    {
                                        NewUser.WorkUnitProperty.RelatedNodeId = DefaultWorkUnit.NodeId;
                                    }
                                    if( null != DefaultLocation )
                                    {
                                        NewUser.DefaultLocationProperty.SelectedNodeId = DefaultLocation.NodeId;
                                    }
                                    NewUser.PasswordProperty.Password = Role.Name.Text.ToLower();
                                    NewUser.postChanges( ForceUpdate: false );
                                }
                            }
                        }
                    }
                }
            }
            

        }//Update()

    }//class CswUpdateSchemaCase27423Part2

}//namespace ChemSW.Nbt.Schema
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassFireExtinguisher : CswNbtObjClass
    {
        public static string LastInspectionDatePropertyName { get { return "Last Inspection Date"; } }
        public static string StatusPropertyName { get { return "Status"; } }
        public static string MountPointPropertyName { get { return "Mount Point"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassFireExtinguisher( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        public CswNbtObjClassFireExtinguisher( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass ); }
        }

        #region Inherited Events

        public override void beforeCreateNode()
        {
            _CswNbtObjClassDefault.beforeCreateNode();
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode()
        {
            _CswNbtObjClassDefault.beforeWriteNode();
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropDate LastInspectionDate
        {
            get
            {
                return ( _CswNbtNode.Properties[LastInspectionDatePropertyName].AsDate );
            }
        }

        public CswNbtNodePropList Status
        {
            get
            {
                return ( _CswNbtNode.Properties[StatusPropertyName].AsList );
            }

        }
        public CswNbtNodePropRelationship MountPoint
        {
            get
            {
                return ( _CswNbtNode.Properties[MountPointPropertyName].AsRelationship );
            }
        }

        #endregion



    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses

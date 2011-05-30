//using System;
//using System.Data;
//using System.Xml;
//using System.Collections.Generic;
//using System.Text;
//using ChemSW.Core;
//using ChemSW.Nbt;
//using ChemSW.Nbt.ObjClasses;
//using ChemSW.Nbt.MetaData;
//using ChemSW.Nbt.PropTypes;
//using ChemSW.Exceptions;

//namespace ChemSW.Nbt.Schema
//{
//    /// <summary>
//    /// Keeps the schema up-to-date
//    /// </summary>
//    public class CswNbtNodeTypePermissions
//    {
//        /// <summary>
//        /// Encapsulate editing nodetypeprops
//        /// 
//        /// </summary>
//        /// 
//        private CswNbtMetaDataNodeType _CswNbtMetaDataNodeType = null;
//        private CswNbtObjClassRole _CswNbtObjClassRole = null;
//        private CswNbtNodePropLogicalSet _CswNbtNodePropLogicalSet = null;

//        public CswNbtNodeTypePermissions( CswNbtObjClassRole CswNbtObjClassRole, CswNbtMetaDataNodeType CswNbtMetaDataNodeType )
//        {
//            _CswNbtMetaDataNodeType = CswNbtMetaDataNodeType;
//            _CswNbtObjClassRole = CswNbtObjClassRole;
//            _CswNbtNodePropLogicalSet = _CswNbtObjClassRole.NodeTypePermissions;
//        }//ctor

//        public void Save()
//        {
//            _CswNbtNodePropLogicalSet.Save();
//            _CswNbtObjClassRole.postChanges( false );
//        }//

//        public bool Create
//        {
//            set
//            {
//                _CswNbtNodePropLogicalSet.SetValue( NodeTypePermission.Create.ToString(), _CswNbtMetaDataNodeType.NodeTypeId.ToString(), value );
//            }
//        }

//        public bool Delete
//        {
//            set
//            {
//                _CswNbtNodePropLogicalSet.SetValue( NodeTypePermission.Delete.ToString(), _CswNbtMetaDataNodeType.NodeTypeId.ToString(), value );
//            }
//        }

//        public bool Edit
//        {
//            set
//            {
//                _CswNbtNodePropLogicalSet.SetValue( NodeTypePermission.Edit.ToString(), _CswNbtMetaDataNodeType.NodeTypeId.ToString(), value );

//            }
//        }

//        public bool View
//        {
//            set
//            {
//                _CswNbtNodePropLogicalSet.SetValue( NodeTypePermission.View.ToString(), _CswNbtMetaDataNodeType.NodeTypeId.ToString(), value );
//            }
//        }

//    }//class CswNbtNodeTypePermisions

//}//ChemSW.Nbt.Schema

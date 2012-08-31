using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24525
    /// </summary>
    public class CswUpdateSchemaCase24525 : CswUpdateSchemaTo
    {
        public override void update()
        {
            #region ADD ARCHIVED PROP TO USER
            CswNbtMetaDataFieldType logicalFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Logical );
            CswNbtMetaDataObjectClass userOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp archivedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( userOC )
            {
                PropName = CswNbtObjClassUser.PropertyName.Archived,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                IsFk = false,
                IsRequired = true,
                ValuePropType = logicalFT.FieldType,
                ValuePropId = logicalFT.FieldTypeId,
            } );
            //set the default val to false - we don't want new users to be archived
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( archivedOCP, archivedOCP.getFieldTypeRule().SubFields.Default.Name, Tristate.False );

            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

            foreach( CswNbtNode userNode in userOC.getNodes( false, false ) )
            {
                userNode.Properties[CswNbtObjClassUser.PropertyName.Archived].AsLogical.Checked = Tristate.False;
                userNode.postChanges( false );
            }
            #endregion

            #region UPDATE VIEWS
            foreach( CswNbtMetaDataNodeTypeProp relationshipProp in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType.Relationship ) )
            {
                if( relationshipProp.IsUserRelationship() )
                {
                    CswNbtView userView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( relationshipProp.ViewId );
                    if( false == userView.IsEmpty() )
                    {
                        CswNbtViewRelationship parent = userView.Root.ChildRelationships[0];
                        bool filterExists;
                        bool viewPropExists;
                        CswNbtViewProperty archivedVP = _viewPropAndFilterExists( out filterExists, out viewPropExists, userView, archivedOCP, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Tristate.True.ToString() );
                        if( false == viewPropExists ) //the view prop isn't there, add it with the filter
                        {
                            userView.AddViewPropertyAndFilter( parent, archivedOCP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: Tristate.True.ToString() );
                        }
                        else if( viewPropExists && false == filterExists ) //the view prop is there, but with no filter
                        {
                            userView.AddViewPropertyFilter( archivedVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: Tristate.True.ToString() );
                        }
                        userView.save();
                    }
                }
            }
            #endregion

        }//Update()

        /// <summary>
        /// Helper method to determine if a view property exists with a given filter. Returns the view property if it exists.
        /// </summary>
        /// <param name="filterExists">If the filter exists</param>
        /// <param name="viewPropExists">If the given prop is in the view</param>
        /// <param name="view">The view to check</param>
        /// <param name="prop">The property to check</param>
        /// <returns>The view property (null if the prop doesn't exist)</returns>
        private CswNbtViewProperty _viewPropAndFilterExists( out bool filterExists,
            out bool viewPropExists,
            CswNbtView view,
            CswNbtMetaDataObjectClassProp prop,
            CswNbtPropFilterSql.PropertyFilterMode
            filterMode,
            string value )
        {
            CswNbtViewProperty ret = null;
            filterExists = false;
            viewPropExists = false;
            foreach( CswNbtViewProperty viewProp in view.getOrderedViewProps( false ) )
            {
                foreach( CswNbtViewPropertyFilter existingFilter in viewProp.Filters )
                {
                    if( viewProp.ObjectClassPropId.Equals( prop.ObjectClassPropId ) )
                    {
                        viewPropExists = true;
                        if( existingFilter.FilterMode.Equals( filterMode ) &&
                            existingFilter.Value.Equals( value ) )
                        {
                            filterExists = true;
                            ret = viewProp;
                        }
                    }
                }
            }
            return ret;
        }

    }//class CswUpdateSchemaCase24525

}//namespace ChemSW.Nbt.Schema
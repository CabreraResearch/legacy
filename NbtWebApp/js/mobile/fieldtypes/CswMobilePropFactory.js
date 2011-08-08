/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="CswMobileFieldTypeBarcode.js" />
/// <reference path="CswMobileFieldTypeTime.js" />
/// <reference path="CswMobileFieldTypeDate.js" />
/// <reference path="CswMobileFieldTypeLink.js" />
/// <reference path="CswMobileFieldTypeList.js" />
/// <reference path="CswMobileFieldTypeLogical.js" />
/// <reference path="CswMobileFieldTypeMemo.js" />
/// <reference path="CswMobileFieldTypeNumber.js" />
/// <reference path="CswMobileFieldTypePassword.js" />
/// <reference path="CswMobileFieldTypeQuantity.js" />
/// <reference path="CswMobileFieldTypeQuestion.js" />
/// <reference path="CswMobileFieldTypeStatic.js" />
/// <reference path="CswMobileFieldTypeText.js" />

//#region CswMobilePropsFactory

function CswMobilePropsFactory(propDef) {
	/// <summary>
	///   Props class factory. Responsible for generating properties according to Field Type rules.
	/// </summary>
    /// <param name="propDef" type="Object">Prop definitional data.</param>
	/// <returns type="CswMobilePropsFactory">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var $label, $content, contentDivId, nodeId, tabId, viewId, fieldType, propId, propName, prop;
    
    //ctor
    (function () {
        
        var p = {
            nodeId: '',
            tabId: '',
            viewId: '',
            propId: '',
            propName: '',
            fieldtype: CswFieldTypes.Static.name
            //prop data follows
        };
        if (propDef) $.extend(p, propDef);

        var field = tryParseString(p.fieldtype, '');
        nodeId = p.nodeId;
        tabId = p.tabId;
        viewId = p.viewId;
        
        prop = getPropFromFieldType(field, p);
        fieldType = prop.fieldType;
        propId = prop.propId;
        propName = prop.propName;
        contentDivId = prop.contentDivId;
        
        $label = $('<h2 id="' + propId + '_label" style="white-space:normal;" class="' + CswMobileCssClasses.proplabel.name + '">' + propName + '</h2>');
        $content = prop.$content;
    })(); //ctor

    function getPropFromFieldType(field, ftDef) {
        /// <summary>
	    ///   Generate a property according to its field type definition.
	    /// </summary>
        /// <param name="field" type="String">Field type name.</param>
        /// <param name="ftDef" type="Object">Field type defintional data.</param>
	    /// <returns type="CswMobileFieldType<Prop>">A field type prop which implements $content and applyFieldTypeLogicToContent.</returns>
        var ret;
        switch (field) {
            case CswFieldTypes.AuditHistoryGrid.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswFieldTypes.Barcode.name:
                ret = new CswMobileFieldTypeBarcode(ftDef);                
                break;
	        case CswFieldTypes.Composite.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.Date.name:
	            ret = new CswMobileFieldTypeDate(ftDef);
	            break;
	        case CswFieldTypes.File.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.Grid.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.Image.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.Link.name:
	            ret = new CswMobileFieldTypeLink(ftDef);
	            break;
	        case CswFieldTypes.List.name:
	            ret = new CswMobileFieldTypeList(ftDef);
	            break;
	        case CswFieldTypes.Location.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.LocationContents.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.Logical.name:
	            ret = new CswMobileFieldTypeLogical(ftDef);
	            break;
	        case CswFieldTypes.LogicalSet.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.Memo.name:
	            ret = new CswMobileFieldTypeMemo(ftDef);
	            break;
	        case CswFieldTypes.MTBF.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.MultiList.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.NodeTypeSelect.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.Number.name:
	            ret = new CswMobileFieldTypeNumber(ftDef);
	            break;
	        case CswFieldTypes.Password.name:
	            ret = new CswMobileFieldTypePassword(ftDef);
	            break;
	        case CswFieldTypes.PropertyReference.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.Quantity.name:
	            ret = new CswMobileFieldTypeQuantity(ftDef);
	            break;
	        case CswFieldTypes.Question.name:
	            ret = new CswMobileFieldTypeQuestion(ftDef);
	            break;
	        case CswFieldTypes.Relationship.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.Scientific.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.Sequence.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.Static.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.Text.name:
	            ret = new CswMobileFieldTypeText(ftDef);
	            break;
	        case CswFieldTypes.Time.name:
	            ret = new CswMobileFieldTypeTime(ftDef);
	            break;
	        case CswFieldTypes.TimeInterval.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.UserSelect.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.ViewPickList.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        case CswFieldTypes.ViewReference.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
	        default:
	            ret = new CswMobileFieldTypeStatic(ftDef);
	            break;
        }
        return ret;
    }

    function applyFieldTypeLogicToContent($control) {
        /// <summary>
	    ///   Takes a control which should have some logic (CSS styling, etc.) applied according to Field Type rules and returns it.
	    /// </summary>
        /// <param name="$control" type="jQuery">A control to modify.</param>
	    /// <returns type="jQuery">The modified control.</returns>
        var ret = prop.applyFieldTypeLogicToContent($control);
        return ret;
    }
    
	//#endregion private
    
    //#region public, priveleged

    this.$label = $label;
    this.$content = $content;
    this.contentDivId = contentDivId;
    this.applyFieldTypeLogicToContent = applyFieldTypeLogicToContent;
    this.nodeId = nodeId;
    this.tabId = tabId;
    this.viewId = viewId;
    this.propId = propId;
    this.propName = propName;
    
    //#endregion public, priveleged
}

//#endregion CswMobilePropsFactory
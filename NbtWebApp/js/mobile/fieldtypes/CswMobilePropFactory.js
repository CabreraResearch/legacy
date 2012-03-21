/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="CswMobileFieldTypeBarcode.js" />
/// <reference path="CswMobileFieldTypeDateTime.js" />
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
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />

//#region CswMobilePropsFactory

function CswMobilePropsFactory(propDef) {
    "use strict";
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
            'readonly': false,
            propId: '',
            propName: '',
            fieldtype: CswSubFields_Map.Static.name
            //prop data follows
        };
        if (propDef) $.extend(p, propDef);
        var field = tryParseString(p.fieldtype, '');

        if (isTrue(p['readonly'])) {
            field = CswSubFields_Map.Static.name;
        }

        nodeId = p.nodeId;
        tabId = p.tabId;
        viewId = p.viewId;

        prop = getPropFromFieldType(field, p);
        fieldType = prop.fieldType;
        propId = prop.propId;
        propName = prop.propName;
        contentDivId = prop.contentDivId;

        if (prop.showLabel !== false) {
            $label = $('<h2 id="' + propId + '_label" style="white-space:normal;" class="' + CswMobileCssClasses.proplabel.name + '">' + propName + '</h2>');
        }
        //prop.applyFieldTypeLogicToContent($label);
        $content = prop.$content;
    })();  //ctor

    function getPropFromFieldType(field, ftDef) {
        /// <summary>
        ///   Generate a property according to its field type definition.
        /// </summary>
        /// <param name="field" type="String">Field type name.</param>
        /// <param name="ftDef" type="Object">Field type defintional data.</param>
        /// <returns type="CswMobileFieldType[Prop]">A field type prop which implements $content and applyFieldTypeLogicToContent.</returns>
        var ret;
        switch (field) {
            case CswSubFields_Map.AuditHistoryGrid.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.Barcode.name:
                ret = new CswMobileFieldTypeBarcode(ftDef);                
                break;
            case CswSubFields_Map.Button.name:
                ret = new CswMobileFieldTypeButton(ftDef);                
                break;
            case CswSubFields_Map.Composite.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.DateTime.name:
                ret = new CswMobileFieldTypeDate(ftDef);
                break;
            case CswSubFields_Map.File.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.Grid.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.Image.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.Link.name:
                ret = new CswMobileFieldTypeLink(ftDef);
                break;
            case CswSubFields_Map.List.name:
                ret = new CswMobileFieldTypeList(ftDef);
                break;
            case CswSubFields_Map.Location.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.LocationContents.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.Logical.name:
                ret = new CswMobileFieldTypeLogical(ftDef);
                break;
            case CswSubFields_Map.LogicalSet.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.Memo.name:
                ret = new CswMobileFieldTypeMemo(ftDef);
                break;
            case CswSubFields_Map.MTBF.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.MultiList.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.NodeTypeSelect.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.Number.name:
                ret = new CswMobileFieldTypeNumber(ftDef);
                break;
            case CswSubFields_Map.Password.name:
                ret = new CswMobileFieldTypePassword(ftDef);
                break;
            case CswSubFields_Map.PropertyReference.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.Quantity.name:
                ret = new CswMobileFieldTypeQuantity(ftDef);
                break;
            case CswSubFields_Map.Question.name:
                ret = new CswMobileFieldTypeQuestion(ftDef);
                break;
            case CswSubFields_Map.Relationship.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.Scientific.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.Sequence.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.Static.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.Text.name:
                ret = new CswMobileFieldTypeText(ftDef);
                break;
            case CswSubFields_Map.TimeInterval.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.UserSelect.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.ViewPickList.name:
                ret = new CswMobileFieldTypeStatic(ftDef);
                break;
            case CswSubFields_Map.ViewReference.name:
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
    
    function updatePropValue(json,id,newValue) {
        var ret = prop.updatePropValue(json, id, newValue);
        return ret;
    }
    
    //#endregion private
    
    //#region public, priveleged

    this.$label = $label;
    this.$content = $content;
    this.contentDivId = contentDivId;
    this.applyFieldTypeLogicToContent = applyFieldTypeLogicToContent;
    this.updatePropValue = updatePropValue;
    this.nodeId = nodeId;
    this.tabId = tabId;
    this.viewId = viewId;
    this.propId = propId;
    this.propName = propName;
    
    //#endregion public, priveleged
}

//#endregion CswMobilePropsFactory
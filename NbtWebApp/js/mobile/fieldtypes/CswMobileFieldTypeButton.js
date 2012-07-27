/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

//#region CswMobileFieldTypeButton

function CswMobileFieldTypeButton(ftDef) {
    /// <summary>
    ///   Button field type. Responsible for generating prop according to Field Type rules.
    /// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
    /// <returns type="CswMobileFieldTypeButton">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var divSuffix = '_propdiv',
        propSuffix = '_input',
        $content, contentDivId, elementId, propId, propName, subfields, value, gestalt;
    
    //ctor
    (function () {
        var p = {
            propId: '',
            propName: '',
            gestalt: '',
            value: '',
            checked: 'false',
            required: false
        };
        if (ftDef) $.extend(p, ftDef);
        var propVals = p.values,
            $button;

        var onButtonClick = function () {
            var realNodePropId = p.nodeId;
            var realPropId = p.propId.replace(realNodePropId, '').replace('prop_', '').replace('_', '');
            realNodePropId = realNodePropId.replace('nodeid_', '') + '_' + realPropId;

            $button.button();
            $button.button('disable');

            CswAjaxJson({
                url: '/NbtWebApp/wsNBT.asmx/onObjectClassButtonClick',
                data: {
                    NodeTypePropAttr: realNodePropId,
                    SelectedText: p.propName
                },
                success: function (data) {
                    if (isTrue(data.success) && (isNullOrEmpty(data.message) || data.action !== 'Unknown')) {
                        var $viewPage = $('#' + p.viewId);
                        $('#' + p.nodeId + '_li').remove();
                        $viewPage.CswChangePage();
                    } else {
                        $content.append(data.message);
                        $button.button('enable');
                    }
                }, // ajax success()
                error: function (data) {
                    $content.append(data.message);
                    $button.button('enable');
                }
            }); // ajax.post()

        }; // onButtonClick()

        propId = p.propId;
        propName = p.propName;
        contentDivId = propId + divSuffix;
        elementId = propId + propSuffix;

        subfields = CswSubFields_Map.Button.subfields;
        value = tryParseString(propVals[subfields.Text.name]);
        gestalt = tryParseString(p.gestalt, '');

        $content = ensureContent($content, contentDivId);
        contentDivId = p.nodekey;

        $button = $('<a id="' + elementId + '" data-role="button" data-identity="' + elementId + '" data-url="loginsubmit" href="javascript:void(0);">' + propName + '</a>')
            .appendTo($content);
        $button.click(onButtonClick);

    })();          //ctor
        
    function applyFieldTypeLogicToContent($control) {
        
    }

    function updatePropValue(json,id,newValue) {
        json = modifyPropJson(json, subfields.Checked.name, newValue);
        return json;
    }
    
    //#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.applyFieldTypeLogicToContent = applyFieldTypeLogicToContent;
    this.updatePropValue = updatePropValue;
    this.value = value;
    this.contentDivId = contentDivId;
    this.propId = propId;
    this.propName = propName;
    this.subfields = subfields;
    this.fieldType = CswSubFields_Map.Button;
    this.showLabel = false;
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeButton
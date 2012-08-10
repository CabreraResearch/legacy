/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />
(function($) {

    $.CswFieldTypeFactory = function(method) {
        "use strict";
        var pluginName = 'CswFieldTypeFactory';

        var m = {
            nodeid: '',
            fieldtype: '',
            propDiv: '',
            saveBtn: '',
            propData: '',
            onChange: function() {
            },
            onReload: function() {
            },    // if a control needs to reload the tab
            cswnbtnodekey: '',
            relatednodeid: '',
            relatednodename: '',
            relatednodetypeid: '',
            relatedobjectclassid: '',
            ID: '',
            Required: '',
            ReadOnly: '',
            EditMode: Csw.enums.editMode.Edit,
            Multi: false,
            onEditView: function() {
            },
            onAfterButtonClick: function() {
            }
        };

        var methods = {
            'make': function(options) {
                if (options) {
                    //Csw.extend(m, options);
                    m = options;
                }
                m.ID = Csw.makeId(m.propDiv.getId(), m.propData.id);
                m.Required = Csw.bool(m.propData.required);
                m.ReadOnly = m.ReadOnly || Csw.bool(m.propData.readonly) || m.EditMode === Csw.enums.editMode.PrintReport;

                switch (m.fieldtype) {
                case "AuditHistoryGrid":
                    m.propDiv.$.CswFieldTypeAuditHistoryGrid('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Barcode":
                    m.propDiv.$.CswFieldTypeBarcode('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Button":
                    m.propDiv.$.CswFieldTypeButton('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Comments":
                    m.propDiv.$.CswFieldTypeComments('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Composite":
                    m.propDiv.$.CswFieldTypeComposite('init', m); //'init', nodeid, propData, onChange
                    break;
                case "DateTime":
                    m.propDiv.$.CswFieldTypeDateTime('init', m); //'init', nodeid, propData, onChange
                    break;
                case "File":
                    m.propDiv.$.CswFieldTypeFile('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Grid":
                    m.propDiv.$.CswFieldTypeGrid('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Image":
                    m.propDiv.$.CswFieldTypeImage('init', m); //'init', nodeid, propData, onChange
                    break;
                case "ImageList":
                    m.propDiv.$.CswFieldTypeImageList('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Link":
                    m.propDiv.$.CswFieldTypeLink('init', m); //'init', nodeid, propData, onChange
                    break;
                case "List":
                    m.propDiv.$.CswFieldTypeList('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Location":
                    m.propDiv.$.CswFieldTypeLocation('init', m); //'init', nodeid, propData, onChange
                    break;
                case "LocationContents":
                    m.propDiv.$.CswFieldTypeLocationContents('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Logical":
                    m.propDiv.$.CswFieldTypeLogical('init', m); //'init', nodeid, propData, onChange
                    break;
                case "LogicalSet":
                    m.propDiv.$.CswFieldTypeLogicalSet('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Memo":
                    m.propDiv.$.CswFieldTypeMemo('init', m); //'init', nodeid, propData, onChange
                    break;
                case "MOL":
                    m.propDiv.$.CswFieldTypeMol('init', m); //'init', nodeid, propData, onChange
                    break;
                case "MTBF":
                    m.propDiv.$.CswFieldTypeMTBF('init', m); //'init', nodeid, propData, onChange
                    break;
                case "MultiList":
                    m.propDiv.$.CswFieldTypeMultiList('init', m); //'init', nodeid, propData, onChange
                    break;
                case "NFPA":
                    m.propDiv.$.CswFieldTypeNFPA('init', m); //'init', nodeid, propData, onChange
                    break;
                case "NodeTypeSelect":
                    m.propDiv.$.CswFieldTypeNodeTypeSelect('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Number":
                    m.propDiv.$.CswFieldTypeNumber('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Password":
                    m.propDiv.$.CswFieldTypePassword('init', m); //'init', nodeid, propData, onChange
                    break;
                case "PropertyReference":
                    m.propDiv.$.CswFieldTypePropertyReference('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Quantity":
                    m.propDiv.$.CswFieldTypeQuantity('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Question":
                    m.propDiv.$.CswFieldTypeQuestion('init', m); //'init', nodeid, propData, onChange
                    break;
                case "Relationship":
                    m.propDiv.$.CswFieldTypeRelationship('init', m); //('init', nodeid, propData, onChange);
                    break;
                case "Scientific":
                    m.propDiv.$.CswFieldTypeScientific('init', m); //('init', nodeid, propData, onChange);
                    break;
                case "Sequence":
                    m.propDiv.$.CswFieldTypeSequence('init', m); //('init', nodeid, propData, onChange);
                    break;
                case "Static":
                    m.propDiv.$.CswFieldTypeStatic('init', m); //('init', nodeid, propData, onChange);
                    break;
                case "Text":
                    m.propDiv.$.CswFieldTypeText('init', m); //('init', nodeid, propData, onChange);
                    break;
                //				case "Time":     
                //					m.propDiv.$.CswFieldTypeTime('init', m); //('init', nodeid, propData, onChange);     
                //					break;     
                case "TimeInterval":
                    m.propDiv.$.CswFieldTypeTimeInterval('init', m); //('init', nodeid, propData, onChange);
                    break;
                case "UserSelect":
                    m.propDiv.$.CswFieldTypeUserSelect('init', m); //('init', nodeid, propData, onChange);
                    break;
                case "ViewPickList":
                    m.propDiv.$.CswFieldTypeViewPickList('init', m); //('init', nodeid, propData, onChange);
                    break;
                case "ViewReference":
                    m.propDiv.$.CswFieldTypeViewReference('init', m); //('init', nodeid, propData, onChange);
                    break;
                default:
                    m.propDiv.$.append(m.propData.gestalt);
                    Csw.error.showError({
                        'type': 'Error',
                        'message': 'Unrecognized Field Type',
                        'detail': 'CswFieldTypeFactory.make: Unrecognized Field Type: ' + m.fieldtype,
                        'display': true
                    });
                    break;
                } // switch (fieldtype)
            }, // make

            'save': function(options) {
                if (options) {
                    //Csw.extend(m, options);
                    m = options;
                }
                m.ID = Csw.makeId(m.propDiv.getId(), m.propData.id);
                m.Required = Csw.bool(m.propData.required);
                m.ReadOnly = Csw.bool(m.propData.readonly);

                switch (m.fieldtype) {
                case "Barcode":
                    m.propDiv.$.CswFieldTypeBarcode('save', m); //('save', $propdiv, propData);
                    break;
                case "Button":
                    m.propDiv.$.CswFieldTypeButton('save', m); //('save', $propdiv, propData);
                    break;
                case "Comments":
                    m.propDiv.$.CswFieldTypeComments('save', m); //('save', $propdiv, propData);
                    break;
                case "Composite":
                    m.propDiv.$.CswFieldTypeComposite('save', m); //('save', $propdiv, propData);
                    break;
                case "DateTime":
                    m.propDiv.$.CswFieldTypeDateTime('save', m); //('save', $propdiv, propData);
                    break;
                case "File":
                    m.propDiv.$.CswFieldTypeFile('save', m); //('save', $propdiv, propData);
                    break;
                case "Grid":
                    m.propDiv.$.CswFieldTypeGrid('save', m); //('save', $propdiv, propData, cswnbtnodekey);
                    break;
                case "Image":
                    m.propDiv.$.CswFieldTypeImage('save', m); //('save', $propdiv, propData);
                    break;
                case "ImageList":
                    m.propDiv.$.CswFieldTypeImageList('save', m); //('save', $propdiv, propData);
                    break;
                case "Link":
                    m.propDiv.$.CswFieldTypeLink('save', m); //('save', $propdiv, propData);
                    break;
                case "List":
                    m.propDiv.$.CswFieldTypeList('save', m); //('save', $propdiv, propData);
                    break;
                case "Location":
                    m.propDiv.$.CswFieldTypeLocation('save', m); //('save', $propdiv, propData);
                    break;
                case "LocationContents":
                    m.propDiv.$.CswFieldTypeLocationContents('save', m); //('save', $propdiv, propData);
                    break;
                case "Logical":
                    m.propDiv.$.CswFieldTypeLogical('save', m); //('save', $propdiv, propData);
                    break;
                case "LogicalSet":
                    m.propDiv.$.CswFieldTypeLogicalSet('save', m); //('save', $propdiv, propData);
                    break;
                case "Memo":
                    m.propDiv.$.CswFieldTypeMemo('save', m); //('save', $propdiv, propData);
                    break;
                case "MOL":
                    m.propDiv.$.CswFieldTypeMol('save', m); //('save', $propdiv, propData);
                    break;
                case "MTBF":
                    m.propDiv.$.CswFieldTypeMTBF('save', m); //('save', $propdiv, propData);
                    break;
                case "MultiList":
                    m.propDiv.$.CswFieldTypeMultiList('save', m); //('save', $propdiv, propData);
                    break;
                case "NFPA":
                    m.propDiv.$.CswFieldTypeNFPA('save', m); //('save', $propdiv, propData);
                    break;
                case "NodeTypeSelect":
                    m.propDiv.$.CswFieldTypeNodeTypeSelect('save', m); //('save', $propdiv, propData);
                    break;
                case "Number":
                    m.propDiv.$.CswFieldTypeNumber('save', m); //('save', $propdiv, propData);
                    break;
                case "Password":
                    m.propDiv.$.CswFieldTypePassword('save', m); //('save', $propdiv, propData);
                    break;
                case "PropertyReference":
                    m.propDiv.$.CswFieldTypePropertyReference('save', m); //('save', $propdiv, propData);
                    break;
                case "Quantity":
                    m.propDiv.$.CswFieldTypeQuantity('save', m); //('save', $propdiv, propData);
                    break;
                case "Question":
                    m.propDiv.$.CswFieldTypeQuestion('save', m); //('save', $propdiv, propData);
                    break;
                case "Relationship":
                    m.propDiv.$.CswFieldTypeRelationship('save', m); //('save', $propdiv, propData);
                    break;
                case "Scientific":
                    m.propDiv.$.CswFieldTypeScientific('save', m); //('save', $propdiv, propData);
                    break;
                case "Sequence":
                    m.propDiv.$.CswFieldTypeSequence('save', m); //('save', $propdiv, propData);
                    break;
                case "Static":
                    m.propDiv.$.CswFieldTypeStatic('save', m); //('save', $propdiv, propData);
                    break;
                case "Text":
                    m.propDiv.$.CswFieldTypeText('save', m); //('save', $propdiv, propData);
                    break;
                //				case "Time":     
                //					m.propDiv.$.CswFieldTypeTime('save', m); //('save', $propdiv, propData);     
                //					break;     
                case "TimeInterval":
                    m.propDiv.$.CswFieldTypeTimeInterval('save', m); //('save', $propdiv, propData);
                    break;
                case "UserSelect":
                    m.propDiv.$.CswFieldTypeUserSelect('save', m); //('save', $propdiv, propData);
                    break;
                case "ViewPickList":
                    m.propDiv.$.CswFieldTypeViewPickList('save', m); //('save', $propdiv, propData);
                    break;
                case "ViewReference":
                    m.propDiv.$.CswFieldTypeViewReference('save', m); //('save', $propdiv, propData);
                    break;
                default:
                    Csw.error.showError({
                        'type': 'Error',
                        'message': 'Unrecognized Field Type',
                        'detail': 'CswFieldTypeFactory.save: Unrecognized Field Type: ' + m.fieldtype,
                        'display': true
                    });
                    break;
                } // switch(fieldtype)
            } // save
        };

        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
            return false;
        }
    };        // $.CswFieldTypeFactory
}(jQuery));
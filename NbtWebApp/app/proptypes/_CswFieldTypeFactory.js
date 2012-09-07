/// <reference path="~/app/CswApp-vsdoc.js" />

(function ($) {

    $.CswFieldTypeFactory = function (method) {
        "use strict";
        var pluginName = 'CswFieldTypeFactory';

        var m = {
            nodeid: '',
            fieldtype: '',
            propDiv: '',
            saveBtn: '',
            propData: '',
            onChange: function () {
            },
            onReload: function () {
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
            onEditView: function () {
            },
            onAfterButtonClick: function () {
            }
        };

        var methods = {
            'make': function (options) {
                if (options) {
                    //Csw.extend(m, options);
                    m = options;
                }
                m.ID = Csw.makeId(m.propDiv.getId(), m.propData.id);
                m.Required = Csw.bool(m.propData.required);
                m.ReadOnly = m.ReadOnly || Csw.bool(m.propData.readonly) || m.EditMode === Csw.enums.editMode.PrintReport;

                switch (m.fieldtype) {
                    case "AuditHistoryGrid":
                    case "Barcode":
                    case "Button":
                    case "Comments":
                    case "Composite":
                    case "DateTime":
                    case "File":
                    case "Grid": 
                    case "Image": 
                    case "ImageList":
                    case "Link": 
                    case "List": 
                    case "Location": 
                    case "LocationContents": break; //keep the refactored props in the switch until _factory is completely removed
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
                return m;
            }, // make

            'save': function (options) {
                if (options) {
                    //Csw.extend(m, options);
                    m = options;
                }
                m.ID = Csw.makeId(m.propDiv.getId(), m.propData.id);
                m.Required = Csw.bool(m.propData.required);
                m.ReadOnly = Csw.bool(m.propData.readonly);

                switch (m.fieldtype) {
                    case "AuditHistoryGrid":
                    case "Barcode":
                    case "Button":
                    case "Comments":
                    case "Composite":
                    case "DateTime":
                    case "File":
                    case "Grid":
                    case "Image": 
                    case "ImageList": 
                    case "Link": 
                    case "List": 
                    case "Location": break; //keep the refactored props in the switch until _factory is completely removed
                    case "LocationContents": break; //keep the refactored props in the switch until _factory is completely removed
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
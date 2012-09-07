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
                    case "Logical": break; //keep the refactored props in the switch until _factory is completely removed
                    case "LogicalSet": break; //keep the refactored props in the switch until _factory is completely removed
                    case "NodeTypeSelect": break; //keep the refactored props in the switch until _factory is completely removed
                    case "UserSelect": break; //keep the refactored props in the switch until _factory is completely removed
                    case "ViewPickList": break; //keep the refactored props in the switch until _factory is completely removed    

                    case "Memo": break; //keep the refactored props in the switch until _factory is completely removed    
                    case "MOL":
                        m.propDiv.$.CswFieldTypeMol('init', m); 
                        break;
                    case "MTBF":
                        m.propDiv.$.CswFieldTypeMTBF('init', m); 
                        break;
                    case "MultiList": break; //keep the refactored props in the switch until _factory is completely removed    
                    case "NFPA":
                        m.propDiv.$.CswFieldTypeNFPA('init', m); 
                        break;
                    
                    case "Number": break; //keep the refactored props in the switch until _factory is completely removed    
                    case "Password":
                        m.propDiv.$.CswFieldTypePassword('init', m); 
                        break;
                    case "PropertyReference":
                        m.propDiv.$.CswFieldTypePropertyReference('init', m); 
                        break;
                    case "Quantity":
                        m.propDiv.$.CswFieldTypeQuantity('init', m); 
                        break;
                    case "Question":
                        m.propDiv.$.CswFieldTypeQuestion('init', m); 
                        break;
                    case "Relationship":
                        m.propDiv.$.CswFieldTypeRelationship('init', m); 
                        break;
                    case "Scientific":
                        m.propDiv.$.CswFieldTypeScientific('init', m); 
                        break;
                    case "Sequence": break; //keep the refactored props in the switch until _factory is completely removed    
                    case "Static": break; //keep the refactored props in the switch until _factory is completely removed    
                    case "Text":
                        m.propDiv.$.CswFieldTypeText('init', m); 
                        break;
                    case "TimeInterval":
                        m.propDiv.$.CswFieldTypeTimeInterval('init', m); 
                        break;
                    
                    case "ViewReference":
                        m.propDiv.$.CswFieldTypeViewReference('init', m); 
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
                    case "Logical": break; //keep the refactored props in the switch until _factory is completely removed
                    case "LogicalSet": break; //keep the refactored props in the switch until _factory is completely removed
                    case "NodeTypeSelect": break; //keep the refactored props in the switch until _factory is completely removed
                    case "UserSelect": break; //keep the refactored props in the switch until _factory is completely removed
                    case "ViewPickList": break; //keep the refactored props in the switch until _factory is completely removed
                    case "Memo": break; //keep the refactored props in the switch until _factory is completely removed    
                    case "MOL":
                        m.propDiv.$.CswFieldTypeMol('save', m); 
                        break;
                    case "MTBF":
                        m.propDiv.$.CswFieldTypeMTBF('save', m); 
                        break;
                    case "MultiList": break; //keep the refactored props in the switch until _factory is completely removed    
                    case "NFPA":
                        m.propDiv.$.CswFieldTypeNFPA('save', m); 
                        break;
                    case "Number": break; //keep the refactored props in the switch until _factory is completely removed    
                    case "Password":
                        m.propDiv.$.CswFieldTypePassword('save', m); 
                        break;
                    case "PropertyReference":
                        m.propDiv.$.CswFieldTypePropertyReference('save', m); 
                        break;
                    case "Quantity":
                        m.propDiv.$.CswFieldTypeQuantity('save', m); 
                        break;
                    case "Question":
                        m.propDiv.$.CswFieldTypeQuestion('save', m); 
                        break;
                    case "Relationship":
                        m.propDiv.$.CswFieldTypeRelationship('save', m); 
                        break;
                    case "Scientific":
                        m.propDiv.$.CswFieldTypeScientific('save', m); 
                        break;
                    case "Sequence": break; //keep the refactored props in the switch until _factory is completely removed    
                    case "Static": break; //keep the refactored props in the switch until _factory is completely removed    
                    case "Text":
                        m.propDiv.$.CswFieldTypeText('save', m); 
                        break;
                    case "TimeInterval":
                        m.propDiv.$.CswFieldTypeTimeInterval('save', m); 
                        break;
                    case "ViewReference":
                        m.propDiv.$.CswFieldTypeViewReference('save', m);
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
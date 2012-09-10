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
                    case "Scientific":
                        m.propDiv.$.CswFieldTypeScientific('init', m); 
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


                    case "Scientific":
                        m.propDiv.$.CswFieldTypeScientific('save', m); 
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
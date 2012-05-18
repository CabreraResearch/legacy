/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.nbt.viewPropFilter = Csw.nbt.viewPropFilter ||
        Csw.nbt.register('viewPropFilter', function (options) {
            'use strict';

            var cswPrivateVar = {
                ID: '',
                parent: '',      // this must be a table

                viewid: '',         // primary key of view from which filter came
                viewJson: '',       // current JSON for view
                propsData: '',      // property definitional data, will be populated from webservice if not supplied and proparbitraryid is supplied

                proparbitraryid: '',    // provide one of these to uniquely identify the filter
                filtarbitraryid: '',    // provide one of these to uniquely identify the filter
                viewbuilderpropid: '',  // provide one of these to uniquely identify the filter

                propname: '',             // default will be populated from propsData if not supplied
                selectedSubFieldName: '', // default will be populated from propsData if not supplied
                selectedFilterMode: '',   // default will be populated from propsData if not supplied
                selectedValue: '',        // default will be populated from propsData if not supplied

                showPropertyName: true,   // whether to show the property name
                showSubfield: true,       // whether to show the subfield
                showFilterMode: true,     // whether to show the filter mode
                showValue: true,          // whether to show the filter value
                
                readOnly: false,    // render all controls as static text instead of form elements

                propRow: 1,                  // starting row for rendering filter in table
                firstColumn: 3,              // starting column for rendering filter in table
                autoFocusInput: false,       // focus on filter value input
                $clickOnEnter: null,           // control to assign a clickOnEnter event, for value input
                //allowNullFilterValue: false,  // include null filters in JSON


                // Populated internally, do not override:
                table: null,
                propNameCell: null,
                subFieldCell: null,
                filterModeCell: null,
                valueCell: null,
                propNameControl: null,
                subfieldControl: null,
                filterModeControl: null,
                valueControl: null,
                selectedSubFieldJson: {}
            };

            var cswPublicRet = {};

            
            cswPrivateVar.makePropFilterId = function(id) {
                var delimiter = '_';
                var idParams = {
                    ID: id,
                    prefix: cswPrivateVar.ID,
                    suffix: ''
                };

                if (false == Csw.isNullOrEmpty(cswPrivateVar.filtarbitraryid)) {
                    idParams.ID = id + delimiter + 'filtarbitraryid';
                    idParams.suffix = cswPrivateVar.filtarbitraryid;
                }
                else if (false == Csw.isNullOrEmpty(cswPrivateVar.viewbuilderpropid)) {
                    idParams.ID = id + delimiter + 'viewbuilderpropid';
                    idParams.suffix = cswPrivateVar.viewbuilderpropid;
                }
                else if (false == Csw.isNullOrEmpty(cswPrivateVar.proparbitraryid)) {
                    idParams.ID = id + delimiter + 'proparbitraryid';
                    idParams.suffix = cswPrivateVar.proparbitraryid;
                }

                return Csw.makeId(idParams);
            }; // makePropFilterId()


            cswPrivateVar.makePropNameControl = function() {
                cswPrivateVar.propNameCell.empty();
                cswPrivateVar.propNameControl = cswPrivateVar.propNameCell.span({ 
                    ID: cswPrivateVar.makePropFilterId('propname'),
                    text: cswPrivateVar.propname,
                    nobr: true
                });
            }; // makePropNameControl()


            cswPrivateVar.makeSubfieldControl = function () {
                var subfields = (Csw.contains(cswPrivateVar.propsData, 'subfields')) ? cswPrivateVar.propsData.subfields : [];
                var subFieldOptions = [];
                var subfieldid = cswPrivateVar.makePropFilterId('filter_subfield');

                cswPrivateVar.subFieldCell.empty();
                if(cswPrivateVar.readOnly)
                {
                    cswPrivateVar.subfieldControl = cswPrivateVar.subFieldCell.span({ 
                        ID: subfieldid,
                        text: cswPrivateVar.selectedSubFieldName
                    });
                } else {
                    Csw.each(subfields, function(thisSubField, subfieldname) {
                        subFieldOptions.push({ value: thisSubField.column, display: subfieldname });
                        if( subfieldname === cswPrivateVar.selectedSubFieldName || thisSubField.column === cswPrivateVar.selectedSubFieldName) {
                            cswPrivateVar.selectedSubFieldJson = thisSubField;
                        }
                    });

                    cswPrivateVar.subfieldControl = cswPrivateVar.subFieldCell.select({ 
                        ID: subfieldid,
                        values: subFieldOptions,
                        selected: cswPrivateVar.selectedSubFieldName,
                        onChange: function () {
                            cswPrivateVar.selectedSubFieldName = cswPrivateVar.subfieldControl.val();
                            cswPrivateVar.renderPropFiltRow();
                        }
                    });
                } // if-else(cswPrivateVar.readOnly)
            }; // makeSubfieldPicklist()


            cswPrivateVar.makeFilterModeControl = function() {
                var filterModeOptions = [];
                var filtermodeid = cswPrivateVar.makePropFilterId('filter_mode');

                cswPrivateVar.filterModeCell.empty();
                if(cswPrivateVar.readOnly)
                {
                    cswPrivateVar.filterModeControl = cswPrivateVar.filterModeCell.span({ 
                        ID: filtermodeid,
                        text: cswPrivateVar.selectedFilterMode
                    });
                } else {
                    if (Csw.contains(cswPrivateVar.selectedSubFieldJson, 'filtermodes')) {
                        Csw.each(cswPrivateVar.selectedSubFieldJson.filtermodes, function(thisMode, mode) {
                            filterModeOptions.push({ value: mode, display: thisMode });
                        });
                    }

                    cswPrivateVar.filterModeControl = cswPrivateVar.filterModeCell.select({ 
                        ID: filtermodeid,
                        values: filterModeOptions,
                        selected: cswPrivateVar.selectedFilterMode,
                        onChange: function () {
                            cswPrivateVar.selectedFilterMode = cswPrivateVar.filterModeControl.val();
                            cswPrivateVar.renderPropFiltRow();
                        }
                    });
                } // if-else(cswPrivateVar.readOnly)
            }; // makeFilterModePicklist()


            cswPrivateVar.makeFilterValueControl = function() {
                var fieldtype = cswPrivateVar.propsData.fieldtype;
                var valueOptionDefs = (Csw.contains(cswPrivateVar.propsData, 'filtersoptions')) ? cswPrivateVar.propsData.filtersoptions.options : {};
                var valueOptions = [];
                var valueId = cswPrivateVar.makePropFilterId('propfilter_input');
                var placeholder = cswPrivateVar.propname;

                cswPrivateVar.valueCell.empty();
                if(cswPrivateVar.readOnly)
                {
                    cswPrivateVar.valueControl = cswPrivateVar.valueCell.span({ 
                        ID: valueId,
                        text: cswPrivateVar.selectedValue
                    });
                } else {
                    // DATETIME
                    if (fieldtype === Csw.enums.subFieldsMap.DateTime.name) {
                        cswPrivateVar.valueControl = cswPrivateVar.valueCell.dateTimePicker({
                            ID: valueId,
                            Date: cswPrivateVar.selectedValue,
                            //Time: '',
//                            DateFormat: Csw.serverDateFormatToJQuery(cswPrivateVar.propsData.dateformat),
//                            TimeFormat: Csw.serverTimeFormatToJQuery(cswPrivateVar.propsData.timeformat),
                            DisplayMode: 'Date',
                            ReadOnly: false,
                            Required: false,
                            showTodayButton: true,
                            onChange: function() {
                                cswPrivateVar.selectedValue = Csw.string(cswPrivateVar.valueControl.val().date);
                            }
                        });
                    // LIST
                    } else if (fieldtype === Csw.enums.subFieldsMap.List.name) {
                        valueOptions.push({ value: '', display: '' });
                        Csw.each(valueOptionDefs, function(optionValue, optionName) {
                            valueOptions.push({ 
                                value: Csw.string(optionValue).trim(), 
                                display: Csw.string(optionName).trim() 
                            });
                        });
                        cswPrivateVar.valueControl = cswPrivateVar.valueCell.select({ 
                            ID: valueId,
                            values: valueOptions,
                            selected: cswPrivateVar.selectedValue,
                            onChange: function() {
                                cswPrivateVar.selectedValue = cswPrivateVar.valueControl.val();
                            }
                        });
                    // LOGICAL
                    } else if (fieldtype === Csw.enums.subFieldsMap.Logical.name) {
                        cswPrivateVar.valueControl = cswPrivateVar.valueCell.triStateCheckBox({ 
                            ID: valueId,
                            Checked: cswPrivateVar.selectedValue,   // tristate, not bool
                            onChange: function() {
                                cswPrivateVar.selectedValue = cswPrivateVar.valueControl.val();
                            }
                        });
                    // DEFAULT (textbox)
                    } else {
                        if (Csw.isNullOrEmpty(cswPrivateVar.selectedValue)) {
                            if (placeholder !== cswPrivateVar.subfieldControl.val()) {
                                placeholder += "'s " + cswPrivateVar.subfieldControl.val();
                            }
                        }
                        cswPrivateVar.valueControl = cswPrivateVar.valueCell.input({
                            ID: valueId,
                            type: Csw.enums.inputTypes.text,
                            value: cswPrivateVar.selectedValue,
                            placeholder: placeholder,
                            width: "100px",
                            autofocus: cswPrivateVar.autoFocusInput,
                            autocomplete: 'on',
                            onChange: function() {
                                cswPrivateVar.selectedValue = cswPrivateVar.valueControl.val();
                            }
                        });
                        if(false === Csw.isNullOrEmpty(cswPrivateVar.$clickOnEnter))
                        {
                            cswPrivateVar.valueControl.$.clickOnEnter(cswPrivateVar.$clickOnEnter);
                        }
                    }

                    if (cswPrivateVar.filterModeControl.val() === 'Null' || cswPrivateVar.filterModeControl.val() === 'NotNull') {
                        cswPrivateVar.valueControl.hide();
                    }
                } // if(cswPrivateVar.readOnly)
            }; // makeFilterValueControl()


            cswPrivateVar.renderPropFiltRow = function() {
                cswPrivateVar.makePropNameControl();
                cswPrivateVar.makeSubfieldControl();
                cswPrivateVar.makeFilterModeControl();
                cswPrivateVar.makeFilterValueControl();
            }; // renderPropFiltRow()


            cswPublicRet.getFilterJson = function () {
                var retJson = {};

//                    nodetypeorobjectclassid = (cswPrivateVar.propsData.nodetypepropid === Csw.Int32MinVal) ? cswPrivateVar.propsData.objectclasspropid : cswPrivateVar.propsData.nodetypepropid;
//                    if (Csw.isNullOrEmpty(nodetypeorobjectclassid)) {
//                        nodetypeorobjectclassid = Csw.string(cswPrivateVar.nodetypeorobjectclassid);
//                    }

//                // workaround for case 26287
//                cswPrivateVar.selectedSubFieldName = cswPrivateVar.subfieldControl.val();
//                cswPrivateVar.selectedFilterMode = cswPrivateVar.filterModeControl.val();
//                cswPrivateVar.selectedValue = cswPrivateVar.valueControl.val();

                retJson = {
                    //nodetypeorobjectclassid: nodetypeorobjectclassid,
                    proptype: Csw.string(cswPrivateVar.proptype, cswPrivateVar.relatedidtype),
                    viewbuilderpropid: cswPrivateVar.viewbuilderpropid,
                    filtarbitraryid: cswPrivateVar.filtarbitraryid,
                    proparbitraryid: cswPrivateVar.proparbitraryid,
                    relatedidtype: cswPrivateVar.relatedidtype,
                    subfield: cswPrivateVar.selectedFilterMode,
                    filter: cswPrivateVar.selectedFilterMode,
                    filtervalue: cswPrivateVar.selectedValue.trim()
                };
                return retJson;
            }; // getFilterJson()


            cswPublicRet.makeFilter = function (options) {
                var o = {
                    viewJson: cswPublicRet.getFilterJson(),
                    filtJson: '',
                    onSuccess: null //function ($filterXml) {}
                };
                if (options) $.extend(o, options);

                var jsonData = {
                    PropFiltJson: JSON.stringify(o.filtJson),
                    ViewJson: JSON.stringify(o.viewJson)
                };

                Csw.ajax.post({
                    url: '/NbtWebApp/wsNBT.asmx/makeViewPropFilter',
                    data: jsonData,
                    success: function (data) {
                        if (Csw.isFunction(o.onSuccess)) {
                            o.onSuccess(data);
                        }
                    }
                });
            }; // makefilter()
            

//            cswPublicRet.bindToButton = function (btn) {
//                if (false == Csw.isNullOrEmpty(btn)) {
//                    cswPrivateVar.subfieldControl.$.clickOnEnter(btn.$);
//                    cswPrivateVar.filterModeControl.$.clickOnEnter(btn.$);
//                    cswPrivateVar.valueControl.$.clickOnEnter(btn.$);
//                }
//                return btn;
//            } // bindToButton()

            cswPrivateVar.setInitialValues = function() {

                if(Csw.isNullOrEmpty(cswPrivateVar.propname)) {
                    cswPrivateVar.propname = cswPrivateVar.propsData.propname;
                }
                                                
                if(Csw.isNullOrEmpty(cswPrivateVar.selectedSubFieldName)) {
                    cswPrivateVar.selectedSubFieldName = Csw.string(cswPrivateVar.propsData.defaultsubfield, 
                                                               Csw.string(cswPrivateVar.propsData.subfieldname, 
                                                                          cswPrivateVar.propsData.subfield));
                }
                if(Csw.isNullOrEmpty(cswPrivateVar.selectedFilterMode)) {
                    cswPrivateVar.selectedFilterMode = Csw.string(cswPrivateVar.propsData.defaultfilter, cswPrivateVar.propsData.filtermode);
                }
                if(Csw.isNullOrEmpty(cswPrivateVar.selectedValue)) {
                    cswPrivateVar.selectedValue = Csw.string(cswPrivateVar.propsData.value, 
                                                        (Csw.contains(cswPrivateVar.propsData, 'filtersoptions')) ? cswPrivateVar.propsData.filtersoptions.selected : '');

                }
            }; // setInitialValues()

            // constructor
            (function () {
                if (options) $.extend(cswPrivateVar, options);

                cswPrivateVar.table = cswPrivateVar.parent;
                if(Csw.isNullOrEmpty(cswPrivateVar.table.controlName) || cswPrivateVar.table.controlName !== 'table') {
                    Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.error.name, "Javascript Error", "csw.viewpropfilter was not called on a Table"));
                } else {

                    cswPrivateVar.propNameCell = cswPrivateVar.table.cell(cswPrivateVar.propRow, cswPrivateVar.firstColumn).empty();
                    cswPrivateVar.subFieldCell = cswPrivateVar.table.cell(cswPrivateVar.propRow, cswPrivateVar.firstColumn + 1).empty();
                    cswPrivateVar.filterModeCell = cswPrivateVar.table.cell(cswPrivateVar.propRow, cswPrivateVar.firstColumn + 2).empty();
                    cswPrivateVar.valueCell = cswPrivateVar.table.cell(cswPrivateVar.propRow, cswPrivateVar.firstColumn + 3).empty();

                    if (false === Csw.bool(cswPrivateVar.showPropertyName)) {
                        cswPrivateVar.propNameCell.hide();
                    }
                    if (false === Csw.bool(cswPrivateVar.showSubfield)) {
                        cswPrivateVar.subFieldCell.hide();
                    }
                    if (false === Csw.bool(cswPrivateVar.showFilterMode)) {
                        cswPrivateVar.filterModeCell.hide();
                    }
                    if (false === Csw.bool(cswPrivateVar.showValue)) {
                        cswPrivateVar.valueCell.hide();
                    }

                    if (Csw.isNullOrEmpty(cswPrivateVar.propsData) && false === Csw.isNullOrEmpty(cswPrivateVar.proparbitraryid)) {
                        var viewJson = '';
                        if (false === Csw.isNullOrEmpty(cswPrivateVar.viewJson)) {
                            viewJson = JSON.stringify(cswPrivateVar.viewJson);
                        }

                        Csw.ajax.post({
                            urlMethod: 'getViewPropFilterUI',
                            //async: false,
                            data: {
                                ViewJson: viewJson,
                                ViewId: cswPrivateVar.viewid,
                                PropArbitraryId: cswPrivateVar.proparbitraryid
                            },
                            success: function (data) {
                                cswPrivateVar.propsData = data;
                                cswPrivateVar.setInitialValues();
                                cswPrivateVar.renderPropFiltRow();
                            } // success
                        }); //ajax
                    } // if (Csw.isNullOrEmpty(cswPrivateVar.propsData) && false === Csw.isNullOrEmpty(cswPrivateVar.proparbitraryid)) {
                    else {
                        cswPrivateVar.setInitialValues();
                        cswPrivateVar.renderPropFiltRow();
                    }
                
                } // if-else(Csw.isNullOrEmpty(cswPrivateVar.table.controlName) || cswPrivateVar.table.controlName !== 'table') {
            })(); // constructor

            return cswPublicRet;
        }); // register
})();

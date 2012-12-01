/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.nbt.viewPropFilter = Csw.nbt.viewPropFilter ||
        Csw.nbt.register('viewPropFilter', function (options) {
            'use strict';

            var cswPrivate = {
                name: '',
                parent: '',      // this must be a table

                viewid: '',         // primary key of view from which filter came
                viewJson: '',       // current JSON for view
                propsData: '',      // property definitional data, will be populated from webservice if not supplied and proparbitraryid is supplied

                proparbitraryid: '',    // provide one of these to uniquely identify the filter
                filtarbitraryid: '',    // provide one of these to uniquely identify the filter
                viewbuilderpropid: '',  // provide one of these to uniquely identify the filter

                propname: '',             // default will be populated from propsData if not supplied
                selectedConjunction: '',  // default will be populated from propsData if not supplied
                selectedSubFieldName: '', // default will be populated from propsData if not supplied
                selectedFilterMode: '',   // default will be populated from propsData if not supplied
                selectedValue: '',        // default will be populated from propsData if not supplied

                showPropertyName: true,   // whether to show the property name
                showConjunction: true,    // whether to show the conjunction
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

            var cswPublic = {};

            
            cswPrivate.makePropFilterId = function(id) {
                var delimiter = '_';
                var idParams = {
                    name: id,
                    prefix: cswPrivate.name,
                    suffix: ''
                };

                if (false == Csw.isNullOrEmpty(cswPrivate.filtarbitraryid)) {
                    idParams.name = id + delimiter + 'filtarbitraryid';
                    idParams.suffix = cswPrivate.filtarbitraryid;
                }
                else if (false == Csw.isNullOrEmpty(cswPrivate.viewbuilderpropid)) {
                    idParams.name = id + delimiter + 'viewbuilderpropid';
                    idParams.suffix = cswPrivate.viewbuilderpropid;
                }
                else if (false == Csw.isNullOrEmpty(cswPrivate.proparbitraryid)) {
                    idParams.name = id + delimiter + 'proparbitraryid';
                    idParams.suffix = cswPrivate.proparbitraryid;
                }

                return Csw.delimitedString(idParams, {delimiter: delimiter}).string();
            }; // makePropFilterId()


            cswPrivate.makePropNameControl = function() {
                cswPrivate.propNameCell.empty();
                cswPrivate.propNameControl = cswPrivate.propNameCell.span({ 
                    name: cswPrivate.makePropFilterId('propname'),
                    text: cswPrivate.propname,
                    nobr: true
                });
            }; // makePropNameControl()


            cswPrivate.makeConjunctionControl = function () {
                var conjunctionOptions = ['And','Or'];
                var conjunctionId = cswPrivate.makePropFilterId('filter_conjunction');

                cswPrivate.conjunctionCell.empty();
                if(cswPrivate.readOnly)
                {
                    cswPrivate.conjunctionControl = cswPrivate.conjunctionCell.span({ 
                        name: conjunctionId,
                        text: cswPrivate.selectedConjunction
                    });
                } else {
                    cswPrivate.conjunctionControl = cswPrivate.conjunctionCell.select({ 
                        name: conjunctionId,
                        values: conjunctionOptions,
                        selected: cswPrivate.selectedConjunction,
                        onChange: function () {
                            cswPrivate.selectedConjunction = cswPrivate.conjunctionControl.val();
                            //cswPrivate.renderPropFiltRow();
                        }
                    });
                } // if-else(cswPrivate.readOnly)
            }; // makeConjunctionControl()


            cswPrivate.makeSubfieldControl = function () {
                var subfields = (Csw.contains(cswPrivate.propsData, 'subfields')) ? cswPrivate.propsData.subfields : [];
                var subFieldOptions = [];
                var subfieldid = cswPrivate.makePropFilterId('filter_subfield');

                cswPrivate.subFieldCell.empty();
                if(cswPrivate.readOnly)
                {
                    cswPrivate.subfieldControl = cswPrivate.subFieldCell.span({ 
                        name: subfieldid,
                        text: cswPrivate.selectedSubFieldName
                    });
                } else {
                    Csw.each(subfields, function(thisSubField, subfieldname) {
                        subFieldOptions.push({ value: thisSubField.column, display: subfieldname });
                        if( subfieldname === cswPrivate.selectedSubFieldName || thisSubField.column === cswPrivate.selectedSubFieldName) {
                            cswPrivate.selectedSubFieldJson = thisSubField;
                        }
                    });

                    cswPrivate.subfieldControl = cswPrivate.subFieldCell.select({ 
                        name: subfieldid,
                        values: subFieldOptions,
                        selected: cswPrivate.selectedSubFieldName,
                        onChange: function () {
                            cswPrivate.selectedSubFieldName = cswPrivate.subfieldControl.val();
                            cswPrivate.renderPropFiltRow();
                        }
                    });
                } // if-else(cswPrivate.readOnly)
            }; // makeSubfieldControl()


            cswPrivate.makeFilterModeControl = function() {
                var filterModeOptions = [];
                var filtermodeid = cswPrivate.makePropFilterId('filter_mode');

                cswPrivate.filterModeCell.empty();
                if(cswPrivate.readOnly)
                {
                    cswPrivate.filterModeControl = cswPrivate.filterModeCell.span({ 
                        name: filtermodeid,
                        text: cswPrivate.selectedFilterMode
                    });
                } else {
                    if (Csw.contains(cswPrivate.selectedSubFieldJson, 'filtermodes')) {
                        Csw.each(cswPrivate.selectedSubFieldJson.filtermodes, function(thisMode, mode) {
                            filterModeOptions.push({ value: mode, display: thisMode });
                        });
                    }

                    cswPrivate.filterModeControl = cswPrivate.filterModeCell.select({ 
                        name: filtermodeid,
                        values: filterModeOptions,
                        selected: cswPrivate.selectedFilterMode,
                        onChange: function () {
                            cswPrivate.selectedFilterMode = cswPrivate.filterModeControl.val();
                            cswPrivate.renderPropFiltRow();
                        }
                    });
                } // if-else(cswPrivate.readOnly)
            }; // makeFilterModeControl()


            cswPrivate.makeFilterValueControl = function() {
                var fieldtype = cswPrivate.propsData.fieldtype;
                var valueOptionDefs = (Csw.contains(cswPrivate.propsData, 'filtersoptions')) ? cswPrivate.propsData.filtersoptions.options : {};
                var valueOptions = [];
                var valueId = cswPrivate.makePropFilterId('propfilter_input');
                var placeholder = cswPrivate.propname;

                cswPrivate.valueCell.empty();
                if(cswPrivate.readOnly)
                {
                    cswPrivate.valueControl = cswPrivate.valueCell.span({ 
                        name: valueId,
                        text: cswPrivate.selectedValue
                    });
                } else {
                    // DATETIME
                    if (fieldtype === Csw.enums.subFieldsMap.DateTime.name) {
                        cswPrivate.valueControl = cswPrivate.valueCell.dateTimePicker({
                            name: valueId,
                            Date: cswPrivate.selectedValue,
                            //Time: '',
//                            DateFormat: Csw.serverDateFormatToJQuery(cswPrivate.propsData.dateformat),
//                            TimeFormat: Csw.serverTimeFormatToJQuery(cswPrivate.propsData.timeformat),
                            DisplayMode: 'Date',
                            ReadOnly: false,
                            isRequired: false,
                            showTodayButton: true,
                            onChange: function() {
                                cswPrivate.selectedValue = Csw.string(cswPrivate.valueControl.val().date);
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
                        cswPrivate.valueControl = cswPrivate.valueCell.select({ 
                            name: valueId,
                            values: valueOptions,
                            selected: cswPrivate.selectedValue,
                            onChange: function() {
                                cswPrivate.selectedValue = cswPrivate.valueControl.val();
                            }
                        });
                    // LOGICAL
                    } else if (fieldtype === Csw.enums.subFieldsMap.Logical.name) {
                        cswPrivate.valueControl = cswPrivate.valueCell.triStateCheckBox({ 
                            name: valueId,
                            checked: cswPrivate.selectedValue,   // tristate, not bool
                            onChange: function() {
                                cswPrivate.selectedValue = cswPrivate.valueControl.val();
                            }
                        });
                    // DEFAULT (textbox)
                    } else {
                        if (Csw.isNullOrEmpty(cswPrivate.selectedValue)) {
                            if (placeholder !== cswPrivate.subfieldControl.selectedText()) {
                                placeholder += "'s " + cswPrivate.subfieldControl.selectedText();
                            }
                        }
                        cswPrivate.valueControl = cswPrivate.valueCell.input({
                            name: valueId,
                            type: Csw.enums.inputTypes.text,
                            value: cswPrivate.selectedValue,
                            placeholder: placeholder,
                            width: "150px",
                            autofocus: cswPrivate.autoFocusInput,
                            autocomplete: 'on',
                            onChange: function() {
                                cswPrivate.selectedValue = cswPrivate.valueControl.val();
                            }
                        });
                        if(false === Csw.isNullOrEmpty(cswPrivate.$clickOnEnter))
                        {
                            cswPrivate.valueControl.$.clickOnEnter(cswPrivate.$clickOnEnter);
                        }
                    }

                    if (cswPrivate.filterModeControl.val() === 'Null' || cswPrivate.filterModeControl.val() === 'NotNull') {
                        cswPrivate.valueControl.hide();
                        cswPrivate.selectedValue = '';
                    }
                } // if(cswPrivate.readOnly)
            }; // makeFilterValueControl()


            cswPrivate.renderPropFiltRow = function() {
                cswPrivate.makePropNameControl();
                cswPrivate.makeConjunctionControl();
                cswPrivate.makeSubfieldControl();
                cswPrivate.makeFilterModeControl();
                cswPrivate.makeFilterValueControl();
            }; // renderPropFiltRow()


            cswPublic.getFilterJson = function () {
                var retJson = {};

//                    nodetypeorobjectclassid = (cswPrivate.propsData.nodetypepropid === Csw.Int32MinVal) ? cswPrivate.propsData.objectclasspropid : cswPrivate.propsData.nodetypepropid;
//                    if (Csw.isNullOrEmpty(nodetypeorobjectclassid)) {
//                        nodetypeorobjectclassid = Csw.string(cswPrivate.nodetypeorobjectclassid);
//                    }

//                // workaround for case 26287
//                cswPrivate.selectedSubFieldName = cswPrivate.subfieldControl.val();
//                cswPrivate.selectedFilterMode = cswPrivate.filterModeControl.val();
//                cswPrivate.selectedValue = cswPrivate.valueControl.val();

                retJson = {
                    //nodetypeorobjectclassid: nodetypeorobjectclassid,
                    proptype: Csw.string(cswPrivate.proptype, cswPrivate.relatedidtype),
                    viewbuilderpropid: cswPrivate.viewbuilderpropid,
                    filtarbitraryid: cswPrivate.filtarbitraryid,
                    proparbitraryid: cswPrivate.proparbitraryid,
                    relatedidtype: cswPrivate.relatedidtype,
                    conjunction: cswPrivate.selectedConjunction,
                    subfieldname: cswPrivate.selectedSubFieldName,
                    filter: cswPrivate.selectedFilterMode,
                    filtervalue: cswPrivate.selectedValue.trim()
                };
                return retJson;
            }; // getFilterJson()


            cswPublic.makeFilter = function (options) {
                var o = {
                    viewJson: cswPublic.getFilterJson(),
                    filtJson: '',
                    onSuccess: null //function ($filterXml) {}
                };
                if (options) Csw.extend(o, options);

                var jsonData = {
                    PropFiltJson: JSON.stringify(o.filtJson),
                    ViewJson: JSON.stringify(o.viewJson)
                };

                Csw.ajax.post({
                    urlMethod: 'makeViewPropFilter',
                    data: jsonData,
                    success: function (data) {
                        if (Csw.isFunction(o.onSuccess)) {
                            o.onSuccess(data);
                        }
                    }
                });
            }; // makefilter()
            

//            cswPublic.bindToButton = function (btn) {
//                if (false == Csw.isNullOrEmpty(btn)) {
//                    cswPrivate.subfieldControl.$.clickOnEnter(btn.$);
//                    cswPrivate.filterModeControl.$.clickOnEnter(btn.$);
//                    cswPrivate.valueControl.$.clickOnEnter(btn.$);
//                }
//                return btn;
//            } // bindToButton()

            cswPrivate.setInitialValues = function() {

                if(Csw.isNullOrEmpty(cswPrivate.propname)) {
                    cswPrivate.propname = cswPrivate.propsData.propname;
                }
                                                
                if(Csw.isNullOrEmpty(cswPrivate.selectedConjunction)) {
                    cswPrivate.selectedConjunction = Csw.string(cswPrivate.propsData.defaultconjunction, cswPrivate.propsData.conjunction);
                }
                if(Csw.isNullOrEmpty(cswPrivate.selectedSubFieldName)) {
                    cswPrivate.selectedSubFieldName = Csw.string(cswPrivate.propsData.defaultsubfield, 
                                                               Csw.string(cswPrivate.propsData.subfieldname, 
                                                                          cswPrivate.propsData.subfield));
                }
                if(Csw.isNullOrEmpty(cswPrivate.selectedFilterMode)) {
                    cswPrivate.selectedFilterMode = Csw.string(cswPrivate.propsData.defaultfilter, cswPrivate.propsData.filtermode);
                }
                if(Csw.isNullOrEmpty(cswPrivate.selectedValue)) {
                    cswPrivate.selectedValue = Csw.string(cswPrivate.propsData.value, 
                                                        (Csw.contains(cswPrivate.propsData, 'filtersoptions')) ? cswPrivate.propsData.filtersoptions.selected : '');

                }
            }; // setInitialValues()

            // constructor
            (function () {
                if (options) Csw.extend(cswPrivate, options);

                cswPrivate.table = cswPrivate.parent;
                if(Csw.isNullOrEmpty(cswPrivate.table.controlName) || cswPrivate.table.controlName !== 'table') {
                    Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.error.name, "Javascript Error", "csw.viewpropfilter was not called on a Table"));
                } else {

                    cswPrivate.conjunctionCell = cswPrivate.table.cell(cswPrivate.propRow, cswPrivate.firstColumn).empty();
                    cswPrivate.propNameCell = cswPrivate.table.cell(cswPrivate.propRow, cswPrivate.firstColumn + 1).empty();
                    cswPrivate.subFieldCell = cswPrivate.table.cell(cswPrivate.propRow, cswPrivate.firstColumn + 2).empty();
                    cswPrivate.filterModeCell = cswPrivate.table.cell(cswPrivate.propRow, cswPrivate.firstColumn + 3).empty();
                    cswPrivate.valueCell = cswPrivate.table.cell(cswPrivate.propRow, cswPrivate.firstColumn + 4).empty();

                    if (false === Csw.bool(cswPrivate.showPropertyName)) {
                        cswPrivate.propNameCell.hide();
                    }
                    if (false === Csw.bool(cswPrivate.showConjunction)) {
                        cswPrivate.conjunctionCell.css({visibility: 'hidden'});  // this one gets mixed, so we want it to take up space
                    }
                    if (false === Csw.bool(cswPrivate.showSubfield)) {
                        cswPrivate.subFieldCell.hide();
                    }
                    if (false === Csw.bool(cswPrivate.showFilterMode)) {
                        cswPrivate.filterModeCell.hide();
                    }
                    if (false === Csw.bool(cswPrivate.showValue)) {
                        cswPrivate.valueCell.hide();
                    }

                    if (Csw.isNullOrEmpty(cswPrivate.propsData) && false === Csw.isNullOrEmpty(cswPrivate.proparbitraryid)) {
                        var viewJson = '';
                        if (false === Csw.isNullOrEmpty(cswPrivate.viewJson)) {
                            viewJson = JSON.stringify(cswPrivate.viewJson);
                        }

                        Csw.ajax.post({
                            urlMethod: 'getViewPropFilterUI',
                            //async: false,
                            data: {
                                ViewJson: viewJson,
                                ViewId: cswPrivate.viewid,
                                PropArbitraryId: cswPrivate.proparbitraryid
                            },
                            success: function (data) {
                                cswPrivate.propsData = data;
                                cswPrivate.setInitialValues();
                                cswPrivate.renderPropFiltRow();
                            } // success
                        }); //ajax
                    } // if (Csw.isNullOrEmpty(cswPrivate.propsData) && false === Csw.isNullOrEmpty(cswPrivate.proparbitraryid)) {
                    else {
                        cswPrivate.setInitialValues();
                        cswPrivate.renderPropFiltRow();
                    }
                
                } // if-else(Csw.isNullOrEmpty(cswPrivate.table.controlName) || cswPrivate.table.controlName !== 'table') {
            })(); // constructor

            return cswPublic;
        }); // register
})();

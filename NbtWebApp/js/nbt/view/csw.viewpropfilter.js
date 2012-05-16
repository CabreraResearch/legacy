/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.nbt.viewPropFilter = Csw.nbt.viewPropFilter ||
        Csw.nbt.register('viewPropFilter', function (options) {
            'use strict';

            var internal = {
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

            var external = {};

            
            internal.makePropFilterId = function(id) {
                var delimiter = '_';
                var idParams = {
                    ID: id,
                    prefix: internal.ID,
                    suffix: ''
                };

                if (false == Csw.isNullOrEmpty(internal.filtarbitraryid)) {
                    idParams.ID = id + delimiter + 'filtarbitraryid';
                    idParams.suffix = internal.filtarbitraryid;
                }
                else if (false == Csw.isNullOrEmpty(internal.viewbuilderpropid)) {
                    idParams.ID = id + delimiter + 'viewbuilderpropid';
                    idParams.suffix = internal.viewbuilderpropid;
                }
                else if (false == Csw.isNullOrEmpty(internal.proparbitraryid)) {
                    idParams.ID = id + delimiter + 'proparbitraryid';
                    idParams.suffix = internal.proparbitraryid;
                }

                return Csw.makeId(idParams);
            }; // makePropFilterId()


            internal.makePropNameControl = function() {
                internal.propNameCell.empty();
                internal.propNameControl = internal.propNameCell.span({ 
                    ID: internal.makePropFilterId('propname'),
                    text: internal.propname,
                    nobr: true
                });
            }; // makePropNameControl()


            internal.makeSubfieldControl = function () {
                var subfields = (Csw.contains(internal.propsData, 'subfields')) ? internal.propsData.subfields : [];
                var subFieldOptions = [];
                var subfieldid = internal.makePropFilterId('filter_subfield');

                internal.subFieldCell.empty();
                if(internal.readOnly)
                {
                    internal.subfieldControl = internal.subFieldCell.span({ 
                        ID: subfieldid,
                        text: internal.selectedSubFieldName
                    });
                } else {
                    Csw.each(subfields, function(thisSubField, subfieldname) {
                        subFieldOptions.push({ value: thisSubField.column, display: subfieldname });
                        if( subfieldname === internal.selectedSubFieldName || thisSubField.column === internal.selectedSubFieldName) {
                            internal.selectedSubFieldJson = thisSubField;
                        }
                    });

                    internal.subfieldControl = internal.subFieldCell.select({ 
                        ID: subfieldid,
                        values: subFieldOptions,
                        selected: internal.selectedSubFieldName,
                        onChange: function () {
                            internal.selectedSubFieldName = internal.subfieldControl.val();
                            internal.renderPropFiltRow();
                        }
                    });
                } // if-else(internal.readOnly)
            }; // makeSubfieldPicklist()


            internal.makeFilterModeControl = function() {
                var filterModeOptions = [];
                var filtermodeid = internal.makePropFilterId('filter_mode');

                internal.filterModeCell.empty();
                if(internal.readOnly)
                {
                    internal.filterModeControl = internal.filterModeCell.span({ 
                        ID: filtermodeid,
                        text: internal.selectedFilterMode
                    });
                } else {
                    if (Csw.contains(internal.selectedSubFieldJson, 'filtermodes')) {
                        Csw.each(internal.selectedSubFieldJson.filtermodes, function(thisMode, mode) {
                            filterModeOptions.push({ value: mode, display: thisMode });
                        });
                    }

                    internal.filterModeControl = internal.filterModeCell.select({ 
                        ID: filtermodeid,
                        values: filterModeOptions,
                        selected: internal.selectedFilterMode,
                        onChange: function () {
                            internal.selectedFilterMode = internal.filterModeControl.val();
                            internal.renderPropFiltRow();
                        }
                    });
                } // if-else(internal.readOnly)
            }; // makeFilterModePicklist()


            internal.makeFilterValueControl = function() {
                var fieldtype = internal.propsData.fieldtype;
                var valueOptionDefs = (Csw.contains(internal.propsData, 'filtersoptions')) ? internal.propsData.filtersoptions.options : {};
                var valueOptions = [];
                var valueId = internal.makePropFilterId('propfilter_input');
                var placeholder = internal.propname;

                internal.valueCell.empty();
                if(internal.readOnly)
                {
                    internal.valueControl = internal.valueCell.span({ 
                        ID: valueId,
                        text: internal.selectedValue
                    });
                } else {
                    if (fieldtype === Csw.enums.subFieldsMap.List.name) {
                        valueOptions.push({ value: '', display: '' });
                        Csw.each(valueOptionDefs, function(optionValue, optionName) {
                            valueOptions.push({ 
                                value: Csw.string(optionValue).trim(), 
                                display: Csw.string(optionName).trim() 
                            });
                        });
                        internal.valueControl = internal.valueCell.select({ 
                            ID: valueId,
                            values: valueOptions,
                            selected: internal.selectedValue,
                            onChange: function() {
                                internal.selectedValue = internal.valueControl.val();
                            }
                        });
                    } else if (fieldtype === Csw.enums.subFieldsMap.Logical.name) {
                        internal.valueControl = internal.valueCell.triStateCheckBox({ 
                            ID: valueId,
                            Checked: internal.selectedValue,   // tristate, not bool
                            onChange: function() {
                                internal.selectedValue = internal.valueControl.val();
                            }
                        });
                    } else {
                        if (Csw.isNullOrEmpty(internal.selectedValue)) {
                            if (placeholder !== internal.subfieldControl.val()) {
                                placeholder += "'s " + internal.subfieldControl.val();
                            }
                        }
                        internal.valueControl = internal.valueCell.input({
                            ID: valueId,
                            type: Csw.enums.inputTypes.text,
                            value: internal.selectedValue,
                            placeholder: placeholder,
                            width: "100px",
                            autofocus: internal.autoFocusInput,
                            autocomplete: 'on',
                            onChange: function() {
                                internal.selectedValue = internal.valueControl.val();
                            }
                        });
                        internal.valueControl.$.clickOnEnter(internal.$clickOnEnter);
                    }

                    if (internal.filterModeControl.val() === 'Null' || internal.filterModeControl.val() === 'NotNull') {
                        internal.valueControl.hide();
                    }
                } // if(internal.readOnly)
            }; // makeFilterValueControl()


            internal.renderPropFiltRow = function() {
                internal.makePropNameControl();
                internal.makeSubfieldControl();
                internal.makeFilterModeControl();
                internal.makeFilterValueControl();
            }; // renderPropFiltRow()


            external.getFilterJson = function () {
                var retJson = {};

//                    nodetypeorobjectclassid = (internal.propsData.nodetypepropid === Csw.Int32MinVal) ? internal.propsData.objectclasspropid : internal.propsData.nodetypepropid;
//                    if (Csw.isNullOrEmpty(nodetypeorobjectclassid)) {
//                        nodetypeorobjectclassid = Csw.string(internal.nodetypeorobjectclassid);
//                    }

                // workaround for case 26287
                internal.selectedSubFieldName = internal.subfieldControl.val();
                internal.selectedFilterMode = internal.filterModeControl.val();
                internal.selectedValue = internal.valueControl.val();

                retJson = {
                    //nodetypeorobjectclassid: nodetypeorobjectclassid,
                    proptype: Csw.string(internal.proptype, internal.relatedidtype),
                    viewbuilderpropid: internal.viewbuilderpropid,
                    filtarbitraryid: internal.filtarbitraryid,
                    proparbitraryid: internal.proparbitraryid,
                    relatedidtype: internal.relatedidtype,
                    subfield: internal.selectedFilterMode,
                    filter: internal.selectedFilterMode,
                    filtervalue: internal.selectedValue.trim()
                };
                return retJson;
            }; // getFilterJson()


            external.makeFilter = function (options) {
                var o = {
                    viewJson: external.getFilterJson(),
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
            

//            external.bindToButton = function (btn) {
//                if (false == Csw.isNullOrEmpty(btn)) {
//                    internal.subfieldControl.$.clickOnEnter(btn.$);
//                    internal.filterModeControl.$.clickOnEnter(btn.$);
//                    internal.valueControl.$.clickOnEnter(btn.$);
//                }
//                return btn;
//            } // bindToButton()

            internal.setInitialValues = function() {
                internal.propname = internal.propsData.propname;
                                
                if(Csw.isNullOrEmpty(internal.selectedSubFieldName)) {
                    internal.selectedSubFieldName = Csw.string(internal.propsData.defaultsubfield, 
                                                               Csw.string(internal.propsData.subfieldname, 
                                                                          internal.propsData.subfield));
                }
                if(Csw.isNullOrEmpty(internal.selectedFilterMode)) {
                    internal.selectedFilterMode = Csw.string(internal.propsData.defaultfilter, internal.propsData.filtermode);
                }
                if(Csw.isNullOrEmpty(internal.selectedValue)) {
                    internal.selectedValue = Csw.string(internal.propsData.value, 
                                                        (Csw.contains(internal.propsData, 'filtersoptions')) ? internal.propsData.filtersoptions.selected : '');

                }
            }; // setInitialValues()

            // constructor
            (function () {
                if (options) $.extend(internal, options);

                internal.table = internal.parent;
                if(Csw.isNullOrEmpty(internal.table.controlName) || internal.table.controlName !== 'table') {
                    Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.error.name, "Javascript Error", "csw.viewpropfilter was not called on a Table"));
                } else {

                    internal.propNameCell = internal.table.cell(internal.propRow, internal.firstColumn).empty();
                    internal.subFieldCell = internal.table.cell(internal.propRow, internal.firstColumn + 1).empty();
                    internal.filterModeCell = internal.table.cell(internal.propRow, internal.firstColumn + 2).empty();
                    internal.valueCell = internal.table.cell(internal.propRow, internal.firstColumn + 3).empty();

                    if (false === Csw.bool(internal.showPropertyName)) {
                        internal.propNameCell.hide();
                    }
                    if (false === Csw.bool(internal.showSubfield)) {
                        internal.subFieldCell.hide();
                    }
                    if (false === Csw.bool(internal.showFilterMode)) {
                        internal.filterModeCell.hide();
                    }
                    if (false === Csw.bool(internal.showValue)) {
                        internal.valueCell.hide();
                    }

                    if (Csw.isNullOrEmpty(internal.propsData) && false === Csw.isNullOrEmpty(internal.proparbitraryid)) {
                        var viewJson = '';
                        if (false === Csw.isNullOrEmpty(internal.viewJson)) {
                            viewJson = JSON.stringify(internal.viewJson);
                        }

                        Csw.ajax.post({
                            urlMethod: 'getViewPropFilterUI',
                            async: false,
                            data: {
                                ViewJson: viewJson,
                                ViewId: internal.viewid,
                                PropArbitraryId: internal.proparbitraryid
                            },
                            success: function (data) {
                                internal.propsData = data;
                                internal.setInitialValues();
                                internal.renderPropFiltRow();
                            } // success
                        }); //ajax
                    } // if (Csw.isNullOrEmpty(internal.propsData) && false === Csw.isNullOrEmpty(internal.proparbitraryid)) {
                    else {
                        internal.setInitialValues();
                        internal.renderPropFiltRow();
                    }
                
                } // if-else(Csw.isNullOrEmpty(internal.table.controlName) || internal.table.controlName !== 'table') {
            })(); // constructor

            return external;
        }); // register
})();

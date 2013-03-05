/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = "CswViewPropFilter";

    function makePropFilterId(id, options) {
        var filterId;
        var delimiter = '_';
        var o = {
            proparbitraryid: '',
            filtarbitraryid: '',
            viewbuilderpropid: '',
            name: ''
        };
        if (options) Csw.extend(o, options);

        if (!Csw.isNullOrEmpty(o.filtarbitraryid)) {
            filterId = id + delimiter + 'filtarbitraryid' + delimiter + o.name + delimiter + o.filtarbitraryid;
        }
        else if (!Csw.isNullOrEmpty(o.viewbuilderpropid)) {
            filterId = id + delimiter + 'viewbuilderpropid' + delimiter + o.name + delimiter + o.viewbuilderpropid;
        }
        else if (!Csw.isNullOrEmpty(o.proparbitraryid)) {
            filterId = id + delimiter + 'proparbitraryid' + delimiter + o.name + delimiter + o.proparbitraryid;
        }
        else if (!Csw.isNullOrEmpty(o.name)) {
            filterId = id + delimiter + o.name;
        } else {
            filterId = id;
        }
        return filterId;
    }

    var methods = {

        init: function (options) {
            var o = {
                //options
                viewid: '',
                viewJson: '',
                propsData: '',
                proparbitraryid: '',
                filtarbitraryid: '',
                viewbuilderpropid: '',
                propRow: 1,
                firstColumn: 3,
                includePropertyName: false,
                advancedIsHidden: false,
                selectedConjunction: '',
                selectedSubfieldVal: '',
                selectedFilterMode: '',
                selectedFilterVal: '',
                autoFocusInput: false
            };

            if (options) Csw.extend(o, options);

            var $table = $(this); //must call on a table
            var propFilterTable = Csw.literals.table($table);

            if (Csw.isNullOrEmpty(o.propsData) && false === Csw.isNullOrEmpty(o.proparbitraryid)) {
                var jsonData = {
                    ViewJson: '',
                    ViewId: o.viewid,
                    PropArbitraryId: o.proparbitraryid
                };
                if (false === Csw.isNullOrEmpty(o.viewJson)) {
                    jsonData.ViewJson = JSON.stringify(o.viewJson);
                }

                Csw.ajax.post({
                    urlMethod: 'getViewPropFilterUI',
                    async: false,
                    data: jsonData,
                    success: function (data) {
                        o.propsData = data;
                        renderPropFiltRow(o);
                    } //success
                }); //ajax
            }
            else {
                renderPropFiltRow(o);
            }

            function renderPropFiltRow(filtOpt) {
                var propsData = filtOpt.propsData,
                    propertyName = propsData.propname,
                    fieldtype = propsData.fieldtype,
                    defaultFilterModeVal = Csw.string(propsData.defaultfilter),
                    defaultConjunction = Csw.string(filtOpt.selectedConjunction, 'And'),
                    defaultSubfieldVal = Csw.string(filtOpt.selectedSubfieldVal, propsData.defaultsubfield),
                    defaultSubFieldId = makePropFilterId('default_filter', filtOpt),
                    filterModesId = makePropFilterId('filter_select', filtOpt),
                    filtValInputId = makePropFilterId('propfilter_input', filtOpt),
                    subfieldOptionsId = makePropFilterId('subfield_select', filtOpt),
                    conjunctionOptionsId = makePropFilterId('conjunctions_select', filtOpt),
                    conjunctions = ['And','Or'],
                    subFieldVals = [],
                    filterModeVals = [],
                    subfields = (Csw.contains(propsData, 'subfields')) ? propsData.subfields : [],
                    filtValOpt = (Csw.contains(propsData, 'filtersoptions')) ? propsData.filtersoptions.options : {},
                    filtValAry = [],
                    filtSelected = Csw.string(filtOpt.selectedFilterVal, (Csw.contains(propsData, 'filtersoptions')) ? propsData.filtersoptions.selected : {}),
                    placeholder = '',
                    conjunctionCell, subfieldCell, filterModesCell, propFilterValueCell, defaultSubField,
                    field, thisField, filtermodes, mode, thisMode, conjunctionList, subfieldsList, filterModesList, filt, filtInput;

                if (filtOpt.includePropertyName) {
                    //Row propRow, Column 3: property
                    propFilterTable.cell(filtOpt.propRow, filtOpt.firstColumn)
                        .empty()
                        .span({ text: propertyName, name: makePropFilterId(propertyName, filtOpt) }) //3
                }
                //Row propRow, Column 4: Conjunction Cell
                conjunctionCell = propFilterTable.cell(filtOpt.propRow, (filtOpt.firstColumn + 1)) //4
                    .empty();

                //Row propRow, Column 5: Subfield Cel
                subfieldCell = propFilterTable.cell(filtOpt.propRow, (filtOpt.firstColumn + 2)) //5
                    .empty();

                //Row propRow, Column 6: Filters cell
                filterModesCell = propFilterTable.cell(filtOpt.propRow, (filtOpt.firstColumn + 3)) //6
                    .empty();

                //Row propRow, Column 7: Input cell
                propFilterValueCell = propFilterTable.cell(filtOpt.propRow, (filtOpt.firstColumn + 4)) //7
                    .empty();

                // Conjunction picklist
                conjunctionList = conjunctionCell.select({ name: conjunctionOptionsId,
                    values: conjunctions,
                    selected: defaultConjunction,
                    cssclass: Csw.enums.cssClasses_ViewBuilder.conjunction_select.name,
                    onChange: function () {
                        filtOpt.selectedConjunction = conjunctionList.val();
                        //renderPropFiltRow(filtOpt);
                    }
                });
                
                //Subfield default value (hidden)
                defaultSubField = subfieldCell.span({
                    name: defaultSubFieldId,
                    value: defaultSubfieldVal,
                    cssclass: Csw.enums.cssClasses_ViewBuilder.default_filter.name
                })
                    .css({ 'text-align': "center" });
                //if (false === filtOpt.advancedIsHidden) {
                defaultSubField.hide();
                //}

                //Generate subfields and filters picklist arrays
                for (field in subfields) {
                    if (Csw.contains(subfields, field)) {
                        thisField = subfields[field];
                        subFieldVals.push({ value: thisField.column, display: field });
                        if ((field === defaultSubfieldVal || thisField.column === defaultSubfieldVal) && Csw.contains(thisField, 'filtermodes')) {
                            filtermodes = thisField.filtermodes;
                            for (mode in filtermodes) {
                                if (Csw.contains(filtermodes, mode)) {
                                    thisMode = filtermodes[mode];
                                    filterModeVals.push({ value: mode, display: thisMode });
                                }
                            }
                        }
                    }
                }

                //Subfield picklist
                subfieldsList = subfieldCell.select({ name: subfieldOptionsId,
                    values: subFieldVals,
                    selected: defaultSubfieldVal,
                    cssclass: Csw.enums.cssClasses_ViewBuilder.subfield_select.name,
                    onChange: function () {
                        var r = {
                            selectedSubfieldVal: subfieldsList.val(),
                            selectedFilterMode: '',
                            selectedFilterVal: '',
                            advancedIsHidden: Csw.bool(subfieldsList.$.is(':hidden'))
                        };
                        Csw.extend(filtOpt, r);
                        renderPropFiltRow(filtOpt);
                    }
                });

                if (filtOpt.advancedIsHidden) {
                    subfieldsList.hide();
                }

                //Filter picklist
                filterModesList = filterModesCell.select({ name: filterModesId,
                    values: filterModeVals,
                    selected: defaultFilterModeVal,
                    cssclass: Csw.enums.cssClasses_ViewBuilder.filter_select.name,
                    onChange: function () {
                        var r = {
                            selectedSubfieldVal: subfieldsList.val(),
                            selectedFilterMode: filterModesList.val(),
                            selectedFilterVal: '',
                            advancedIsHidden: Csw.bool(subfieldsList.$.is(':hidden'))
                        };
                        Csw.extend(filtOpt, r);
                        renderPropFiltRow(filtOpt);
                    }
                });

                if (false === Csw.isNullOrEmpty(filtOpt.selectedFilterMode)) {
                    filterModesList.val(filtOpt.selectedFilterMode).propDom('selected', true);
                }

                //                if (filtOpt.advancedIsHidden) {
                //                    filterModesList.hide();
                //                }

                //Filter input (value)
                if (fieldtype === Csw.enums.subFieldsMap.List.name) {
                    if (Csw.contains(propsData, 'filtersoptions')) {
                        filtValAry.push({ value: '', display: '' });
                        for (filt in filtValOpt) {
                            if (Csw.contains(filtValOpt, filt)) {
                                filtValAry.push({ value: Csw.string(filt).trim(), display: Csw.string(filtValOpt[filt]).trim() });
                            }
                        }
                        filtInput = propFilterValueCell.select({ name: filtValInputId,
                            values: filtValAry,
                            selected: filtSelected,
                            cssclass: Csw.enums.cssClasses_ViewBuilder.filter_value.name
                        });
                    }
                }
                else if (fieldtype === Csw.enums.subFieldsMap.Logical.name) {
                    filtInput = propFilterValueCell.triStateCheckBox({ name: filtValInputId,
                        checked: (defaultSubfieldVal === 'checked') ? 'true' : 'false',
                        cssclass: 'ViewPropFilterLogical ' + Csw.enums.cssClasses_ViewBuilder.filter_value.name
                    });
                } else {
                    if (Csw.isNullOrEmpty(defaultSubfieldVal)) {
                        placeholder = propertyName;
                        if (placeholder !== subfieldsList.selectedText()) {
                            placeholder += "'s " + subfieldsList.selectedText();
                        }
                    }
                    filtInput = propFilterValueCell.input({
                        name: filtValInputId,
                        type: Csw.enums.inputTypes.text,
                        cssclass: Csw.enums.cssClasses_ViewBuilder.filter_value.name,
                        value: '',
                        placeholder: placeholder,
                        width: "100px",
                        autofocus: filtOpt.autoFocusInput,
                        autocomplete: 'on'
                    });
                }
                if (false === Csw.isNullOrEmpty(filtInput, true)) {
                    filtInput.data('propsData', propsData);
                }
                if (filtOpt.selectedFilterMode === 'Null' ||
                    filtOpt.selectedFilterMode === 'NotNull') {
                    filtInput.hide();
                }
            }
            return propFilterTable.$;
        }, // 'init': function (options) {
        'static': function (options) {
            var o = {
                //options
                propsData: '',
                proparbitraryid: '',
                filtarbitraryid: '',
                viewbuilderpropid: '',
                propRow: 1,
                firstColumn: 3,
                includePropertyName: false,
                autoFocusInput: false
            };

            if (options) Csw.extend(o, options);

            var $table = $(this); //must call on a table
            var propFilterTable = Csw.literals.table($table);

            if (false === Csw.isNullOrEmpty(o.propsData)) {
                renderPropFiltRow(o);
            }

            function renderPropFiltRow(filtOpt) {
                var propsData = filtOpt.propsData;
                var propertyName = Csw.string(filtOpt.propname);

                if (filtOpt.includePropertyName) {
                    //Row propRow, Column 3: property
                    var propSelectCell = propFilterTable.cell(filtOpt.propRow, filtOpt.firstColumn) //3
                        .css({ 'padding': '2px' });

                    var propCellId = makePropFilterId(propertyName, filtOpt);
                    propSelectCell.span({ name: propCellId, value: propertyName });
                }

                var selectedConjunction = Csw.string(propsData.conjunction);
                var selectedSubfield = Csw.string(propsData.subfieldname, propsData.subfield);
                var selectedFilterMode = Csw.string(propsData.filtermode);
                var filterValue = Csw.string(propsData.value);

                //Row propRow, Column 4: Subfield Cell
                var conjunctionCell = propFilterTable.cell(filtOpt.propRow, (filtOpt.firstColumn + 1)) //4
                    .css({ 'padding': '2px' });
                var defaultConjunctionId = makePropFilterId('default_conjunction', filtOpt);

                //Row propRow, Column 5: Subfield Cell
                var subfieldCell = propFilterTable.cell(filtOpt.propRow, (filtOpt.firstColumn + 2)) //5
                    .css({ 'padding': '2px' });
                var defaultSubFieldId = makePropFilterId('default_filter', filtOpt);

                //Row propRow, Column 6: Filters cell
                var filtersCell = propFilterTable.cell(filtOpt.propRow, (filtOpt.firstColumn + 3)) //6
                    .css({ 'padding': '2px' });
                var filtersOptionsId = makePropFilterId('filter_select', filtOpt);

                //Row propRow, Column 7: Input cell
                var propFilterValueCell = propFilterTable.cell(filtOpt.propRow, (filtOpt.firstColumn + 4)) //7
                    .css({ 'padding': '2px' });
                var filtValInputId = makePropFilterId('propfilter_input', filtOpt);

                //Conjunction
                conjunctionCell.span({
                    name: defaultConjunctionId,
                    value: selectedConjunction,
                    cssclass: Csw.enums.cssClasses_ViewBuilder.default_filter.name
                })
                    .css({ 'text-align': "center" });
                //Subfield
                subfieldCell.span({
                    name: defaultSubFieldId,
                    value: selectedSubfield,
                    cssclass: Csw.enums.cssClasses_ViewBuilder.default_filter.name
                })
                    .css({ 'text-align': "center" });
                //Selected Filter
                filtersCell.span({
                    name: filtersOptionsId,
                    value: selectedFilterMode,
                    cssclass: Csw.enums.cssClasses_ViewBuilder.filter_select.name
                })
                    .css({ 'text-align': "center" });
                //Filter Input
                propFilterValueCell.span({
                    name: filtValInputId,
                    value: filterValue,
                    cssclass: Csw.enums.cssClasses_ViewBuilder.default_filter.name
                })
                    .css({ 'text-align': "center" })
                    .data('propsData', filtOpt.propsData);
            }
            return propFilterTable;
        }, // 'add': function (options) {
        getFilterJson: function (options) {
            var o = {
                filtJson: {},
                name: '',
                proparbitraryid: '',
                filtarbitraryid: '',
                allowNullFilterValue: false
            };
            if (options) Csw.extend(o, options);

            var $thisProp = $(this),
                retJson = {},
                $filtInput = $thisProp.find('.' + Csw.enums.cssClasses_ViewBuilder.filter_value.name),
                fieldtype = Csw.string(o.filtJson.fieldtype, o.fieldtype),
                filterValue, $conjunction, conjunctionText, $subField, subFieldText, $filter, filterText, nodetypeorobjectclassid;

            if ($filtInput.length > 1) {
                $filtInput = $filtInput.filter('#' + o.filtarbitraryid);
            }

            switch (fieldtype) {
                case Csw.enums.subFieldsMap.Logical.name:
                    filterValue = Csw.string($filtInput.val, $filtInput.attr('value'));
                    break;
                case Csw.enums.subFieldsMap.List.name:
                    filterValue = $filtInput.find(':selected').val();
                    break;
                default:
                    filterValue = $filtInput.val();
                    break;
            }

            if (false === Csw.isNullOrEmpty(filterValue) || o.allowNullFilterValue) {
                $subField = $thisProp.find('.' + Csw.enums.cssClasses_ViewBuilder.subfield_select.name);
                if ($subField.length > 1) {
                    $subField = $subField.filter('#' + o.filtarbitraryid);
                }
                subFieldText = $subField.find(':selected').text();

                $conjunction = $thisProp.find('.' + Csw.enums.cssClasses_ViewBuilder.conjunction_select.name);
                conjunctionText = $conjunction.find(':selected').text();

                $filter = $thisProp.find('.' + Csw.enums.cssClasses_ViewBuilder.filter_select.name);
                if ($filter.length > 1) {
                    $filter = $filter.filter('#' + o.filtarbitraryid);
                }
                filterText = $filter.find(':selected').val();
                nodetypeorobjectclassid = (o.filtJson.nodetypepropid === Csw.Int32MinVal) ? o.filtJson.objectclasspropid : o.filtJson.nodetypepropid;
                if (Csw.isNullOrEmpty(nodetypeorobjectclassid)) {
                    nodetypeorobjectclassid = Csw.string(o.nodetypeorobjectclassid);
                }

                retJson = {
                    nodetypeorobjectclassid: nodetypeorobjectclassid,
                    proptype: Csw.string(o.proptype, o.relatedidtype),
                    viewbuilderpropid: o.viewbuilderpropid,
                    filtarbitraryid: o.filtarbitraryid,
                    proparbitraryid: o.proparbitraryid,
                    relatedidtype: o.relatedidtype,
                    conjunction: conjunctionText,
                    subfieldname: subFieldText,
                    filter: filterText,
                    filtervalue: Csw.string(filterValue).trim()
                };

            } // if(filterValue !== '')
            return retJson;
        }, // 'getFilterJson': function (options) { 
        makeFilter: function (options) {
            var o = {
                viewJson: '',
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
        }, // 'makefilter': function (options)
        bindToButton: function () {
            var $button = $(this);

            if (!Csw.isNullOrEmpty($button)) {
                $('.' + Csw.enums.cssClasses_ViewBuilder.conjunction_select.name).each(function () {
                    var $input = $(this);
                    $input.clickOnEnter($button);
                });
                $('.' + Csw.enums.cssClasses_ViewBuilder.subfield_select.name).each(function () {
                    var $input = $(this);
                    $input.clickOnEnter($button);
                });
                $('.' + Csw.enums.cssClasses_ViewBuilder.filter_select.name).each(function () {
                    var $input = $(this);
                    $input.clickOnEnter($button);
                });
                $('.' + Csw.enums.cssClasses_ViewBuilder.default_filter.name).each(function () {
                    var $input = $(this);
                    $input.clickOnEnter($button);
                });
                $('.' + Csw.enums.cssClasses_ViewBuilder.filter_value.name).each(function () {
                    var $input = $(this);
                    $input.clickOnEnter($button);
                });
            }
            return $button;
        } // 'bindToButton': function (options)
    }; // methods 

    $.fn.CswViewPropFilter = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);



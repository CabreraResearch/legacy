/// <reference path="/js/../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../controls/CswSelect.js" />

(function ($) { /// <param name="$" type="jQuery" />

    var pluginName = "CswViewPropFilter";

    function makePropFilterId(id, options) {
        var filterId = '';
        var delimiter = '_';
        var o = {
            proparbitraryid: '',
            filtarbitraryid: '',
            viewbuilderpropid: '',
            ID: ''
        };
        if (options) $.extend(o, options);

        if (!isNullOrEmpty(o.filtarbitraryid)) {
            filterId = makeId({ ID: id + delimiter + 'filtarbitraryid',
                prefix: o.ID,
                suffix: o.filtarbitraryid
            });
        }
        else if (!isNullOrEmpty(o.viewbuilderpropid)) {
            filterId = makeId({ ID: id + delimiter + 'viewbuilderpropid',
                prefix: o.ID,
                suffix: o.viewbuilderpropid
            });
        }
        else if (!isNullOrEmpty(o.proparbitraryid)) {
            filterId = makeId({ ID: id + delimiter + 'proparbitraryid',
                prefix: o.ID,
                suffix: o.proparbitraryid
            });
        }
        else if (!isNullOrEmpty(o.ID)) {
            filterId = makeId({ ID: id,
                prefix: o.ID
            });
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
                selectedSubfieldVal: '',
                selectedFilterVal: '',
                autoFocusInput: false
            };

            if (options) $.extend(o, options);

            var $propFilterTable = $(this); //must call on a table

            if (isNullOrEmpty(o.propsData) && false === isNullOrEmpty(o.proparbitraryid)) {
                var jsonData = {
                    ViewJson: JSON.stringify(o.viewJson),
                    PropArbitraryId: o.proparbitraryid
                };

                CswAjaxJson({
                    url: '/NbtWebApp/wsNBT.asmx/getViewPropFilterUI',
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
                    defaultFilterModeVal = tryParseString(propsData.defaultfilter),
                    defaultSubfieldVal = tryParseString(filtOpt.selectedSubfieldVal, propsData.defaultsubfield),
                    defaultSubFieldId = makePropFilterId('default_filter', filtOpt),
                    filterModesId = makePropFilterId('filter_select', filtOpt),
                    filtValInputId = makePropFilterId('propfilter_input', filtOpt),
                    subfieldOptionsId = makePropFilterId('subfield_select', filtOpt),
                    subFieldVals = [],
                    filterModeVals = [],
                    subfields = (contains(propsData, 'subfields')) ? propsData.subfields : [],
                    filtValOpt = (contains(propsData, 'filtersoptions')) ? propsData.filtersoptions.options : {},
                    filtValAry = [],
                    filtSelected = (contains(propsData, 'filtersoptions')) ? propsData.filtersoptions.selected : {},
                    placeholder = '',
                    $propSelectCell, propCellId, $subfieldCell, $filterModesCell, $propFilterValueCell, $defaultSubField,
                    field, thisField, filtermodes, mode, thisMode, $subfieldsList, $filterModesList, filt, $filtInput;

                if (filtOpt.includePropertyName) {
                    //Row propRow, Column 3: property
                    $propSelectCell = $propFilterTable.CswTable('cell', filtOpt.propRow, filtOpt.firstColumn) //3
                                                      .empty();
                    propCellId = makePropFilterId(propertyName, filtOpt);
                    $propSelectCell.CswSpan('init', { ID: propCellId, value: propertyName });
                }

                //Row propRow, Column 4: Subfield Cell
                $subfieldCell = $propFilterTable.CswTable('cell', filtOpt.propRow, (filtOpt.firstColumn + 1)) //4
                                                .empty();

                //Row propRow, Column 5: Filters cell
                $filterModesCell = $propFilterTable.CswTable('cell', filtOpt.propRow, (filtOpt.firstColumn + 2)) //5
                                                   .empty();

                //Row propRow, Column 6: Input cell
                $propFilterValueCell = $propFilterTable.CswTable('cell', filtOpt.propRow, (filtOpt.firstColumn + 3)) //6
                                                       .empty();

                //Subfield default value (hidden)
                $defaultSubField = $subfieldCell.CswSpan('init', {
                    ID: defaultSubFieldId,
                    value: defaultSubfieldVal,
                    cssclass: ViewBuilder_CssClasses.default_filter.name
                })
                                                .css({ 'text-align': "center" });
                if (false === filtOpt.advancedIsHidden) {
                    $defaultSubField.hide();
                }

                //Generate subfields and filters picklist arrays
                for (field in subfields) {
                    if (subfields.hasOwnProperty(field)) {
                        thisField = subfields[field];
                        subFieldVals.push({ value: thisField.column, display: field });
                        if (field === defaultSubfieldVal && thisField.hasOwnProperty('filtermodes')) {
                            filtermodes = thisField.filtermodes;
                            for (mode in filtermodes) {
                                if (filtermodes.hasOwnProperty(mode)) {
                                    thisMode = filtermodes[mode];
                                    filterModeVals.push({ value: mode, display: thisMode });
                                }
                            }
                        }
                    }
                }

                //Subfield picklist
                $subfieldsList = $subfieldCell.CswSelect('init', { ID: subfieldOptionsId,
                    values: subFieldVals,
                    selected: defaultSubfieldVal,
                    cssclass: ViewBuilder_CssClasses.subfield_select.name,
                    onChange: function () {
                        var $this = $(this);
                        var r = {
                            selectedSubfieldVal: $this.val(),
                            selectedFilterVal: '',
                            advancedIsHidden: isTrue($this.is(':hidden'))
                        };
                        $.extend(filtOpt, r);
                        renderPropFiltRow(filtOpt);
                    }
                });

                if (filtOpt.advancedIsHidden) {
                    $subfieldsList.hide();
                }

                //Filter picklist
                $filterModesList = $filterModesCell.CswSelect('init', { ID: filterModesId,
                    values: filterModeVals,
                    selected: defaultFilterModeVal,
                    cssclass: ViewBuilder_CssClasses.filter_select.name,
                    onChange: function () {
                        var $this = $(this);
                        var r = {
                            selectedSubfieldVal: $subfieldsList.val(),
                            selectedFilterVal: $this.val(),
                            advancedIsHidden: isTrue($this.is(':hidden'))
                        };
                        $.extend(filtOpt, r);
                        renderPropFiltRow(filtOpt);
                    }
                });

                if (false === isNullOrEmpty(filtOpt.selectedFilterVal)) {
                    $filterModesList.val(filtOpt.selectedFilterVal).CswAttrDom('selected', true);
                }

                if (filtOpt.advancedIsHidden) {
                    $filterModesList.hide();
                }

                //Filter input (value)
                if (fieldtype === CswSubFields_Map.List.name) {
                    if (propsData.hasOwnProperty('filtersoptions')) {
                        for (filt in filtValOpt) {
                            if (filtValOpt.hasOwnProperty(filt)) {
                                filtValAry.push({ value: filt, display: filtValOpt[filt] });
                            }
                        }
                        $filtInput = $propFilterValueCell.CswSelect('init', { ID: filtValInputId,
                            values: filtValAry,
                            selected: filtSelected,
                            cssclass: ViewBuilder_CssClasses.filter_value.name
                        });
                    }
                }
                else if (fieldtype === CswSubFields_Map.Logical.name) {
                    $filtInput = $propFilterValueCell.CswTristateCheckBox('init', { ID: filtValInputId,
                        Checked: (defaultSubfieldVal === 'checked') ? 'true' : 'false',
                        cssclass: 'ViewPropFilterLogical ' + ViewBuilder_CssClasses.filter_value.name
                    });
                } else {
                    if (isNullOrEmpty(defaultSubfieldVal)) {
                        placeholder = propertyName;
                        if (placeholder !== $subfieldsList.find(':selected').text()) {
                            placeholder += "'s " + $subfieldsList.find(':selected').text();
                        }
                    }
                    $filtInput = $propFilterValueCell.CswInput('init', {
                        ID: filtValInputId,
                        type: CswInput_Types.text,
                        cssclass: ViewBuilder_CssClasses.filter_value.name,
                        value: '',
                        placeholder: placeholder,
                        width: "200px",
                        autofocus: filtOpt.autoFocusInput,
                        autocomplete: 'on'
                    });
                }
                if (false === isNullOrEmpty($filtInput, true)) {
                    $filtInput.data('propsData', propsData);
                }
            }
            return $propFilterTable;
        }, // 'init': function(options) {
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

            if (options) $.extend(o, options);

            var $propFilterTable = $(this); //must call on a table

            if (false === isNullOrEmpty(o.propsData)) {
                renderPropFiltRow(o);
            }

            function renderPropFiltRow(filtOpt) {
                var propsData = filtOpt.propsData;
                var propertyName = tryParseString(filtOpt.propname);

                if (filtOpt.includePropertyName) {
                    //Row propRow, Column 3: property
                    var $propSelectCell = $propFilterTable.CswTable('cell', filtOpt.propRow, filtOpt.firstColumn) //3
                                                          .empty()
                                                          .css('padding', '2px');

                    var propCellId = makePropFilterId(propertyName, filtOpt);
                    $propSelectCell.CswSpan('init', { ID: propCellId, value: propertyName });
                }

                var selectedSubfield = tryParseString(propsData.subfieldname);
                var selectedFilterMode = tryParseString(propsData.filtermode);
                var filterValue = tryParseString(propsData.value);

                //Row propRow, Column 4: Subfield Cell
                var $subfieldCell = $propFilterTable.CswTable('cell', filtOpt.propRow, (filtOpt.firstColumn + 1)) //4
                                                    .empty()
                                                    .css('padding', '2px');
                var defaultSubFieldId = makePropFilterId('default_filter', filtOpt);

                //Row propRow, Column 5: Filters cell
                var $filtersCell = $propFilterTable.CswTable('cell', filtOpt.propRow, (filtOpt.firstColumn + 2)) //5
                                                    .empty()
                                                    .css('padding', '2px');
                var filtersOptionsId = makePropFilterId('filter_select', filtOpt);

                //Row propRow, Column 6: Input cell
                var $propFilterValueCell = $propFilterTable.CswTable('cell', filtOpt.propRow, (filtOpt.firstColumn + 3)) //6
                                                    .empty()
                                                    .css('padding', '2px');
                var filtValInputId = makePropFilterId('propfilter_input', filtOpt);

                //Subfield
                $subfieldCell.CswSpan('init', {
                    ID: defaultSubFieldId,
                    value: selectedSubfield,
                    cssclass: ViewBuilder_CssClasses.default_filter.name
                })
                                .css({ 'text-align': "center" });
                //Selected Filter
                $filtersCell.CswSpan('init', {
                    ID: filtersOptionsId,
                    value: selectedFilterMode,
                    cssclass: ViewBuilder_CssClasses.filter_select.name
                })
                                .css({ 'text-align': "center" });
                //Filter Input
                $propFilterValueCell.CswSpan('init', {
                    ID: filtValInputId,
                    value: filterValue,
                    cssclass: ViewBuilder_CssClasses.default_filter.name
                })
                                .css({ 'text-align': "center" })
                                .data('propsData', filtOpt.propsData);
            }
            return $propFilterTable;
        }, // 'add': function(options) {
        getFilterJson: function (options) {
            var o = {
                filtJson: {},
                ID: '',
                allowNullFilterValue: false
            };
            if (options) $.extend(o, options);

            var $thisProp = $(this),
                retJson = {},
                $filtInput = $thisProp.find('.' + ViewBuilder_CssClasses.filter_value.name),
                fieldtype = tryParseString(o.filtJson.fieldtype, o.fieldtype),
                filterValue, $subField, subFieldText, $filter, filterText, nodetypeorobjectclassid;

            switch (fieldtype) {
                case CswSubFields_Map.Logical.name:
                    filterValue = $filtInput.CswTristateCheckBox('value');
                    break;
                case CswSubFields_Map.List.name:
                    filterValue = $filtInput.find(':selected').val();
                    break;
                default:
                    filterValue = $filtInput.val();
                    break;
            }

            if (false === isNullOrEmpty(filterValue) || o.allowNullFilterValue) {
                $subField = $thisProp.find('.' + ViewBuilder_CssClasses.subfield_select.name);
                subFieldText = $subField.find(':selected').text();

                $filter = $thisProp.find('.' + ViewBuilder_CssClasses.filter_select.name);
                filterText = $filter.find(':selected').val();
                nodetypeorobjectclassid = (o.filtJson.nodetypepropid === Int32MinVal) ? o.filtJson.objectclasspropid : o.filtJson.nodetypepropid;
                if (isNullOrEmpty(nodetypeorobjectclassid)) {
                    nodetypeorobjectclassid = tryParseString(o.nodetypeorobjectclassid);
                }

                retJson = {
                    nodetypeorobjectclassid: nodetypeorobjectclassid,
                    proptype: tryParseString(o.filtJson.type, o.relatedidtype),
                    viewbuilderpropid: o.viewbuilderpropid,
                    filtarbitraryid: o.filtarbitraryid,
                    proparbitraryid: o.proparbitraryid,
                    relatedidtype: o.relatedidtype,
                    subfield: subFieldText,
                    filter: filterText,
                    filtervalue: filterValue
                };

            } // if(filterValue !== '')
            return retJson;
        }, // 'getFilterJson': function(options) { 
        makeFilter: function (options) {
            var o = {
                viewJson: '',
                filtJson: '',
                onSuccess: null //function($filterXml) {}
            };
            if (options) $.extend(o, options);

            var jsonData = {
                PropFiltJson: JSON.stringify(o.filtJson),
                ViewJson: JSON.stringify(o.viewJson)
            };

            CswAjaxJson({
                url: '/NbtWebApp/wsNBT.asmx/makeViewPropFilter',
                data: jsonData,
                success: function (data) {
                    if (isFunction(o.onSuccess)) {
                        o.onSuccess(data);
                    }
                }
            });
        }, // 'makefilter': function(options)
        bindToButton: function () {
            var $button = $(this);

            if (!isNullOrEmpty($button)) {
                $('.' + ViewBuilder_CssClasses.subfield_select.name).each(function () {
                    var $input = $(this);
                    $input.clickOnEnter($button);
                });
                $('.' + ViewBuilder_CssClasses.filter_select.name).each(function () {
                    var $input = $(this);
                    $input.clickOnEnter($button);
                });
                $('.' + ViewBuilder_CssClasses.default_filter.name).each(function () {
                    var $input = $(this);
                    $input.clickOnEnter($button);
                });
                $('.' + ViewBuilder_CssClasses.filter_value.name).each(function () {
                    var $input = $(this);
                    $input.clickOnEnter($button);
                });
            }
            return $button;
        } // 'bindToButton': function(options)
    } // methods 

    $.fn.CswViewPropFilter = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }

    };
})(jQuery);



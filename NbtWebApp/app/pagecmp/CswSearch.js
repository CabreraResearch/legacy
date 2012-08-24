///// <reference path="~/app/CswApp-vsdoc.js" />
//

//(function ($) {
//    "use strict";
//    $.fn.CswSearch = function (options) {

//        var o = {
//            //DOM to persist
//            $parent: '',
//            searchTable: '',

//            //URLs
//            getNewPropsUrl: '/NbtWebApp/wsNBT.asmx/getNodeTypeSearchProps',
//            doViewSearchUrl: '/NbtWebApp/wsNBT.asmx/doViewSearch',
//            doNodeSearchUrl: '/NbtWebApp/wsNBT.asmx/doNodeTypeSearch',
//            getSearchableViewsUrl: '/NbtWebApp/wsNBT.asmx/getSearchableViews',
//            getClientSearchXmlUrl: '/NbtWebApp/wsNBT.asmx/getClientSearchJson',

//            //options
//            searchviewid: '',
//            viewmode: 'tree',
//            parentviewid: '',
//            nodetypeorobjectclassid: '',
//            propertyid: '',
//            cswnbtnodekey: '',
//            relatedidtype: '',
//            ID: '',
//            propsCount: 1,
//            advancedIsHidden: true,

//            //Data to persist
//            propsData: '',
//            selectedPropData: '',
//            nodeTypesData: '',
//            $nodeTypesSelect: '',

//            onSearchSubmit: function () { },
//            onClearSubmit: function () { },
//            onSearchClose: function () { },

//            //For submit
//            selectedPropVal: '',
//            selectedSubfieldVal: '',
//            selectedFilterMode: '',
//            selectedFilterVal: '',

//            bottomRow: 2,
//            bottomCell: 1,
//            propRow: '',
//            clearBtnCell: 1,
//            searchBtnCell: 6,
//            searchtype: ''
//        };

//        if (options) Csw.extend(o, options);

//        var $parent = $(this);
//        var parent = Csw.literals.factory($parent);

//        o.searchTable = parent.table({
//            ID: Csw.makeId(o.ID, 'tbl'),
//            align: 'center'
//        });

//        var topSpan = o.searchTable.span();

//        var topSpanDivId = Csw.makeId({ ID: 'search_criteria_div', prefix: o.ID });
//        var topSpanDiv = topSpan.div({ ID: topSpanDivId })
//                            .addClass('CswSearch_Div');

//        init();

//        function modAdvanced(modOptions) {
//            var a = {
//                'link': '',
//                'advancedIsHidden': false
//            };
//            if (modOptions) Csw.extend(a, modOptions);

//            if ('Advanced' === a.link.text() || (a.advancedIsHidden)) {
//                $('.' + Csw.enums.cssClasses_ViewBuilder.subfield_select.name).each(function () { $(this).show(); });
//                $('.' + Csw.enums.cssClasses_ViewBuilder.filter_select.name).each(function () { $(this).show(); });
//                $('.' + Csw.enums.cssClasses_ViewBuilder.default_filter.name).each(function () { $(this).hide(); });
//                a.link.text('Simple');
//                a.advancedIsHidden = false;
//            }
//            else if ('Simple' === a.link.text() || (!a.advancedIsHidden)) {
//                $('.' + Csw.enums.cssClasses_ViewBuilder.subfield_select.name).each(function () { $(this).hide(); });
//                $('.' + Csw.enums.cssClasses_ViewBuilder.filter_select.name).each(function () { $(this).hide(); });
//                $('.' + Csw.enums.cssClasses_ViewBuilder.default_filter.name).each(function () { $(this).show(); });
//                a.link.text('Advanced');
//                a.advancedIsHidden = true;
//            }
//            return a.advancedIsHidden;
//        } // modAdvanced()

//        function renderViewBasedSearchContent() {
//            //skip cell 1,1
//            var andRow = 3, //2
//                properties = o.propsData,
//                prop, thisProp, nodeTypeId, filtArbitraryId,
//                propRow = 2; //1

//            while (andRow <= o.propsCount) { //eventually this will be configurable: and/or, or, and/not, etc
//                //Row i, Column 1: and
//                o.searchTable.cell(andRow, 1)
//                    .propDom({ align: 'right' })
//                    .css({ 'text-align': 'right' })
//                    .span({ text: '&nbsp;and&nbsp;' });

//                andRow += 1;
//            }

//            for (prop in properties) {
//                if (Csw.contains(properties, prop)) {
//                    thisProp = properties[prop];
//                    nodeTypeId = Csw.makeId({ ID: 'viewbuilderpropid', suffix: thisProp.viewbuilderpropid, prefix: o.ID });
//                    o.searchTable.cell(propRow, 2)
//                        .span({ ID: nodeTypeId, cssclass: Csw.enums.cssClasses_ViewBuilder.metadatatype_static.name, text: thisProp.metadatatypename })
//                        .propNonDom('relatedidtype', thisProp.relatedidtype);
//                    o.selectedSubfieldVal = '';
//                    o.selectedFilterMode = '';
//                    o.selectedFilterVal = '';

//                    filtArbitraryId = thisProp.filtarbitraryid;
//                    //prop filter row
//                    o.searchTable.$.CswViewPropFilter('init', {
//                        ID: o.ID,
//                        propRow: propRow,
//                        firstColumn: 3,
//                        includePropertyName: true,
//                        propsData: thisProp,
//                        filtarbitraryid: filtArbitraryId,
//                        advancedIsHidden: o.advancedIsHidden
//                    });
//                    propRow += 1;
//                }
//            }

//            o.bottomRow = propRow;
//            o.bottomCell = 1;
//            o.searchtype = 'viewsearch';

//            renderSearchButtons();
//        } // renderViewBasedSearchContent()

//        function makeSelect(optionGroup1, optionGroup2) {
//            var $select = $('<select></select>');

//            function makeOptions(optionGroup) {
//                var opt, option, value, optionCol = '';
//                for (opt in optionGroup) {
//                    if (Csw.contains(optionGroup, opt)) {
//                        option = optionGroup[opt];
//                        value = Csw.string(option.value, option.id);
//                        if (Csw.contains(option, 'name') && false === Csw.isNullOrEmpty(value)) {
//                            optionCol += '<option id="' + option.id + '" type="' + option.type + '" value="' + value + '" ';
//                            if ((Csw.bool(option.selected))) {
//                                optionCol += 'selected="selected" ';
//                            }
//                            optionCol += '>' + option.name + '</option>';
//                        }
//                    }
//                }
//                if (false === Csw.isNullOrEmpty(optionCol)) {
//                    $select.append('<optgroup label="' + optionGroup.label + '">' + optionCol + '</optgroup>');
//                }
//            }

//            makeOptions(optionGroup1);
//            makeOptions(optionGroup2);
//            return $select;
//        }

//        function renderNodeTypeSearchContent() {
//            //Row 1, Column 1: empty (Csw.contains 'and' for View search)
//            //Row 1, Column 2: nodetypeselect picklist
//            var nodeTypeSelectId = Csw.makeId(o.ID, 'nodetype_select'),
//                $select;

//            $select = makeSelect(o.nodeTypesData.nodetypeselect, o.nodeTypesData.objectclassselect);

//            o.$nodeTypesSelect = $select
//                                    .CswAttrDom('id', nodeTypeSelectId)
//                                    .CswAttrDom('name', nodeTypeSelectId)
//                                    .addClass(Csw.enums.searchCssClasses.nodetype_select.name)
//                                    .change(function () {
//                                        var $thisSelect = $(this);
//                                        var r = {
//                                            nodetypeorobjectclassid: $thisSelect.val(),
//                                            relatedidtype: $thisSelect.find(':selected').CswAttrNonDom('type'),
//                                            cswnbtnodekey: '',
//                                            optionId: $thisSelect.find(':selected').CswAttrDom('id'),
//                                            $parent: o.searchTable,
//                                            $nodeTypesSelect: $thisSelect
//                                        };
//                                        Csw.extend(o, r);
//                                        getNewProps();
//                                    });
//            o.relatedidtype = o.$nodeTypesSelect.find(':selected').CswAttrNonDom('type');

//            if (false === Csw.isNullOrEmpty(o.nodetypeorobjectclassid)) {
//                o.$nodeTypesSelect.find('option[id="' + o.optionId + '"]').CswAttrDom('selected', true);
//            }
//            o.searchTable.cell(2, 2).append(o.$nodeTypesSelect); //1

//            var propRow = 2, //1
//                genProps = o.propsData.properties['Generic Properties'],
//                specProps = o.propsData.properties['Specific Properties'];
//            //Row propRow, Column 3: properties 
//            var propSelectId = Csw.makeId({ ID: 'property_select', prefix: o.ID });

//            genProps.label = 'Generic Properties';
//            specProps.label = 'Specific Properties';
//            var $propSelect = makeSelect(genProps, specProps)
//                                .CswAttrDom('id', propSelectId)
//                                .addClass(Csw.enums.searchCssClasses.property_select.name)
//                                .change(function () {
//                                    var $this = $(this),
//                                        thisPropId = $this.val(),
//                                        newOh = Csw.object(o.propsData.properties);
//                                    var r = {
//                                        propertyid: thisPropId,
//                                        selectedSubfieldVal: '',
//                                        selectedFilterMode: '',
//                                        selectedFilterVal: '',
//                                        selectedPropData: newOh.find('viewbuilderpropid', thisPropId)
//                                    };
//                                    Csw.extend(o, r);
//                                    o.searchTable.$.CswViewPropFilter('init', {
//                                        ID: o.ID,
//                                        propRow: propRow,
//                                        firstColumn: 3,
//                                        includePropertyName: false,
//                                        propsData: o.selectedPropData,
//                                        viewbuilderpropid: thisPropId,
//                                        advancedIsHidden: o.advancedIsHidden
//                                    });
//                                });

//            if (false === Csw.isNullOrEmpty(o.propertyid)) {
//                $propSelect.val(o.propertyid).CswAttrDom('selected', true);
//            }
//            o.searchTable.cell(propRow, 3).append($propSelect);

//            o.propertyid = $propSelect.find(':selected').val();
//            var oH = Csw.object(o.propsData.properties);
//            o.selectedPropData = oH.find('viewbuilderpropid', o.propertyid);

//            o.searchTable.$.CswViewPropFilter('init', {
//                ID: o.ID,
//                propRow: propRow,
//                firstColumn: 3,
//                includePropertyName: false,
//                propsData: o.selectedPropData,
//                viewbuilderpropid: o.propertyid,
//                advancedIsHidden: o.advancedIsHidden
//            });

//            o.bottomRow = (o.propsCount + 2); //1
//            o.bottomCell = 2;
//            o.searchtype = 'nodetypesearch';
//            renderSearchButtons();
//        } // renderNodeTypeSearchContent()

//        function getNewProps() {
//            var jsonData = {
//                RelatedIdType: o.relatedidtype,
//                NodeTypeOrObjectClassId: o.nodetypeorobjectclassid,
//                IdPrefix: o.ID,
//                NodeKey: o.cswnbtnodekey
//            };

//            Csw.ajax.post({
//                url: o.getNewPropsUrl,
//                data: jsonData,
//                success: function (data) {
//                    o.propsData = data;
//                    renderNodeTypeSearchContent();
//                }
//            });
//        } // getNewProps()

//        function renderSearchButtons() {
//            var clearPositionTable;
//            var clearCellNumber = o.bottomCell;
//            var advancedCellNumber = clearCellNumber + 1;
//            var cellRow = o.bottomRow;
//            if (o.searchtype === 'nodetypesearch') {
//                //Row i, Column 1: cell for clear/advanced                                            
//                var splitCellTable = o.searchTable.cell(o.bottomRow, o.bottomCell).table({
//                    ID: Csw.makeId(o.ID, 'split_cell_table'),
//                    cellpadding: 1,
//                    cellspacing: 1,
//                    cellalign: 'left',
//                    align: 'left'
//                });
//                clearPositionTable = splitCellTable;
//                clearCellNumber = 1;
//                advancedCellNumber = 2;
//                cellRow = 2;
//            } else {
//                clearPositionTable = o.searchTable;
//            }

//            //Row i, Column 1 (1/1): clear button
//            var clearButtonCell = clearPositionTable.cell(cellRow, clearCellNumber);
//            //clear btn
//            clearButtonCell.button({
//                ID: Csw.makeId(o.ID, 'clear_button'),
//                enabledText: 'Reset', //case 22756: this is more accurate name-to-behavior.
//                disabledText: 'Reset',
//                disableOnClick: false,
//                onClick: function () {
//                    o.onClearSubmit(o.parentviewid, o.viewmode);
//                }
//            });

//            //Row i, Column 1 (1/2): advanced link
//            var advancedLinkCell = clearPositionTable.cell(cellRow, advancedCellNumber);
//            var advancedLink = advancedLinkCell.a({
//                ID: Csw.makeId(o.ID, 'advanced_options'),
//                href: 'javascript:void(0)',
//                value: (o.advancedIsHidden) ? 'Advanced' : 'Simple'
//            })
//                                                    .click(function () {
//                                                        o.advancedIsHidden = modAdvanced({ 'link': advancedLink, advancedIsHidden: o.advancedIsHidden });
//                                                        return false;
//                                                    });

//            //Row i, Column 5: search button
//            var searchButtonCell = o.searchTable.cell(o.bottomRow, o.searchBtnCell)
//                                    .propDom({ align: 'right' })
//                                    .css({ 'text-align': 'right' });
//            var searchButton = searchButtonCell.button({
//                ID: Csw.makeId(o.ID, 'search_button'),
//                enabledText: 'Search',
//                disabledText: 'Searching',
//                onClick: function () { doSearch(); }
//            });
//            searchButton.$.CswViewPropFilter('bindToButton');
//        } // renderSearchButtons()

//        function init() {
//            var thisViewId = (false === Csw.isNullOrEmpty(o.searchviewid)) ? o.searchviewid : o.parentviewid;
//            var jsonData = {
//                ViewId: thisViewId,
//                SelectedNodeTypeIdNum: o.nodetypeorobjectclassid,
//                IdPrefix: o.ID,
//                NodeKey: o.cswnbtnodekey
//            };

//            Csw.ajax.post({
//                'url': o.getClientSearchXmlUrl,
//                'data': jsonData,
//                'success': function (data) {
//                    topSpanDiv.empty();
//                    o.searchtype = data.searchtype;
//                    var searchTableId = Csw.makeId({ prefix: o.ID, ID: 'search_tbl' });
//                    o.searchTable = topSpanDiv.table({
//                        ID: searchTableId,
//                        cellpadding: 1,
//                        cellspacing: 1,
//                        cellalign: 'left',
//                        align: 'center',
//                        TableCssClass: 'CswSearch_Table'
//                    });

//                    //close btn
//                    o.searchTable.cell(1, 10)
//                                   .propDom({ align: 'right' })
//                                   .css({ 'text-align': 'right' })
//                                   .imageButton({
//                                       ButtonType: Csw.enums.imageButton_ButtonType.Delete,
//                                       AlternateText: 'Close',
//                                       ID: Csw.makeId({ 'prefix': o.ID, 'id': 'closebtn' }),
//                                       onClick: function () {
//                                           o.onSearchClose();
//                                       }
//                                   });

//                    switch (o.searchtype) {
//                        case 'nodetypesearch':
//                            o.nodeTypesData = data.nodetypes;
//                            o.propsData = data.props;
//                            renderNodeTypeSearchContent();
//                            break;
//                        case 'viewsearch':
//                            o.propsData = data.properties;
//                            //o.propsCount = data.properties.property.size();
//                            renderViewBasedSearchContent();
//                            break;
//                    }
//                } // success
//            }); // Csw.ajax
//        } // init()

//        function doSearch() {
//            var searchOpt,
//                props = [],
//                searchUrl = '',
//                dataJson;

//            function collectPropFilters() {
//                $('.' + Csw.enums.cssClasses_ViewBuilder.filter_value.name).each(function () {
//                    var $thisProp = $(this),
//                        propsData = $thisProp.data('propsData'),
//                        thisNodeProp = $thisProp.parent()
//                                                .parent() //we need the <tr> which Csw.contains the context for the whole prop
//                                                .CswViewPropFilter('getFilterJson', {
//                                                    nodetypeorobjectclassid: o.nodetypeorobjectclassid,
//                                                    relatedidtype: propsData.relatedidtype,
//                                                    proptype: propsData.proptype,
//                                                    filtJson: { fieldtype: propsData.fieldtype },
//                                                    ID: o.ID,
//                                                    $parent: o.searchTable.$,
//                                                    filtarbitraryid: propsData.filtarbitraryid,
//                                                    proparbitraryid: propsData.proparbitraryid,
//                                                    viewbuilderpropid: propsData.viewbuilderpropid,
//                                                    advancedIsHidden: o.advancedIsHidden,
//                                                    allowNullFilterValue: true //Case 24413
//                                                });
//                    props.push(thisNodeProp);
//                });
//            }

//            switch (o.searchtype) {
//                case 'nodetypesearch':
//                    searchUrl = o.doNodeSearchUrl;
//                    o.nodetypeorobjectclassid = o.$nodeTypesSelect.val();
//                    o.relatedidtype = o.$nodeTypesSelect.find(':selected').CswAttrNonDom('type');

//                    collectPropFilters();

//                    searchOpt = {
//                        viewbuilderprops: props,
//                        parentviewid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId)
//                    };
//                    break;
//                case 'viewsearch':
//                    searchUrl = o.doViewSearchUrl;
//                    collectPropFilters();
//                    searchOpt = {
//                        viewprops: props,
//                        searchviewid: o.searchviewid,
//                        parentviewid: o.parentviewid
//                    };
//                    break;
//            }

//            if (false === Csw.isNullOrEmpty(searchOpt)) {
//                dataJson = {
//                    SearchJson: searchOpt
//                };
//                Csw.ajax.post({
//                    url: searchUrl,
//                    data: dataJson,
//                    success: function (view) {
//                        o.searchviewid = view.searchviewid;
//                        o.parentviewid = view.parentviewid;
//                        o.viewmode = view.viewmode;
//                        o.searchtype = 'viewsearch'; //the next search will be always be based on the view returned
//                        init(); //our arbitraryid's have probably changed. need to pull fresh XML.
//                        o.onSearchSubmit(view.searchviewid, view.viewmode, view.parentviewid);
//                    }
//                });
//            }
//        } // doSearch()

//        return o.searchTable.$;
//    };
//})(jQuery);
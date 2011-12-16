/// <reference path="/js/../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../controls/CswSelect.js" />
/// <reference path="../view/CswViewPropFilter.js" />

(function ($) { /// <param name="$" type="jQuery" />
    $.fn.CswSearch = function (options) {

        var o = { 
            //DOM to persist
            $parent: '',
            $searchTable: '',
                
            //URLs
            getNewPropsUrl: '/NbtWebApp/wsNBT.asmx/getNodeTypeSearchProps',
            doViewSearchUrl: '/NbtWebApp/wsNBT.asmx/doViewSearch',
            doNodeSearchUrl: '/NbtWebApp/wsNBT.asmx/doNodeTypeSearch',
            getSearchableViewsUrl: '/NbtWebApp/wsNBT.asmx/getSearchableViews',
            getClientSearchXmlUrl: '/NbtWebApp/wsNBT.asmx/getClientSearchJson',

            //options
            searchviewid: '',
            viewmode: 'tree',
            parentviewid: '',
            nodetypeorobjectclassid: '',
            propertyid: '',
            cswnbtnodekey: '',
            relatedidtype: '',
            ID: '',
            propsCount: 1,
            advancedIsHidden: true,
                
            //Data to persist
            propsData: '',
            selectedPropData: '',
            nodeTypesData: '',
            $nodeTypesSelect: '',

            onSearchSubmit: function (view) {},
            onClearSubmit: function(parentviewid) {},
            onSearchClose: function() {}, 

            //For submit
            selectedPropVal: '',
            selectedSubfieldVal: '',
            selectedFilterVal: '',

            bottomRow: 2,
            bottomCell: 1,
            propRow: '',
            clearBtnCell: 1,
            searchBtnCell: 6,
            searchtype: ''
        };
        
        if(options) $.extend(o, options);
        
        var $parent = $(this);
        o.$searchTable = $parent.CswDiv('init',{ID: o.ID});
        
        var $topspan = o.$searchTable.CswSpan('init');

        var topspandivid = makeId({ID: 'search_criteria_div', prefix: o.ID});
        var $topspandiv = $topspan.CswDiv('init',{ID: topspandivid})
                            .addClass('CswSearch_Div');
        init();

        function modAdvanced(options) {
            var a = {
                '$link': '',
                'advancedIsHidden': false
            };
            if(options) $.extend(a,options);
    
            if('Advanced' === a.$link.text() || ( a.advancedIsHidden ) )
            {   
                $('.' + ViewBuilder_CssClasses.subfield_select.name).each(function() { $(this).show(); });
                $('.' + ViewBuilder_CssClasses.filter_select.name).each(function() { $(this).show(); });
                $('.' + ViewBuilder_CssClasses.default_filter.name).each(function() { $(this).hide(); });
                a.$link.text('Simple');
                a.advancedIsHidden = false;
            }
            else if('Simple' === a.$link.text() || ( !a.advancedIsHidden ) )
            {
                $('.' + ViewBuilder_CssClasses.subfield_select.name).each(function() { $(this).hide(); });
                $('.' + ViewBuilder_CssClasses.filter_select.name).each(function() { $(this).hide(); });
                $('.' + ViewBuilder_CssClasses.default_filter.name).each(function() { $(this).show(); });
                a.$link.text('Advanced');
                a.advancedIsHidden = true;
            }
            return a.advancedIsHidden; 
        } // modAdvanced()

        function renderViewBasedSearchContent() {
            //skip cell 1,1
            var andRow = 3, //2
                properties = o.propsData,
                $andCell, $andText, prop, thisProp, $nodeTypeCell, nodeTypeId, filtArbitraryId,
                propRow = 2; //1
            
            while(andRow <= o.propsCount) //eventually this will be configurable: and/or, or, and/not, etc
            {
                //Row i, Column 1: and
                $andCell = o.$searchTable.CswTable('cell', andRow, 1)
                               .CswAttrDom({align:"right"});
                $andText = $('<span>&nbsp;and&nbsp;</span>');
                $andCell.append($andText);
                andRow++;
            }
        
            for (prop in properties) {
                if (contains(properties, prop)) {
                    thisProp = properties[prop];
                    $nodeTypeCell = o.$searchTable.CswTable('cell', propRow, 2);
                    nodeTypeId = makeId({ ID: 'viewbuilderpropid', suffix: thisProp.viewbuilderpropid, prefix: o.ID });
                    //node type
                    $nodeTypeCell.CswSpan('init', { ID: nodeTypeId,
                        value: thisProp.metadatatypename,
                        cssclass: ViewBuilder_CssClasses.metadatatype_static.name})
                        .CswAttrDom('relatedidtype', thisProp.relatedidtype);
                    o.selectedSubfieldVal = '';
                    o.selectedFilterVal = '';

                    filtArbitraryId = thisProp.filtarbitraryid;
                    //prop filter row
                    o.$searchTable.CswViewPropFilter('init', {
                        ID: o.ID,
                        propRow: propRow,
                        firstColumn: 3,
                        includePropertyName: true,
                        propsData: thisProp,
                        filtarbitraryid: filtArbitraryId,
                        advancedIsHidden: o.advancedIsHidden
                    });
                    propRow++;
                }
            }

            o.bottomRow = propRow;
            o.bottomCell = 1;
            o.searchtype = 'viewsearch';
            
            renderSearchButtons();
        } // renderViewBasedSearchContent()

        function makeSelect(optionGroup1, optionGroup2) {
            var $select = $('<select></select>');
            
            function makeOptions(optionGroup) {
                var opt, option, value, optionCol = '', selected = '';
                for (opt in optionGroup) {
                    if (contains(optionGroup, opt)) {
                        option = optionGroup[opt];
                        value = tryParseString(option.value, option.id);
                        if(contains(option,'name') && false === isNullOrEmpty(value)) {
                            selected = (isTrue(option.selected)) ? 'selected="true"' : '';
                            optionCol += '<option ' + selected + ' id="' + option.id + '" type="' + option.type + '" value="' + value + '">' + option.name + '</option>';
                        }
                    }
                }
                if (false === isNullOrEmpty(optionCol)) {
                    $select.append('<optgroup label="' + optionGroup.label + '">' + optionCol + '</optgroup>');
                }    
            }

            makeOptions(optionGroup1);
            makeOptions(optionGroup2);
            return $select;
        }
        
        function renderNodeTypeSearchContent() {
            //Row 1, Column 1: empty (contains 'and' for View search)
            //Row 1, Column 2: nodetypeselect picklist
            var nodeTypeSelectId = makeId({ID: 'nodetype_select',prefix: o.ID}),
                $typeSelectCell, $select, ocpSelect;
            
            $typeSelectCell = o.$searchTable.CswTable('cell', 2, 2) //1
                                                .empty();
            $select = makeSelect(o.nodeTypesData.nodetypeselect, o.nodeTypesData.objectclassselect);
            
            o.$nodeTypesSelect = $select
                                    .CswAttrDom('id', nodeTypeSelectId)
                                    .CswAttrDom('name', nodeTypeSelectId)
                                    .addClass(CswSearch_CssClasses.nodetype_select.name)
                                    .change( function() {
                                           var $thisSelect = $(this);
                                           var r = {
                                                nodetypeorobjectclassid: $thisSelect.val(),
                                                relatedidtype: $thisSelect.find(':selected').CswAttrDom('type'),
                                                cswnbtnodekey: '',
                                                optionId: $thisSelect.find(':selected').CswAttrDom('id'),
                                                $parent: o.$searchTable,
                                                $nodeTypesSelect: $thisSelect 
                                           };
                                           $.extend(o,r);
                                           getNewProps();  
                                    });
            o.relatedidtype = o.$nodeTypesSelect.find(':selected').CswAttrDom('type');
            
            if (false === isNullOrEmpty(o.nodetypeorobjectclassid)) {
                o.$nodeTypesSelect.find('option[id="' + o.optionId + '"]').CswAttrDom('selected',true);
            }
            $typeSelectCell.append(o.$nodeTypesSelect);
        
            var propRow = 2, //1
                genProps = o.propsData.properties['Generic Properties'],
                specProps = o.propsData.properties['Specific Properties']; 
            //Row propRow, Column 3: properties 
            var $propSelectCell = o.$searchTable.CswTable('cell', propRow, 3)
                                    .empty();
            var propSelectId = makeId({ID: 'property_select', prefix: o.ID});

//            ocpSelect = o.propsData.select['Generic Properties'];
//            
//            var selectOpt = o.propsData.properties.select;
//            var selectAry = [];
//            var selected = '';
//            for (var id in selectOpt) {
//                if (selectOpt.hasOwnProperty(id)) {
//                    selectAry.push({ value: id, display: selectOpt[id] });
//                }
//            }
            genProps.label = 'Generic Properties';
            specProps.label = 'Specific Properties';
            var $propSelect = makeSelect(genProps, specProps)
                                .CswAttrDom('id', propSelectId)
                                .addClass(CswSearch_CssClasses.property_select.name)
                                .change(function () {
                                    var $this = $(this),
                                        thisPropId = $this.val(),
                                        oH = new ObjectHelper(o.propsData.properties);
                                    var r = {
                                        propertyid: thisPropId,
                                        selectedSubfieldVal: '',
                                        selectedFilterVal: '',
                                        selectedPropData: oH.find('viewbuilderpropid', thisPropId)
                                    };
                                    $.extend(o,r);
                                    o.$searchTable.CswViewPropFilter('init', {
                                                ID: o.ID,
                                                propRow: propRow,
                                                firstColumn: 3,
                                                includePropertyName: false,
                                                propsData: o.selectedPropData,
                                                viewbuilderpropid: thisPropId,
                                                advancedIsHidden: o.advancedIsHidden
                                            }); 
                                });
                                
            if (false === isNullOrEmpty(o.propertyid)) {
                $propSelect.val(o.propertyid).CswAttrDom('selected',true);
            }
            $propSelectCell.append($propSelect);
            
            o.propertyid = $propSelect.find(':selected').val();
            var oH = new ObjectHelper(o.propsData.properties);
            o.selectedPropData = oH.find('viewbuilderpropid', o.propertyid);
        
            o.$searchTable.CswViewPropFilter('init', {
                                            ID: o.ID,
                                            propRow: propRow,
                                            firstColumn: 3,
                                            includePropertyName: false,
                                            propsData: o.selectedPropData,
                                            viewbuilderpropid: o.propertyid,
                                            advancedIsHidden: o.advancedIsHidden
                                        });
            
            o.bottomRow = (o.propsCount + 2); //1
            o.bottomCell = 2;
            o.searchtype = 'nodetypesearch';
            renderSearchButtons();
        } // renderNodeTypeSearchContent()

        function getNewProps()
        {
            var jsonData = {
                RelatedIdType: o.relatedidtype,
                NodeTypeOrObjectClassId: o.nodetypeorobjectclassid,
                IdPrefix: o.ID,
                NodeKey: o.cswnbtnodekey
            };

            CswAjaxJson({ 
                        url: o.getNewPropsUrl,
                        data: jsonData,
                        success: function(data) { 
                            o.propsData = data;
                            renderNodeTypeSearchContent();
                        }
                    });
        } // getNewProps()

        function renderSearchButtons() {
            var $clearPosition = o.$searchTable;
            var clearCellNumber = o.bottomCell;
            var advancedCellNumber = clearCellNumber + 1;
            var cellRow = o.bottomRow;
            if(o.searchtype === 'nodetypesearch')
            {
                //Row i, Column 1: cell for clear/advanced                                            
                var $splitCell = o.$searchTable.CswTable('cell', o.bottomRow, o.bottomCell)
                                    .empty();
                var splitCellTableId = makeId({prefix: o.ID, ID: 'split_cell_table'});
                var $splitCellTable = $splitCell.CswTable('init',{ID: splitCellTableId, 
                                                                    cellpadding: 1,
                                                                    cellspacing: 1,
                                                                    cellalign: 'left',
                                                                    align: 'left'
                                                                    });
                $clearPosition = $splitCellTable;
                clearCellNumber = 1;
                advancedCellNumber = 2;
                cellRow = 2;
            }
            
            //Row i, Column 1 (1/1): clear button
            var $clearButtonCell = $clearPosition.CswTable('cell', cellRow, clearCellNumber)
                                    .empty();
            var clearButtonId = makeId({ID: 'clear_button', prefix: o.ID});
            //clear btn
            $clearButtonCell.CswButton({ID: clearButtonId,
                                        enabledText: 'Reset', //case 22756: this is more accurate name-to-behavior.
                                        disabledText: 'Reset',
                                        disableOnClick: false, 
                                        onclick: function() 
                                        {
                                            o.onClearSubmit(o.parentviewid,o.viewmode);
                                        }
                                    });
                                            
            //Row i, Column 1 (1/2): advanced link
            var $advancedLinkCell = $clearPosition.CswTable('cell', cellRow, advancedCellNumber)
                                    .empty();
            var advancedLinkId = makeId({ID: 'advanced_options', prefix: o.ID});
            var $advancedLink = $advancedLinkCell.CswLink('init',{
                                                        ID: advancedLinkId,
                                                        href: 'javascript:void(0)',
                                                        value: (o.advancedIsHidden) ? 'Advanced' : 'Simple' })
                                                    .click(function() {
                                                            o.advancedIsHidden = modAdvanced({'$link': $advancedLink, advancedIsHidden: o.advancedIsHidden });
                                                            return false;
                                                    });  
            
            //Row i, Column 5: search button
            var $searchButtonCell = o.$searchTable.CswTable('cell', o.bottomRow, o.searchBtnCell)
                                    .CswAttrDom({align:"right"})
                                    .empty();
            var searchButtonId = makeId({ID: 'search_button', prefix: o.ID});
            var $searchButton = $searchButtonCell.CswButton({ID: searchButtonId, 
                                                            enabledText: 'Search', 
                                                            disabledText: 'Searching', 
                                                            onclick: function() { doSearch(); }
                                                  });
           $searchButton.CswViewPropFilter('bindToButton');
        } // renderSearchButtons()

        function init() {
            var thisViewId = (false === isNullOrEmpty(o.searchviewid)) ? o.searchviewid : o.parentviewid; 
            var jsonData = {
                ViewId: thisViewId, 
                SelectedNodeTypeIdNum: o.nodetypeorobjectclassid, 
                IdPrefix: o.ID,
                NodeKey: o.cswnbtnodekey
            };

            CswAjaxJson({ 
                'url': o.getClientSearchXmlUrl,
                'data':jsonData,
                'success': function(data) { 
                    $topspandiv.empty();
                    o.searchtype = data.searchtype;
                    var searchTableId = makeId({prefix: o.ID, ID: 'search_tbl'});
                    o.$searchTable = $topspandiv.CswTable('init', { 
                                        ID: searchTableId, 
                                        cellpadding: 1,
                                        cellspacing: 1,
                                        cellalign: 'left',
                                        align: 'center',
                                        TableCssClass: 'CswSearch_Table'
                                     });
                    
                    //close btn
                    o.$searchTable.CswTable('cell',1,10)
                                   .css('align','right')
                                   .CswImageButton({
                                        ButtonType: CswImageButton_ButtonType.Delete,
                                        AlternateText: 'Close',
                                        ID: makeId({ 'prefix': o.ID, 'id': 'closebtn' }),
                                        onClick: function () { 
                                            o.onSearchClose();
                                            return CswImageButton_ButtonType.None; 
                                        }
                                    });
                    
                    switch(o.searchtype) {
                        case 'nodetypesearch':
                            o.nodeTypesData = data.nodetypes;
                            o.propsData = data.props;
                            renderNodeTypeSearchContent();
                            break;
                        case 'viewsearch':
                            o.propsData = data.properties;
                            //o.propsCount = data.properties.property.size();
                            renderViewBasedSearchContent();
                            break;
                    }
                } // success
            }); // CswAjaxJson
        } // init()

        function doSearch() {
            var searchOpt,
                props = [],
                searchUrl = '',
                dataJson;

            function collectPropFilters(cssclass) {
                $('.' + ViewBuilder_CssClasses.filter_value.name).each(function() {
                    var $thisProp = $(this),
                        propsData = $thisProp.data('propsData'),
                        thisNodeProp = $thisProp.parent()
                                                .parent() //we need the <tr> which contains the context for the whole prop
                                                .CswViewPropFilter('getFilterJson',{
                                                    nodetypeorobjectclassid: o.nodetypeorobjectclassid,
                                                    relatedidtype: propsData.relatedidtype,  
                                                    filtJson: { fieldtype: propsData.fieldtype },
                                                    ID: o.ID,   
                                                    $parent: o.$searchTable,
                                                    filtarbitraryid: propsData.filtarbitraryid,
                                                    proparbitraryid: propsData.proparbitraryid,
                                                    viewbuilderpropid: propsData.viewbuilderpropid,
                                                    advancedIsHidden: o.advancedIsHidden,
                                                    allowNullFilterValue: true //Case 24413
                                                }); 
                    props.push( thisNodeProp );
                });
            }
            
            switch (o.searchtype) {
                case 'nodetypesearch':
                    searchUrl = o.doNodeSearchUrl;
                    o.nodetypeorobjectclassid = o.$nodeTypesSelect.val();
                    o.relatedidtype = o.$nodeTypesSelect.find(':selected').CswAttrDom('type');
                    
                    collectPropFilters();
                    
                    searchOpt = {
                        viewbuilderprops : props,
                        parentviewid: $.CswCookie('get', CswCookieName.CurrentViewId)
                    };
                    break;
                case 'viewsearch':
                    searchUrl = o.doViewSearchUrl;
                    collectPropFilters();
                    searchOpt = { 
                            viewprops: props,
                            searchviewid: o.searchviewid,
                            parentviewid: o.parentviewid
                    };
                    break;
            }

            if (false === isNullOrEmpty(searchOpt)) {
                dataJson = {
                    SearchJson: searchOpt
                };
                CswAjaxJson({ 
                    url: searchUrl,
                    data: dataJson,
                    success: function(view) { 
                        o.searchviewid = view.searchviewid;
                        o.parentviewid = view.parentviewid;
                        o.viewmode = view.viewmode;
                        o.searchtype = 'viewsearch'; //the next search will be always be based on the view returned
                        init(); //our arbitraryid's have probably changed. need to pull fresh XML.
                        o.onSearchSubmit(view.searchviewid, view.viewmode, view.parentviewid);
                    }
                });
            }
        } // doSearch()

    return o.$searchTable;
    };
})(jQuery);
; (function ($) {
	$.fn.CswViewPropFilter = function (options) {

        var o = { 
            //URLs
            'getNewPropsUrl': '/NbtWebApp/wsNBT.asmx/getViewPropFilter',

            //options
			'viewid': '',
            'viewxml': '',
            'proparbitraryid': '',
            'idprefix': 'csw',

            'selectedSubfieldVal': '',
            'selectedFilterVal': ''
		};
		
        if(options) $.extend(o, options);
        
        var $parent = $(this);
        var $cswPropFilterRow = $parent.CswDOM('span',{ID: 'cswPropFilterRow', prefix: o.idprefix});
        var $propsXml;
        var $propFilterTable;

        init();

        function init()
        {
            CswAjaxXml({ 
		        'url': o.getNewPropsUrl,
		        'data': "ViewXml=" + o.viewxml + "&PropArbitraryId=" + o.proparbitraryid,
                'success': function($xml) { 

                            $propFilterTable = $cswPropFilterRow.CswTable('init', { 
                                        ID: makeId({prefix: o.idprefix, ID: 'search_tbl'}), 
                                        cellpadding: 1,
                                        cellspacing: 1,
                                        cellalign: 'center',
                                        align: 'center'
                                    });
                            
                            $propsXml = $xml.children('nodetypeprops').children('property');
                            renderPropFiltRow();
                } //success
            }); //ajax
        }

        function renderPropFiltRow()
        {
            var propertyId = $propsXml.attr('propid');
            var propertyName = $propsXml.attr('propname');
            var filtArbitraryId = $propsXml.attr('filtarbitraryid');
                
            //Row propRow, Column 3: property
            var $propSelectCell = $propFilterTable.CswTable('cell', propRow, 3)
                                                    .empty();
            var propCellId = makeId({ID: propertyId,prefix: o.idprefix});
            var $props = $propSelectCell.CswDOM('span',{ID: propCellId, value: propertyName});
        
            var $defaultFilter = $propsXml.children('defaultsubfield').attr('filter');
            var fieldtype = $propsXml.attr('fieldtype');

            //Row propRow, Column 4: subfield picklist 
            var $subfieldsOptions = $(xmlToString($propsXml.children('subfields').children('select')))
                                    .change(function() {
                                        var $this = $(this);
                                        var r = {
                                            'selectedSubfieldVal': $this.val(),
                                            'selectedFilterVal': ''
                                        };
                                        $.extend(o,r);
                                        renderPropFiltRow() });

            if(o.selectedSubfieldVal !== '')
            {
                $subfieldsOptions.val(o.selectedSubfieldVal).attr('selected',true);
            }
            $subfieldCell.append($subfieldsOptions);
            var $subfield = $subfieldsOptions.find(':selected').val();
            var defaultValue = $subfieldsOptions.find(':selected').attr('defaultvalue');

            //Row propRow, Column 5: filter picklist
            var $filtersCell = $propFilterTable.CswTable('cell', propRow, 5)
                                .empty();

            var $filtersOptions =  $(xmlToString($propsXml.children('propertyfilters').children('subfield[column=' + $subfield + ']').children('select')))
                                    .change(function() {
                                        var $this = $(this);
                                        var r = {
                                            'selectedSubfieldVal': $subfieldsOptions.val(),
                                            'selectedFilterVal': $this.val()
                                        };
                                        $.extend(o,r);
                                        renderPropFiltRow() });

            if(o.selectedFilterVal !== '')
            {
                $filtersOptions.val(o.selectedFilterVal).attr('selected',true);
            }
            $filtersCell.append($filtersOptions);
            var $filter = $filtersOptions.find(':selected').val();

            //Row propRow, Column 6: search input
            var $propFilterValueCell = $propFilterTable.CswTable('cell', propRow, 6)
                            .empty();
            
            if( fieldtype === 'List' )
            {
                $propFilterValueCell.append( $(xmlToString($propsXml.children('filtersoptions').children('select'))) );
            }
            else if( fieldtype === 'Logical' )
            {
                $propFilterValueCell.CswTristateCheckBox('init',{'ID': 'search_input_searchpropid_' + propertyId, 'prefix': o.idprefix}); 
            }
            else
            {
                var filtValueSuggest;
                if( defaultValue !== '' && defaultValue != undefined )
                {
                    filtValueSuggest = defaultValue;
                }
                else
                {
                    filtValueSuggest = $props.find(':selected').text();
                    if(filtValueSuggest !== $subfieldsOptions.find(':selected').text() )
                    {
                        filtValueSuggest += "'s " +  $subfieldsOptions.find(':selected').text();
                    }  
                }
                var filtValInputId = makeId({ID: 'search_input_searchpropid', prefix: o.idprefix, suffix:propertyId});
                var $filtValInput = $propFilterValueCell.CswDOM('input',{
                                                        ID: filtValInputId,
                                                        type: 'text',
                                                        cssclass: 'csw_search_input',
                                                        placeholder: filtValueSuggest })
                                                .attr('autocomplete','on')
                                                .attr('autofocus','true')
                                                .attr({width:"200px"});
            }
                            
			   
        } // renderPropFiltRow()

        return $cswPropFilterRow;

	 // function(options) {
    };
})(jQuery);



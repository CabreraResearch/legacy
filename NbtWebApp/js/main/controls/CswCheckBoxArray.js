/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	$.fn.CswCheckBoxArray = function (method) {
	
		var pluginName = 'CswCheckBoxArray';
	    var storedData = 'CbaData';
	    var storedDataClass = 'CbaStoredData';
	    
		var methods = {
		    init: function(options) {
		
				var o = {
					ID: '',
					cols: ['col1', 'col2', 'col3'],
					data: [{ label: 'row1', 
							 key: 1,
							 values: [ true, false, true ] },
						   { label: 'row2', 
							 key: 2,
							 values: [ false, true, false ] },
						   { label: 'row3', 
							 key: 3,
							 values: [ true, false, true ] }],
					HeightInRows: 4,
					//CheckboxesOnLeft: false,
					UseRadios: false,
					Required: false,
					ReadOnly: false,
					onchange: null, //function() { }
				    dataAry: [],
			        nameCol: '',
			        keyCol: '',
			        valCol: ''
				};
				
				if (options) {
					$.extend(o, options);
				}

		        var $Div = transmorgify({ dataAry: o.dataAry,
			                              nameCol: o.nameCol,
			                              keyCol: o.keyCol,
			                              valCol: o.valCol 
		                                 }, 
		                                 $(this));
		        
		        var dataStore = $Div.data(storedData);
		        if (false === isNullOrEmpty(storedData)) {
		            $.extend(o, dataStore);
		        }
		        
				var checkType = CswInput_Types.checkbox.name;
				if(o.UseRadios)
					checkType = CswInput_Types.radio.name;
				
				$Div.contents().remove();
		        var storeDataId = o.ID + '_cswCbaArrayDataStore';
				var $OuterDiv = $('<div id="' + storeDataId + '" class="' + storedDataClass + '"/>')
    				                .appendTo($Div)
				                    .data(storedData, {columns: o.cols, data: o.data});;
		        
				if (o.ReadOnly) {
					for (var r = 0; r < o.data.length; r++) {
						var rRow = o.data[r];
						var rowlabeled = false;
						var first = true;
						for (var c = 0; c < o.cols.length; c++) {
							if (isTrue(rRow.values[c])) {
								if (!rowlabeled) {
									$OuterDiv.append(rRow.label + ": ");
									rowlabeled = true;
								}
								if (!first) {
									$OuterDiv.append(", ");
								}
								$OuterDiv.append(o.cols[c]);
								first = false;
							}
						}
						if (rowlabeled) {
							$OuterDiv.append('<br/>');
						}
					}
				} else {
					var $table = $OuterDiv.CswTable('init', { ID: o.ID + '_tbl' });

					$OuterDiv.css('height', (25 * o.HeightInRows) + 'px');
					$OuterDiv.addClass('cbarraydiv');
					$table.addClass('cbarraytable');

					// Header
					var tablerow = 1;
					for(var d = 0; d < o.cols.length; d++)
					{
						var $dCell = $table.CswTable('cell', tablerow, d+2);
						$dCell.addClass('cbarraycell');
						$dCell.append(o.cols[d]);
					}
					tablerow++;

					//[none] row
					if(o.UseRadios && ! o.Required)
					{
						// Row label
						var $labelcell = $table.CswTable('cell', tablerow, 1);
						$labelcell.addClass('cbarraycell');
						$labelcell.append('[none]');
						for(var e = 0; e < o.cols.length; e++)
						{
							var $eCell = $table.CswTable('cell', tablerow, e+2);
							$eCell.addClass('cbarraycell');
							var eCheckid = o.ID + '_none';
							var $eCheck = $('<input type="'+ checkType +'" class="CBACheckBox_'+ o.ID +'" id="'+ eCheckid + '" name="' + o.ID + '" />')
										   .appendTo($eCell)
										   .click(o.onchange);
							$eCell.CswAttrXml({'key': '', rowlabel: '[none]', collabel: o.cols[e], row: -1, col: e });
						
							$eCheck.CswAttrDom('checked', 'true');   // the browser will override this if another one is checked

						} // for(var c = 0; c < o.cols.length; c++)
					} // if(o.UseRadios && ! o.Required)
					tablerow++;

				    var onClick = function() {
					    var $this = $(this);
					    var col = $this.CswAttrXml('col');
					    var row = $this.CswAttrXml('row');
					    var cbaDiv = $('#' + storeDataId);
					    var cache = cbaDiv.data(storedData);
				        if (cache.data.hasOwnProperty(row)) {
					        var thisRow = cache.data[row];
					        if (thisRow.hasOwnProperty('values')) {
					            var values = thisRow.values;
					            values[col] = $this.is(':checked');
					        }
					    }
					    cbaDiv.data(storedData, cache);
					};
				    
					// Data
					for(var s = 0; s < o.data.length; s++)
					{
						var sRow = o.data[s];
						// Row label
						var $sLabelcell = $table.CswTable('cell', tablerow + s, 1);
						$sLabelcell.addClass('cbarraycell');
						$sLabelcell.append(sRow.label);
					    
					    for(var f = 0; f < o.cols.length; f++)
						{
						
							var $fCell = $table.CswTable('cell', tablerow + s, f+2);
							$fCell.addClass('cbarraycell');
							var fCheckid = o.ID + '_' + s + '_' + f;
                            
							var $fCheck = $('<input type="'+ checkType +'" class="CBACheckBox_'+ o.ID +'" id="'+ fCheckid + '" name="' + o.ID + '" />')
										   .appendTo($fCell)
										   .bind('click', o.onchange)
							               .CswAttrXml({key: sRow.key, rowlabel: sRow.label, collabel: o.cols[f], row: s, col: f })
							               .data('thisRow', sRow)
						                   .bind('click', onClick);

							if(sRow.values[f]) {
								$fCheck.CswAttrDom('checked', 'true');
							}
						} // for(var c = 0; c < o.cols.length; c++)
					} // for(var r = 0; r < o.data.length; r++)

					if(!o.UseRadios)
					{
						var CheckAllLinkText = "Check All";
						if($('.CBACheckBox_' + o.ID).not(':checked').length === 0)
							CheckAllLinkText = "Uncheck All";

						var $checkalldiv = $('<div style="text-align: right"><a href="#">'+ CheckAllLinkText +'</a></div>')
											 .appendTo($Div);
						var $checkalllink = $checkalldiv.children('a');
						$checkalllink.click(function() { ToggleCheckAll($checkalllink, o.ID); return false; });
					}

				} // if-else(o.ReadOnly)
			}, // init

			getdata: function (options) { 
				
				var o = {
					ID: ''
				};

				if (options) {
					$.extend(o, options);
				}
				
				var $Div = $(this);
				var data = [];
				var $dataDiv = $Div.find('.' + storedDataClass);
				if (!isNullOrEmpty($dataDiv)) {
				    data = $dataDiv.data(storedData);
				}	
				return data;
			}
		};
	
	    
	    function transmorgify (options, $control) {
			    var $this = $control;

			    var o = {
                    dataAry: [],
			        nameCol: '',
			        keyCol: '',
			        valCol: ''
			    };
			    if(options) $.extend(o, options);

			    if (false === isNullOrEmpty(o.dataAry) && o.dataAry.length > 0) {
	   			    // get columns
                    var cols = [];

			        var firstProp = o.dataAry[0];
			        for (var column in firstProp) {

			            if (firstProp.hasOwnProperty(column)) {
			                var fieldname = column;
			                if (fieldname !== o.nameCol && fieldname !== o.keyCol)
			                {
			                    cols.push(fieldname);
			                }
			            }
			        }
			        if (false === isNullOrEmpty(o.valCol) && cols.indexOf(o.valCol) === -1) {
			            cols.push(o.valCol)
			                ;			        }

			        // get data
			        var data = [];

			        for (var i = 0; i < o.dataAry.length; i++) {
			            var thisSet = o.dataAry[i];

			            if (thisSet.hasOwnProperty(o.keyCol) && thisSet.hasOwnProperty(o.nameCol)) {
			                var values = [];
			                for (var v = 0; v < cols.length; v++) {
			                    if (thisSet.hasOwnProperty(cols[v])) {
			                        values.push(isTrue(thisSet[cols[v]]));
			                    }
			                }
			                var dataOpts = { 'label': thisSet[o.nameCol],
			                    'key': thisSet[o.keyCol],
			                    'values': values };
			                data.push(dataOpts);
			            }
			        }

			        var dataStore = {
                        cols: cols,
			            data: data
			        };

			        $this.data(storedData, dataStore);
			    }
			    return $this;
			}
	    
		function ToggleCheckAll($checkalllink, id)
		{
			// Are there any unchecked checkboxes?
			if($('.CBACheckBox_' + id).not(':checked').length > 0)
			{
				CheckAll($checkalllink, id);
			} else {
				UncheckAll($checkalllink, id);
			}
		} // ToggleCheckAll()

		function CheckAll($checkalllink, id)
		{
			$('.CBACheckBox_' + id).CswAttrDom('checked', 'checked');
			$checkalllink.text('Uncheck all');
		}
		function UncheckAll($checkalllink, id)
		{
			$('.CBACheckBox_' + id).removeAttr('checked');
			$checkalllink.text('Check all');
		}

		// Method calling logic
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
		}    
  
	};
})(jQuery);
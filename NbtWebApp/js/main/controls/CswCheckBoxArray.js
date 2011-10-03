/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../tools/CswClientDb.js" />

; (function ($) { /// <param name="$" type="jQuery" />
    $.fn.CswCheckBoxArray = function (method) {
    
        var pluginName = 'CswCheckBoxArray',
            storedDataSuffix = '_cswCbaArrayDataStore',
            cbaPrevSelected = 'cswCba_prevSelected';
        
        var methods = {
            init: function(options) {
                var o = {
                    ID: '',
                    cols: [], //['col1', 'col2', 'col3'],
                    data: [], //[{ label: 'row1', 
//                             key: 1,
//                             values: [ true, false, true ] },
//                           { label: 'row2', 
//                             key: 2,
//                             values: [ false, true, false ] },
//                           { label: 'row3', 
//                             key: 3,
//                             values: [ true, false, true ] }],
                    HeightInRows: 4,
                    //CheckboxesOnLeft: false,
                    UseRadios: false,
                    Required: false,
                    ReadOnly: false,
                    Multi: false,
                    MultiIsUnchanged: true,
                    onchange: null, //function() { }
                    dataAry: [],
                    nameCol: '',
                    keyCol: '',
                    valCol: '',
                    valColName: ''
                };
                
                if (options) {
                    $.extend(o, options);
                }
                
                var storeDataId = o.ID + storedDataSuffix;
                var clientDb = new CswClientDb();
                clientDb.removeItem(storeDataId);
                clientDb.removeItem(cbaPrevSelected);
                
                var $Div = $(this);
                var cbaData = transmogrify({ 
                                    storeDataId: storeDataId,
                                    dataAry: o.dataAry,
                                    nameCol: o.nameCol,
                                    keyCol: o.keyCol,
                                    valCol: o.valCol,
                                    cols: o.cols
                                });
                if (false === isNullOrEmpty(cbaData)) {
                    $.extend(o, cbaData);
                }
                o.MultiIsUnchanged = o.Multi;

                var checkType = CswInput_Types.checkbox.name;
                if(o.UseRadios) {
                    checkType = CswInput_Types.radio.name;
                }
                
                var $OuterDiv = $('<div id="' + storeDataId + '"/>');

                clientDb.setItem(storeDataId, {columns: o.cols, data: o.data});
                
                if (o.ReadOnly) {
                    for (var r = 0; r < o.data.length; r++) {
                        var rRow = o.data[r];
                        var rowlabeled = false;
                        var first = true;
                        for (var c = 0; c < o.cols.length; c++) {
                            if (isTrue(rRow.values[c])) {
                                if (false === o.Multi) {
                                    if (false === rowlabeled) {
                                        $OuterDiv.append(rRow.label + ": ");
                                        rowlabeled = true;
                                    }
                                    if (false === first) {
                                        $OuterDiv.append(", ");
                                    }
                                    if(false === o.UseRadios) {
                                        $OuterDiv.append(o.cols[c]);
                                    }
                                    first = false;
                                }
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
                    for(var d = 0; d < o.cols.length; d++) {
                        var $dCell = $table.CswTable('cell', tablerow, d+2);
                        $dCell.addClass('cbarraycell');
                        var colName = o.cols[d];
                        if (colName === o.valCol && false === isNullOrEmpty(o.valColName)) {
                            colName = o.valColName;
                        }
                        if ((colName !== o.keyCol && colName !== o.nameCol)) {
                            $dCell.append(colName);
                        }
                    }
                    tablerow++;

                    //[none] row
                    if(o.UseRadios && false === o.Required) {
                        // Row label
                        var $labelcell = $table.CswTable('cell', tablerow, 1);
                        $labelcell.addClass('cbarraycell');
                        $labelcell.append('[none]');
                        for(var e = 0; e < o.cols.length; e++) {
                            var $eCell = $table.CswTable('cell', tablerow, e+2);
                            $eCell.addClass('cbarraycell');
                            var eCheckid = o.ID + '_none';
                            var $eCheck = $('<input type="'+ checkType +'" class="CBACheckBox_'+ o.ID +'" id="'+ eCheckid + '" name="' + o.ID + '" />')
                                           .appendTo($eCell)
                                           .click(function() {
                                               o.MultiIsUnchanged = false;
                                               o.onchange();
                                           });
                            $eCell.CswAttrXml({'key': '', rowlabel: '[none]', collabel: o.cols[e], row: -1, col: e });
                            if (false === o.Multi) {
                                $eCheck.CswAttrDom('checked', 'true'); // the browser will override this if another one is checked
                            }
                        } // for(var c = 0; c < o.cols.length; c++)
                    } // if(o.UseRadios && ! o.Required)
                    tablerow++;

                    var onChange = function(cB) {
                        //var cB = this;
                        var col = cB.attributes['col'].value;
                        var row = cB.attributes['row'].value;
                        var cache = clientDb.getItem(storeDataId);
                        cache.MultiIsUnchanged = false;
                        if (contains(cache.data, row) && contains(cache.data[row],'values')) {
                            cache.data[row].values[col] = cB.checked;
                            if(o.UseRadios) { //we're toggling--cache the prev selected row/col to deselect on later change
                                var data = clientDb.getItem(cbaPrevSelected);
                                if(contains(data,'row') && contains(data,'col')) {
                                    cache.data[data.row].values[data.col] = false;
                                }
                                clientDb.setItem(cbaPrevSelected, {row: row, col: col});
                            }
                        }      
                        clientDb.setItem(storeDataId, cache);
                    };
                    
                    // Data
                    for(var s = 0; s < o.data.length; s++) {
                        var sRow = o.data[s];
                        // Row label
                        var $sLabelcell = $table.CswTable('cell', tablerow + s, 1);
                        $sLabelcell.addClass('cbarraycell');
                        $sLabelcell.append(sRow.label);
                        
                        for(var f = 0; f < o.cols.length; f++) {
                            var $fCell = $table.CswTable('cell', tablerow + s, f+2);
                            $fCell.addClass('cbarraycell');
                            var fCheckid = o.ID + '_' + s + '_' + f;
                            var $fCheck = $('<input type="'+ checkType +'" class="CBACheckBox_'+ o.ID +'" id="'+ fCheckid + '" name="' + o.ID + '" />')
                                           .appendTo($fCell)
                                           .bind('click', o.onchange)
                                           .CswAttrXml({key: sRow.key, rowlabel: sRow.label, collabel: o.cols[f], row: s, col: f })
                                           .bind('change', function() { onChange(this); });
                            $.data($fCheck, 'thisRow', sRow);

                            if(sRow.values[f]) {
                                if(o.UseRadios) {
                                    clientDb.setItem('currentSelected', { col: f, row: s });
                                }
                                $fCheck.CswAttrDom('checked', 'true');
                            }
                        } // for(var c = 0; c < o.cols.length; c++)
                    } // for(var r = 0; r < o.data.length; r++)

                    if(false === o.UseRadios) {
                        var CheckAllLinkText = "Check All";
                        if($('.CBACheckBox_' + o.ID).not(':checked').length === 0)
                            CheckAllLinkText = "Uncheck All";

                        var $checkalldiv = $('<div style="text-align: right"><a href="#">'+ CheckAllLinkText +'</a></div>')
                                             .appendTo($Div);
                        var $checkalllink = $checkalldiv.children('a');
                        $checkalllink.click(function() { toggleCheckAll($checkalllink, o.ID); return false; });
                    }

                } // if-else(o.ReadOnly)
                
                $Div.contents().remove();
                $Div.append($OuterDiv);
                return $Div;
            }, // init

            getdata: function (options) { 
                
                var o = {
                    ID: ''
                };

                if (options) {
                    $.extend(o, options);
                }
                var storeDataId = o.ID + storedDataSuffix;
                var clientDb = new CswClientDb();
                var data = clientDb.getItem(storeDataId);
                return data;
            }
        };
        
        function transmogrify (options) {
            var dataStore = {
                cols: [],
                data: []
            };
            var o = {
                storeDataId: '',
                dataAry: [],
                nameCol: '',
                keyCol: '',
                valCol: '',
                cols: []
            };
            if(options) $.extend(o, options);
            
            if (false === isNullOrEmpty(o.dataAry) && o.dataAry.length > 0) {
                // get columns
                var cols = o.cols;
                if (hasLength(cols) && cols.length === 0) {
                    var firstProp = o.dataAry[0];
                    for (var column in firstProp) {

                        if (contains(firstProp, column)) {
                            var fieldname = column;
                            if (fieldname !== o.nameCol && fieldname !== o.keyCol)
                            {
                                cols.push(fieldname);
                            }
                        }
                    }
                }
                if (false === isNullOrEmpty(o.valCol) && false === contains(cols,o.valCol)) {
                    cols.push(o.valCol);
                }

                // get data
                var data = [];

                for (var i = 0; i < o.dataAry.length; i++) {
                    var thisSet = o.dataAry[i];

                    if (contains(thisSet, o.keyCol) && contains(thisSet, o.nameCol)) {
                        var values = [];
                        for (var v = 0; v < cols.length; v++) {
                            if (contains(thisSet, cols[v])) {
                                values.push(isTrue(thisSet[cols[v]]));
                            }
                        }
                        var dataOpts = { 'label': thisSet[o.nameCol],
                            'key': thisSet[o.keyCol],
                            'values': values };
                        data.push(dataOpts);
                    }
                }

                dataStore.cols = cols;
                dataStore.data = data;
                var clientDb = new CswClientDb();
                clientDb.setItem(o.storeDataId, dataStore);
            }
            return dataStore;
        }
        
        function toggleCheckAll($checkalllink, id)
        {
            // Are there any unchecked checkboxes?
            if($('.CBACheckBox_' + id).not(':checked').length > 0)
            {
                checkAll($checkalllink, id);
            } else {
                uncheckAll($checkalllink, id);
            }
        } // ToggleCheckAll()

        function checkAll($checkalllink, id)
        {
            $('.CBACheckBox_' + id).CswAttrDom('checked', 'checked').click();
            $checkalllink.text('Uncheck all');
        }
        function uncheckAll($checkalllink, id)
        {
            $('.CBACheckBox_' + id).removeAttr('checked').click();
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
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../controls/CswGrid.js" />

; (function ($) { /// <param name="$" type="jQuery" />
    "use strict";    
    var pluginName = 'CswQuotas';

    var methods = {
        init: function(options) {
            var o = {
                Url: '/NbtWebApp/wsNBT.asmx/getQuotas',
                SaveUrl: '/NbtWebApp/wsNBT.asmx/saveQuotas',
                ID: 'action_quotas',
                onQuotaChange: null // function() { }
            };
            if(options) $.extend(o, options);

            var $Div = $(this);
            var $table;
            var row;
            var $cell1, $cell2, $cell3, $cell4;
            var quotaJson;

            function initTable() {
                $Div.contents().remove();
                $table = $Div.CswTable('init', { ID: o.ID + '_tbl', border: 1, cellpadding: 5 });
                row = 1;

                // Header row
                $cell1 = $table.CswTable('cell', row, 1);
                $cell1.append('<b>Object Class</b>');
                $cell2 = $table.CswTable('cell', row, 2);
                $cell2.append('<b>Node Types</b>');
                $cell3 = $table.CswTable('cell', row, 3);
                $cell3.append('<b>Current Usage</b>');
                $cell4 = $table.CswTable('cell', row, 4);
                $cell4.append('<b>Quota</b>');
                row += 1;

                // Quota table
                CswAjaxJson({
                    url: o.Url,
                    data: {},
                    success: function(result) {
                        quotaJson = result;
                        var canedit = isTrue(quotaJson.canedit);
                    
                        crawlObject(quotaJson.objectclasses, function (childObj) {
                            if(tryParseNumber(childObj.nodetypecount) > 0) {

                                // one object class row                                
                                makeQuotaRow($table, row, canedit, 'OC_' + childObj.objectclassid, childObj.objectclass, '', childObj.currentusage, childObj.quota);
                                row += 1;

                                // several nodetype rows
                                crawlObject(childObj.nodetypes, function (childObj_nt) {
                                    makeQuotaRow($table, row, canedit, 'NT_' + childObj_nt.nodetypeid, '', childObj_nt.nodetypename, childObj_nt.currentusage, childObj_nt.quota);
                                    row += 1;
                                }, false);
                            }
                        }, false); // crawlObject()

                        if(canedit) {
                            $Div.CswButton({
                                ID: o.ID + '_save',
                                enabledText: 'Save',
                                disabledText: 'Saving',
                                onclick: handleSave
                            });
                        }
                    } // success
                }); // ajax()
            } // initTable()

            function makeQuotaRow($table, row, canedit, id, objectclass, nodetype, currentusage, quota)
            {
                // one object class row                                
                $cell1 = $table.CswTable('cell', row, 1);
                $cell1.append(objectclass);

                $cell2 = $table.CswTable('cell', row, 2);
                $cell2.append(nodetype);

                $cell3 = $table.CswTable('cell', row, 3);
                $cell3.append(currentusage);
                                
                $cell4 = $table.CswTable('cell', row, 4);
                if(canedit) {
                    $cell4.CswInput({	
                        ID: o.ID + '_' + id + '_quota',
                        name: o.ID + '_' + id + '_quota',
                        type: CswInput_Types.text,
                        value: quota,
                        width: '50px'
                    });
                } else {
                    $cell4.append(quota);
                }
            } // makeQuotaRow()

            function handleSave() {
                crawlObject(quotaJson.objectclasses, function (childObj) {
                    childObj.quota = $('#' + o.ID + '_OC_' + childObj.objectclassid + '_quota').val();
                    crawlObject(childObj.nodetypes, function (childObj_nt) {
                        childObj_nt.quota = $('#' + o.ID + '_NT_' + childObj_nt.nodetypeid + '_quota').val();
                    }, false);
                }, false);

                CswAjaxJson({
                    url: o.SaveUrl,
                    data: { Quotas: JSON.stringify(quotaJson) },
                    success: function() {
                        initTable();
                        ChemSW.tools.tryExecMethod(o.onQuotaChange);
                    }
                });
            } // handleSave()

            initTable();
        } // init
    }; // methods

    
    // Method calling logic
    $.fn.CswQuotas = function (method) {
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }
    };
})(jQuery);

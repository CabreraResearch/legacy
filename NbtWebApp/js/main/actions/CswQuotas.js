/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

; (function ($) { 
    "use strict";    
    var pluginName = 'CswQuotas';

    var methods = {
        init: function (options) {
            var o = {
                Url: '/NbtWebApp/wsNBT.asmx/getQuotas',
                SaveUrl: '/NbtWebApp/wsNBT.asmx/saveQuotas',
                ID: 'action_quotas',
                onQuotaChange: null // function () { }
            };
            if(options) $.extend(o, options);

            var $Div = $(this);
            var table;
            var row;
            var quotaJson;

            function initTable() {
                $Div.contents().remove();
                table = Csw.controls.table({
                    $parent: $Div,
                    ID: Csw.controls.dom.makeId(o.ID, 'tbl'), 
                    border: 1, 
                    cellpadding: 5
                });
                row = 1;

                // Header row
                table.add(row, 1, '<b>Object Class</b>');
                table.add(row, 2, '<b>Node Types</b>');
                table.add(row, 3, '<b>Current Usage</b>');
                table.add(row, 4, '<b>Quota</b>');
                row += 1;

                // Quota table
                Csw.ajax.post({
                    url: o.Url,
                    data: {},
                    success: function (result) {
                        quotaJson = result;
                        var canedit = Csw.bool(quotaJson.canedit);
                    
                        Csw.crawlObject(quotaJson.objectclasses, function (childObj) {
                            if(Csw.number(childObj.nodetypecount) > 0) {

                                // one object class row                                
                                makeQuotaRow(row, canedit, 'OC_' + childObj.objectclassid, childObj.objectclass, '', childObj.currentusage, childObj.quota);
                                row += 1;

                                // several nodetype rows
                                Csw.crawlObject(childObj.nodetypes, function (childObj_nt) {
                                    makeQuotaRow(row, canedit, 'NT_' + childObj_nt.nodetypeid, '', childObj_nt.nodetypename, childObj_nt.currentusage, childObj_nt.quota);
                                    row += 1;
                                }, false);
                            }
                        }, false); // Csw.crawlObject()

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

            function makeQuotaRow(row, canedit, id, objectclass, nodetype, currentusage, quota) {
                // one object class row                                
                var cell4;
                table.add(row, 1, objectclass);
                table.add(row, 2, nodetype);
                table.add(row, 3, currentusage);
                
                if(canedit) {
                    cell4 = table.cell(row, 4);
                    cell4.$.CswInput({	
                        ID: o.ID + '_' + id + '_quota',
                        name: o.ID + '_' + id + '_quota',
                        type: Csw.enums.inputTypes.text,
                        value: quota,
                        width: '50px'
                    });
                } else {
                    table.add(row, 4, quota);
                }
            } // makeQuotaRow()

            function handleSave() {
                Csw.crawlObject(quotaJson.objectclasses, function (childObj) {
                    childObj.quota = $('#' + o.ID + '_OC_' + childObj.objectclassid + '_quota').val();
                    Csw.crawlObject(childObj.nodetypes, function (childObj_nt) {
                        childObj_nt.quota = $('#' + o.ID + '_NT_' + childObj_nt.nodetypeid + '_quota').val();
                    }, false);
                }, false);

                Csw.ajax.post({
                    url: o.SaveUrl,
                    data: { Quotas: JSON.stringify(quotaJson) },
                    success: function () {
                        initTable();
                        Csw.tryExec(o.onQuotaChange);
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

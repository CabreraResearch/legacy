/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../controls/CswGrid.js" />
/// <reference path="../pagecmp/CswDialog.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";    
    var pluginName = 'CswNodeTable';
    
    var methods = {
    
        'init': function (options) {

            var o = {
                TableUrl: '/NbtWebApp/wsNBT.asmx/getTable',
                viewid: '',
                ID: '',
                nodeid: '',
                cswnbtnodekey: '',
                EditMode: EditMode.Edit.name,
                //onAddNode: function(nodeid,cswnbtnodekey){},
                onEditNode: null, //function(nodeid,cswnbtnodekey){},
                onDeleteNode: null, //function(nodeid,cswnbtnodekey){}
                onSuccess: null, // function() {}
                rowsize: 3
            };
            if (options) $.extend(o, options);
            
            var $parent = $(this);
            var $table = $parent.CswLayoutTable('init', { 
                                                    ID: o.ID + '_tbl', 
                                                    cellset: { rows: 2, columns: 1 },
                                                    cellalign: 'center'
                                                });

            CswAjaxJson({
                url: o.TableUrl,
                data: { 
                    ViewId: o.viewid, 
                    NodeId: o.nodeid, 
                    NodeKey: o.cswnbtnodekey 
                },
                success: function (data) {
                    var r = 1;
                    var c = 1;

                    crawlObject(data, function(nodeObj, nodeid) {
                        var cellset = $table.CswLayoutTable('cellset', r, c);
                        var $thumbnailcell = cellset[1][1]
                                                .css({ 
                                                    width: '33%',
                                                    verticalAlign: 'bottom'
                                                     });
                        var $textcell = cellset[2][1]
                                                .css({ width: '33%' });
                        var name = '<b>' + nodeObj.nodename + '</b>';
                        var locked = isTrue(nodeObj.locked);

                        $thumbnailcell.append('<img src="'+ nodeObj.thumbnailurl +'" style="max-width: 90%;"><br/>');
                        if(locked) {
                            name += '<img src="Images/quota/lock.gif" title="Quota exceeded" />';
                        }
                        $textcell.append(name + '<br/>');
                        
                        crawlObject(nodeObj.props, function(propObj, propid) {
                            $textcell.append('' + propObj.propname + ': ' + propObj.gestalt + '<br/>');
                        });
            
                        c++;
                        if(c > o.rowsize) { c = 1; r++; }
                    });


                    if (isFunction(o.onSuccess)) {
                        o.onSuccess();
                    }
                } // success{} 
            }); // ajax
        } // 'init'
    }; // methods

    $.fn.CswNodeTable = function(method) {
        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }
    };

})(jQuery);


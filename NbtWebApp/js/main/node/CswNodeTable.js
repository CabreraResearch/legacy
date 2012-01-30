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
                columns: 3,
                maxlength: 35,
                rowpadding: 25
            };
            if (options) $.extend(o, options);
            
            var $parent = $(this);
            var $table = $parent.CswLayoutTable('init', { 
                                                    ID: o.ID + '_tbl', 
                                                    cellset: { rows: 2, columns: 1 },
                                                    cellalign: 'center',
                                                    width: '100%',
                                                    cellspacing: '5px'
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

                    crawlObject(data, function(nodeObj) {
                        if (nodeObj.nodename == "Results Truncated")
                        {
                            c = 1; 
                            r+=1;
                        }
                        var cellset = $table.CswLayoutTable('cellset', r, c);
                        var width = (1/o.columns * 100) + '%';
                        var $thumbnailcell = cellset[1][1]
                                                .css({ 
                                                        paddingTop: o.rowpadding + 'px',
                                                        width: width,
                                                        verticalAlign: 'bottom'
                                                     });
                        var $textcell = cellset[2][1]
                                                .css({
                                                    width: width 
                                                });
                        var name;
                        if (nodeObj.nodename.length > o.maxlength)
                        {
                            name = '<b>' + nodeObj.nodename.substr(0,o.maxlength) + '...</b>';
                        } else {
                            name = '<b>' + nodeObj.nodename + '</b>';
                        }
                        var locked = isTrue(nodeObj.locked);

                        if (false === isNullOrEmpty(nodeObj.thumbnailurl))
                        {
                            $thumbnailcell.append('<img src="' + nodeObj.thumbnailurl + '" style="max-width: 90%;">');
                        }
                        $thumbnailcell.append('<br/>');

                        if(locked) {
                            name += '<img src="Images/quota/lock.gif" title="Quota exceeded" />';
                        }
                        $textcell.append(name + '<br/>');
                        
                        crawlObject(nodeObj.props, function(propObj) {
                            $textcell.append('' + propObj.propname + ': ');
                            if (propObj.gestalt.length > o.maxlength)
                            {
                                $textcell.append(propObj.gestalt.substr(0,o.maxlength) + '...');
                            }else {
                                $textcell.append(propObj.gestalt);
                            }
                            $textcell.append('<br/>');
                        });
            
                        c += 1;
                        if (c > o.columns) { c = 1; r += 1; }
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
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }
    };

})(jQuery);


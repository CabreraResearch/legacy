; (function ($) {
        
    var PluginName = 'CswDialog';

    var methods = {

        // Specialized

        'AddNodeDialog': function (nodetypeid, onAddNode) {
                            var $div = $('<div></div>');
                            $div.CswNodeTabs({
                                'nodetypeid': nodetypeid,
                                'EditMode': 'AddInPopup',
                                'onSave': function (nodeid) {
                                    $div.dialog('close');
                                    onAddNode(nodeid);
                                }
                            });
                            $div.dialog({ 'modal': true,
                                'width': 800,
                                'height': 600
                            });
                        },        

        'EditNodeDialog': function (nodeid, onEditNode) {
                            var $div = $('<div></div>');
                            $div.CswNodeTabs({
                                'nodeid': nodeid,
                                'EditMode': 'EditInPopup',
                                'onSave': function (nodeid) {
                                    $div.dialog('close');
                                    onEditNode(nodeid);
                                }
                            });
                            $div.dialog({ 'modal': true,
                                'width': 800,
                                'height': 600
                            });
                        },

        'DeleteNodeDialog': function (nodename, nodeid, onDeleteNode) {
                            var $div = $('<div>Are you sure you want to delete: ' + nodename + '?<br/><br/></div>');

                            $('<input type="button" id="deletenode_submit" name="deletenode_submit" value="Delete" />')
                                .appendTo($div)
                                .click(function () {
                                    $div.dialog('close');
                                    deleteNode(nodeid, onDeleteNode);
                                });

                            $('<input type="button" id="deletenode_cancel" name="deletenode_cancel" value="Cancel" />')
                                .appendTo($div)
                                .click(function () {
                                    $div.dialog('close');
                                });

                            $div.dialog({ 'modal': true,
                                'width': 400,
                                'height': 200
                            });
                        },

        'AboutDialog': function () {
                            var $div = $('<div></div>');
                            CswAjaxXml({
                                url: '/NbtWebApp/wsNBT.asmx/getAbout',
                                data: '',
                                success: function ($xml) {
                                    $div.append('NBT Assembly Version: ' + $xml.children('assembly').text() + '<br/><br/>');
                                    var $table = makeTable('abouttable')
                                                  .appendTo($div);
                                    var row = 1;
                                    $xml.children('component').each(function () {
                                        var $namecell = getTableCell($table, row, 1);
                                        var $versioncell = getTableCell($table, row, 2);
                                        var $copyrightcell = getTableCell($table, row, 3);
                                        $namecell.css('padding', '2px 5px 2px 5px');
                                        $versioncell.css('padding', '2px 5px 2px 5px');
                                        $copyrightcell.css('padding', '2px 5px 2px 5px');
                                        var $component = $(this);
                                        $namecell.append($component.children('name').text());
                                        $versioncell.append($component.children('version').text());
                                        $copyrightcell.append($component.children('copyright').text());
                                        row++;
                                    });
                                }
                            });
                            $div.dialog({ 'modal': true,
                                'width': 600,
                                'height': 400
                            });
                        },


        // Generic

        'OpenPopup': function(url) { 
                            var popup = window.open(url, null, 'height=600, width=600, status=no, resizable=yes, scrollbars=yes, toolbar=yes,location=no, menubar=yes');
                            popup.focus();
                            return popup;
                        },
        'OpenDialog': function (id, url) {
                            var $dialogdiv = $('<div id="' + id + '"></div>');
                            $dialogdiv.load(url,
                                            {},
                                            function (responseText, textStatus, XMLHttpRequest) {
                                                $dialogdiv.appendTo('body')
                                                          .dialog();
                                            });
                        },
        'CloseDialog': function (id) {
                            $('#' + id)
                                .dialog('close')
                                .remove();
                        }
    };
    
    // Method calling logic
    $.CswDialog = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);

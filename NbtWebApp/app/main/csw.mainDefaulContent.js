/// <reference path="CswApp-vsdoc.js" />

(function _initMain() {

    Csw.main.onReady.then(function() {

        function render(div, options) {
            var o = {
                viewid: '',
                viewmode: '',
                onAddNode: null
            };
            Csw.extend(o, options);

            return  Csw.ajax.post({
                urlMethod: 'getDefaultContent',
                data: { ViewId: o.viewid },
                success: function(data) {

                    var addDiv = div.div({
                        name: 'adddiv',
                        cssclass: 'adddiv',
                        text: 'Add New:'
                    }).hide();

                    function _makeAddLinksRecursive(addObj, parent) {
                        var ul = parent.ul();

                        function onEach(entryObj) {
                            var $li = handleItem({
                                itemJson: entryObj,
                                onAlterNode: o.onAddNode
                            }).appendTo(ul.$);
                            addDiv.show();
                        }

                        if (Csw.contains(addObj, 'entries')) {
                            Csw.iterate(addObj.entries, onEach);
                            if (ul.children().length() > 0) {
                                _makeAddLinksRecursive(addObj.children, ul);
                            } else {
                                _makeAddLinksRecursive(addObj.children, parent);
                            }
                        } // if(contains(addObj, 'entries'))
                    }

                    // _makeAddLinksRecursive()

                    _makeAddLinksRecursive(data, addDiv);
                } // success
            }); // ajax
        }

        function handleItem(options) {
            'use strict';
            var o = {
                itemJson: {},
                onAlterNode: null // function (nodeid, nodekey) { },
            };
            if (options) Csw.extend(o, options);
            var text = o.itemJson.text;
            var $li = $('<li><a href="#">' + text + '</a></li>');
            $li.css({ 'list-style-image': 'url(' + o.itemJson.icon + ')' });
            var $a = $li.children('a');

            $a.click(function () {
                Csw.layouts.addnode({
                    action: o.itemJson.action,
                    title: 'Add New ' + text,
                    nodetypeid: Csw.string(o.itemJson.nodetypeid),
                    relatednodeid: Csw.string(o.itemJson.relatednodeid),
                    onAddNode: o.onAlterNode
                });
                return false;
            });
            return $li;
        } // handleItem()

        Csw.main.register('showDefaultContentTree', function (viewopts) {
            var v = {
                viewid: '',
                viewmode: '',
                onAddNode: function(nodeid, nodekey) {
                    Csw.main.refreshSelected({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true });
                }
            };
            Csw.extend(v, viewopts);

            Csw.main.clear({ right: true });

            return render(Csw.main.rightDiv, v);

        }); // showDefaultContentTree()

        Csw.main.register('showDefaultContentTable', function(viewopts) {
            var v = {
                viewid: '',
                viewmode: '',
                onAddNode: function(nodeid, nodekey) {
                    Csw.main.refreshSelected({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true });
                }
            };
            Csw.extend(v, viewopts);

            Csw.main.clear({ centerbottom: true });

            var div = Csw.main.centerBottomDiv.div({
                name: 'deftbldiv',
                align: 'center'
            });
            div.css({ textAlign: 'center' });
            div.append('No Results.');

            return render(div, v);
        }); // showDefaultContentTable()

    });
}());
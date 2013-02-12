
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';

    var hoverButton;
    Csw.nodeHoverIn = Csw.nodeHoverIn ||
        Csw.register('nodeHoverIn', function (event, nodeid, nodekey, nodename, parentDiv, buttonHoverIn, buttonHoverOut) {
            'use strict';
            var previewopts = {
                name: nodeid + '_preview',
                nodeid: nodeid,
                nodekey: nodekey,
                nodename: nodename,
                eventArg: event
            };

            if (false === Csw.isNullOrEmpty(nodeid)) {
                hoverButton = parentDiv.buttonExt({
                    name: 'preview',
                    enabledText: '',
                    width: '100px',
                    disableOnClick: false,
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.magglass),
                    onHoverIn: function() { Csw.tryExec(buttonHoverIn); },
                    onHoverOut: function() { Csw.tryExec(buttonHoverOut); },
                    onClick: function(btn, btnEvent) {
                        previewopts.top = btnEvent.pageY;
                        previewopts.left = btnEvent.pageX;
                        var preview = Csw.nbt.nodePreview(Csw.main.body, previewopts);
                        preview.open();
                        return false;
                    }
                }).css({
                    position: 'absolute',
                    top: parentDiv.$.position().top + 'px',
                    right: '5px'
                });
            }
        }); // Csw.nodeHoverIn 


    Csw.nodeHoverOut = Csw.nodeHoverOut ||
        Csw.register('nodeHoverOut', function (event, nodeid) {
            if (null != hoverButton) {
                hoverButton.remove();
                hoverButton = null;
            }
        }); // Csw.nodeHoverOut


    Csw.nbt.nodePreview = Csw.nbt.nodePreview ||
        Csw.nbt.register('nodePreview', function (cswParent, options) {
            'use strict';

            var cswPrivate = {
                name: '',
                nodeid: '',
                nodekey: '',
                nodename: '',
                eventArg: {},
                top: 0,
                left: 0
            };
            Csw.extend(cswPrivate, options);

            var cswPublic = {};

            cswPublic.open = function () {

                cswPrivate.extWindow = Ext.create('Ext.window.Window', {
                    title: cswPrivate.nodename,
                    y: cswPrivate.top,
                    x: cswPrivate.left,
                    height: 200,
                    width: 600,
                    layout: 'fit',
                    items: {
                        xtype: 'component',
                        layout: 'fit'
                    }
                }).show();
                cswPrivate.div = Csw.domNode({
                    el: cswPrivate.extWindow.items.items[0].getEl().dom,
                    ID: cswPrivate.extWindow.items.items[0].getEl().id
                }).css({
                    background: 'transparent'
                });

                cswPrivate.loadingDiv = cswPrivate.div.div({
                    text: '&nbsp;&nbsp;&nbsp;Loading...',
                    styles: {
                        fontStyle: 'italic'
                    }
                });

                cswPrivate.previewTabsAndProps = Csw.layouts.tabsAndProps(cswPrivate.div, {
                    name: cswPrivate.name + 'tabs',
                    globalState: {
                        currentNodeId: cswPrivate.nodeid,
                        currentNodeKey: cswPrivate.nodekey,
                        ShowAsReport: false
                    },
                    tabState: {
                        EditMode: Csw.enums.editMode.Preview,
                        showSaveButton: false
                    },
                    AjaxWatchGlobal: false,
                    onInitFinish: function (AtLeastOneProp) {
                        cswPrivate.loadingDiv.remove();
                        if (false === AtLeastOneProp) {
                            cswPublic.close();
                        }
                    }
                });
            }; // open()


            cswPublic.close = function () {
                cswPrivate.div.remove();
            }; // close()

            return cswPublic;
        }); // Csw.nbt.nodePreview
} ());

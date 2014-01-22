(function () {

    Csw.layouts.register('searchNode', function (cswHelpers) {
        var cswPrivate = {};
        Csw.extend(cswPrivate, cswHelpers);

        var cswPublic = {};
        
        cswPublic.activeTabId = 0;

        cswPublic.render = function (div) {
            var searchTbl = div.table();

            var imageCell = searchTbl.cell(2, 1).div().css('width', '200px');
            var labelCell = searchTbl.cell(1, 2).div().css('height', '20px');
            var propsCell = searchTbl.cell(2, 2).div().css('width', '400px');
            var buttonsCell = searchTbl.cell(2, 3).div().css('width', '400px');
            var disabledButtonsCell = searchTbl.cell(2, 4).div().css('width', '400px');

            Csw.ajaxWcf.post({
                urlMethod: 'Design/GetSearchImageLink',
                data: cswPrivate.nodeId,
                success: function (ret) {
                    imageCell.img({
                        src: ret.imagelink
                    });
                }
            });

            var propsPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: propsCell.getId(),
                border: 0,
                layout: {
                    align: 'stretch',
                    padding: 1
                }
            });

            var buttonsPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: buttonsCell.getId(),
                border: 0,
                layout: {
                    align: 'stretch',
                    padding: 1
                }
            });

            Csw.ajax.deprecatedWsNbt({
                urlMethod: 'getProps',
                data: {
                    EditMode: cswPrivate.Layout,
                    NodeId: cswPrivate.nodeId,
                    TabId: Csw.int32MinVal,
                    SafeNodeKey: cswPrivate.nodeKey,
                    NodeTypeId: cswPrivate.nodeTypeId,
                    Date: new Date().toDateString(),
                    Multi: false,
                    filterToPropId: '',
                    ConfigMode: true,
                    RelatedNodeId: '',
                    GetIdentityTab: false,
                    ForceReadOnly: false
                },
                success: function (data) {
                    //split buttons and every other prop
                    var buttonProps = [];
                    var otherProps = [];
                    Csw.iterate(data.properties, function (prop) {
                        if ('Button' === prop.fieldtype) {
                            buttonProps.push(prop);
                        } else {
                            otherProps.push(prop);
                        }
                    });
                    labelCell.setLabelText(data.node.nodename, false, false);

                    cswPrivate.renderProps(data.node, otherProps, propsPanel.id, '', false);
                    cswPrivate.renderProps(data.node, buttonProps, buttonsPanel.id, '', false);

                } // success
            }); // ajax

            var disabledBtnsTbl = disabledButtonsCell.table({
                cellspacing: 5,
                cellpadding: 5
            });
            disabledBtnsTbl.cell(1, 1).buttonExt({
                isEnabled: false,
                enabledText: 'Details',
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil)
            }).disable();

            disabledBtnsTbl.cell(1, 2).buttonExt({
                isEnabled: false,
                enabledText: 'Delete',
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.trash)
            }).disable();
        };

        return cswPublic;
    });
})();
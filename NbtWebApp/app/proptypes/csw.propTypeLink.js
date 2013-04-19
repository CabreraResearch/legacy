/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.link = Csw.properties.register('link',
        function(nodeProperty) {
            'use strict';

            //The render function to be executed as a callback
            var render = function() {
                'use strict';
                var cswPrivate = Csw.object();

                cswPrivate.text = nodeProperty.propData.values.text;
                cswPrivate.href = nodeProperty.propData.values.href;
                cswPrivate.url = nodeProperty.propData.values.url;

                if (nodeProperty.isReadOnly()) {
                    nodeProperty.propDiv.a({
                        href: cswPrivate.url,
                        text: cswPrivate.text
                    });
                } else {

                    var table = nodeProperty.propDiv.table();

                    var updateProp = function(val) {
                        //Case 29390: no sync for Links

                        cswPrivate.text = val.text;
                        cswPrivate.href = val.href;
                        cswPrivate.url = val.url;

                        nodeProperty.propData.values.text = val.text;
                        nodeProperty.propData.values.href = val.href;
                        nodeProperty.propData.values.url = val.url;
                        nodeProperty.broadcastPropChange(val);
                    };


                    table.empty();
                    if (nodeProperty.isDisabled()) {
                        table.cell(1, 1).p({
                            text: cswPrivate.text
                        });
                    } else {
                        table.cell(1, 1).a({
                            href: cswPrivate.url,
                            text: cswPrivate.text
                        });
                    }
                    cswPrivate.cell12 = table.cell(1, 2).div();

                    cswPrivate.cell12.icon({
                        iconType: Csw.enums.iconType.pencil,
                        hovertext: 'Edit',
                        size: 16,
                        isButton: true,
                        onClick: function() {
                            cswPrivate.editTable.show();
                        }
                    });

                    cswPrivate.editTable = nodeProperty.propDiv.table().hide();

                    cswPrivate.editTable.cell(1, 1).span({ text: 'Text' });

                    cswPrivate.editTextCell = cswPrivate.editTable.cell(1, 2);
                    cswPrivate.editText = cswPrivate.editTextCell.input({
                        name: nodeProperty.name + '_text',
                        type: Csw.enums.inputTypes.text,
                        value: cswPrivate.text,
                        onChange: function(text) {
                            nodeProperty.propData.values.text = text;
                            updateProp(nodeProperty.propData.values);
                        }
                    });

                    cswPrivate.editTable.cell(2, 1).span({ text: 'URL body' });

                    cswPrivate.editHrefCell = cswPrivate.editTable.cell(2, 2);
                    cswPrivate.editHref = cswPrivate.editHrefCell.input({
                        name: nodeProperty.name + '_href',
                        type: Csw.enums.inputTypes.text,
                        value: cswPrivate.href,
                        onChange: function(href) {
                            nodeProperty.propData.values.href = href;
                            updateProp(nodeProperty.propData.values);
                        }
                    });

                    cswPrivate.urlCell = cswPrivate.editTable.cell(3, 2);
                    cswPrivate.urlText = cswPrivate.urlCell.span({
                        text: cswPrivate.url
                    });

                    cswPrivate.editText.required(nodeProperty.isRequired());
                    cswPrivate.editHref.required(nodeProperty.isRequired());
                    if (nodeProperty.isRequired() && cswPrivate.href === '') {
                        cswPrivate.editTable.show();
                    }
                    cswPrivate.editText.clickOnEnter(function() {
                        cswPrivate.publish('CswSaveTabsAndProp_tab' + nodeProperty.tabState.tabid + '_' + nodeProperty.tabState.nodeid);
                    });
                    cswPrivate.editHref.clickOnEnter(function() {
                        cswPrivate.publish('CswSaveTabsAndProp_tab' + nodeProperty.tabState.tabid + '_' + nodeProperty.tabState.nodeid);
                    });
                }

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

} ());
  

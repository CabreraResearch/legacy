/// <reference path="../jquery/jquery-1.6.1-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
    $.fn.CswNodeTabs = function (options)
    {

        var o = {
            ID: '',
            TabsUrl: '/NbtWebApp/wsNBT.asmx/getTabs',
            SinglePropUrl: '/NbtWebApp/wsNBT.asmx/getSingleProp',
            PropsUrl: '/NbtWebApp/wsNBT.asmx/getProps',
            MovePropUrl: '/NbtWebApp/wsNBT.asmx/moveProp',
            SavePropUrl: '/NbtWebApp/wsNBT.asmx/saveProps',
            CopyPropValuesUrl: '/NbtWebApp/wsNBT.asmx/copyPropValues',
            nodeid: '',               
            relatednodeid: '',
            tabid: '',                
            cswnbtnodekey: '',        
            nodetypeid: '',           
            filterToPropId: '',       
            title: '',
            EditMode: 'Edit', // Edit, AddInPopup, EditInPopup, Demo, PrintReport, DefaultValue
            onSave: function (nodeid, cswnbtnodekey) { },
            onBeforeTabSelect: function (tabid) { return true; },
            onTabSelect: function (tabid) { },
            onPropertyChange: function(propid, propname) { },
            ShowCheckboxes: false,
            NodeCheckTreeId: '',
            'onEditView': function(viewid) { }
        };

        if (options)
        {
            $.extend(o, options);
        }
        var $parent = $(this);

        var $outertabdiv = $('<div id="' + o.ID + '_tabdiv" />')
                        .appendTo($parent);

        getTabs(o);

        if(o.EditMode !== 'PrintReport')
        {
            var $linkdiv = $('<div id="' + o.ID + '_linkdiv" align="right"/>')
                            .appendTo($parent);
            var $AsReportLink = $('<a href="#">As Report</a>')
                            .appendTo($linkdiv)
                            .click(function() { openPopup('NewNodeReport.html?nodeid=' + o.nodeid + '&cswnbtnodekey=' + o.cswnbtnodekey, 600, 800); });
        }

        function clearTabs()
        {
            $outertabdiv.contents().remove();
        }

        function getTabs()
        {
            var dataXml = {
                EditMode: o.EditMode,
                NodeId: o.nodeid,
                SafeNodeKey: o.cswnbtnodekey,
                NodeTypeId: o.nodetypeid
            };

            CswAjaxXml({
                url: o.TabsUrl,
                data: dataXml,
                stringify: false,
                success: function ($xml)
                {
                    clearTabs();
                    var tabdivs = [];
                    var selectedtabno = 0;
                    var tabno = 0;

                    $xml.children().each(function ()
                    {
                        $tab = $(this);
                        if(o.EditMode === 'PrintReport' || tabdivs.length === 0)
                        {
                            // For PrintReports, we're going to make a separate tabstrip for each tab
                            tabdivs[tabdivs.length] = $("<div><ul></ul></div>").appendTo($outertabdiv);
                        }
                        var $tabdiv = tabdivs[tabdivs.length - 1];
                        $tabdiv.children('ul').append('<li><a href="#' + $tab.CswAttrXml('id') + '">' + $tab.CswAttrXml('name') + '</a></li>');
                        $tabdiv.append('<div id="' + $tab.CswAttrXml('id') + '"><form id="' + $tab.CswAttrXml('id') + '_form" /></div>');
                        if($tab.CswAttrXml('id') === o.tabid)
                        {
                            selectedtabno = tabno;
                        }
                        tabno++;
                    });  // $xml.children().each(function ()

                    for(var t in tabdivs)
                    {
                        var $tabdiv = tabdivs[t];
                        $tabdiv.tabs({
                            'selected': selectedtabno,
                            'select': function (event, ui)
                            {
                                if(o.onBeforeTabSelect(tabid))
                                {
                                    var $tabcontentdiv = $($tabdiv.children('div')[ui.index]);
                                    var tabid = $tabcontentdiv.CswAttrDom('id');
                                    getProps($tabcontentdiv, tabid);

                                    o.onTabSelect(tabid);
                                } else {
                                    return false;
                                }
                            }
                        });
                        var $tabcontentdiv = $($tabdiv.children('div')[$tabdiv.tabs('option', 'selected')]);
                        var selectedtabid = $tabcontentdiv.CswAttrDom('id');
                        getProps($tabcontentdiv, selectedtabid);
                        o.onTabSelect(selectedtabid);
                    } // for(var t in tabdivs)

                } // success{}
            }); // ajax
        } // getTabs()

        function getProps($tabcontentdiv, tabid)
        {
            var dataXml = {
                EditMode: o.EditMode,
                NodeId: o.nodeid,
                TabId: tabid, 
                SafeNodeKey: o.cswnbtnodekey,
                NodeTypeId: o.nodetypeid
            };

            CswAjaxXml({
                url: o.PropsUrl,
                data: dataXml,
                stringify: false,
                success: function ($xml)
                {
                    var $form = $tabcontentdiv.children('form');
                    $form.contents().remove();

                    if(o.title !== '')
                        $form.append(o.title);

                    var $layouttable = $form.CswLayoutTable('init', {
                        'ID': o.ID + '_props',
                        'OddCellRightAlign': true,
                        'ReadOnly': (o.EditMode === 'PrintReport'),
                        'cellset': {
                            rows: 1,
                            columns: 2
                        },
                        'onSwap': function (e, onSwapData)
                        {
                            onSwap(onSwapData);
                        },
                        'showConfigButton': (o.filterToPropId === ''),
                        'onConfigOn': function($buttontable) { 
                            $xml.children().each(function ()
                            {
                                var $propxml = $(this);
                                var $subtable = $layouttable.find('#' + $propxml.CswAttrXml('id') + '_subproptable');
								var $parentcell = $subtable.parent();
                                var $cellset = $layouttable.CswLayoutTable('cellset', $parentcell.CswAttrDom('row'), $parentcell.CswAttrDom('column'));
                                var $propcell = _getPropertyCell($cellset);

                                if($subtable.length > 0)
                                {
									var fieldOpt = {
										'fieldtype': $propxml.CswAttrXml('fieldtype'),
										'nodeid': o.nodeid,
										'relatednodeid': o.relatednodeid,
										'propid': $propxml.CswAttrXml('id'),
										'$propdiv': $propcell.children('div'),
										'$propxml': $propxml,
										'onchange': function() { },
										'onReload': function() { getProps($tabcontentdiv, tabid); },
										'cswnbtnodekey': o.cswnbtnodekey
									};
                                
                                    _updateSubProps(fieldOpt, o.SinglePropUrl, o.EditMode, o.cswnbtnodekey, $propxml.CswAttrXml('id'), o.nodetypeid, $propxml, $propcell, $tabcontentdiv, tabid, true);
                                }
                            });
                        },
                        'onConfigOff': function($buttontable) { 
                            $xml.children().each(function ()
                            {
                                var $propxml = $(this);
                                var $subtable = $layouttable.find('#' + $propxml.CswAttrXml('id') + '_subproptable');
								var $parentcell = $subtable.parent();
                                var $cellset = $layouttable.CswLayoutTable('cellset', $parentcell.CswAttrDom('row'), $parentcell.CswAttrDom('column'));
                                var $propcell = _getPropertyCell($cellset);

                                if($subtable.length > 0)
                                {
									var fieldOpt = {
										'fieldtype': $propxml.CswAttrXml('fieldtype'),
										'nodeid': o.nodeid,
										'relatednodeid': o.relatednodeid,
										'propid': $propxml.CswAttrXml('id'),
										'$propdiv': $propcell.children('div'),
										'$propxml': $propxml,
										'onchange': function() { },
										'onReload': function() { getProps($tabcontentdiv, tabid); },
										'cswnbtnodekey': o.cswnbtnodekey
									};

                                    _updateSubProps(fieldOpt, o.SinglePropUrl, o.EditMode, o.cswnbtnodekey, $propxml.CswAttrXml('id'), o.nodetypeid, $propxml, $propcell, $tabcontentdiv, tabid, false);
                                }
                            });
                        }

                    });

                    var i = 0;

                    _handleProps($layouttable, $xml, $tabcontentdiv, tabid);

                    if(o.EditMode !== 'PrintReport')
                    {
                        var $savetab = $form.CswButton({ID: 'SaveTab', 
                                                enabledText: 'Save Changes', 
                                                disabledText: 'Saving...', 
                                                onclick: function () { Save($form, $layouttable, $xml, $savetab); }
                                                });
                    }

                    // Validation
                    $form.validate({
                        highlight: function (element, errorClass)
                        {
                            var $elm = $(element);
                            $elm.CswAttrDom('csw_invalid', '1');
                            $elm.animate({ backgroundColor: '#ff6666' });
                        },
                        unhighlight: function (element, errorClass)
                        {
                            var $elm = $(element);
                            if($elm.CswAttrDom('csw_invalid') === '1')  // only unhighlight where we highlighted
                            {
                                $elm.css('background-color', '#66ff66');
                                $elm.CswAttrDom('csw_invalid', '0')
                                setTimeout(function () { $elm.animate({ backgroundColor: 'transparent' }); }, 500);
                            }
                        }
                    });
                } // success{}
            });
        } // getProps()

        function onSwap(onSwapData)
        {
            _moveProp(_getPropertyCell(onSwapData.cellset).children('div').first(), onSwapData.swaprow, onSwapData.swapcolumn);
            _moveProp(_getPropertyCell(onSwapData.swapcellset).children('div').first(), onSwapData.row, onSwapData.column);
        } // onSwap()

        function _moveProp($propdiv, newrow, newcolumn)
        {
            if ($propdiv.length > 0)
            {
                var propid = $propdiv.CswAttrDom('propid');

                var dataJson = { 
                    PropId: propid, 
                    NewRow: newrow, 
                    NewColumn: newcolumn, 
                    EditMode: o.EditMode
                };

                CswAjaxJSON({
                    url: o.MovePropUrl,
                    data: dataJson,
                    success: function (result)
                    {

                    }
                });
            }
        } // _moveProp()

        function _getLabelCell($cellset)
        {
            return $cellset[1][1].children('div');
        }
        function _getPropertyCell($cellset)
        {
            return $cellset[1][2].children('div');
        }

        function _handleProps($layouttable, $xml, $tabcontentdiv, tabid, ConfigMode)
        {
            $xml.children().each(function ()
            {
                var $propxml = $(this);
                var propid = $propxml.CswAttrXml('id');
                var fieldtype = $propxml.CswAttrXml('fieldtype');
                var $cellset = $layouttable.CswLayoutTable('cellset', $propxml.CswAttrXml('displayrow'), $propxml.CswAttrXml('displaycol'));

                if (($propxml.CswAttrXml('display') !== 'false' || ConfigMode ) &&
                    fieldtype !== 'Image' &&
                    fieldtype !== 'Grid' &&
                    (o.filterToPropId === '' || o.filterToPropId === propid))
                {
                    var $labelcell = _getLabelCell($cellset);
                    $labelcell.addClass('propertylabel');
                    if($propxml.CswAttrXml('helptext') !== '')
                    {
                        $('<a href="#" title="'+ $propxml.CswAttrXml('helptext') + '" onclick="return false;">'+ $propxml.CswAttrXml('name') +'</a>')
                            .appendTo($labelcell);
                    }
                    else
                    {
                        $labelcell.append($propxml.CswAttrXml('name'));
                    }
                    if(o.ShowCheckboxes && $propxml.CswAttrXml('copyable') === "true")
                    {
                        var $propcheck = $labelcell.CswInput('init',{ID: 'check_'+ propid,
                                                                        type: CswInput_Types.checkbox,
                                                                        value: false, // Value --not defined?,
                                                                        cssclass: o.ID +'_check'                                                                   
                                                                    }); 
                        $propcheck.CswAttrDom('propid',propid);	
                    }
                }

                var $propcell = _getPropertyCell($cellset);
                $propcell.addClass('propertyvaluecell');

                _makeProp($propcell, $propxml, $tabcontentdiv, tabid, ConfigMode);

            });
        } // _handleProps()

        function _makeProp($propcell, $propxml, $tabcontentdiv, tabid, ConfigMode)
        {
            $propcell.empty();
            if (($propxml.CswAttrXml('display') !== 'false' || ConfigMode ) &&
                (o.filterToPropId === '' || o.filterToPropId === $propxml.CswAttrXml('id')))
            {
                var fieldOpt = {
                    'fieldtype': $propxml.CswAttrXml('fieldtype'),
                    'nodeid': o.nodeid,
                    'relatednodeid': o.relatednodeid,
                    'propid': $propxml.CswAttrXml('id'),
                    '$propdiv': $('<div/>').appendTo($propcell),
                    '$propxml': $propxml,
                    'onchange': function() { },
                    'onReload': function() { getProps($tabcontentdiv, tabid); },
                    'cswnbtnodekey': o.cswnbtnodekey,
                    'EditMode': o.EditMode,
                    'onEditView': o.onEditView,
                    'ReadOnly': isTrue( $propxml.CswAttrXml('readonly') )
                };
                fieldOpt.$propdiv.CswAttrDom('nodeid', fieldOpt.nodeid);
                fieldOpt.$propdiv.CswAttrDom('propid', fieldOpt.propid);
                fieldOpt.$propdiv.CswAttrDom('cswnbtnodekey', fieldOpt.cswnbtnodekey);

                fieldOpt.onchange = function () { o.onPropertyChange(fieldOpt.propid, $propxml.CswAttrXml('name')); };
                if ($propxml.CswAttrXml('hassubprops') === "true")
                {
                    fieldOpt.onchange = function ()
                    {
                        _updateSubProps(fieldOpt, o.SinglePropUrl, o.EditMode, o.cswnbtnodekey, $propxml.CswAttrXml('id'), o.nodetypeid, $propxml, $propcell, $tabcontentdiv, tabid, false);
                        o.onPropertyChange(fieldOpt.propid, $propxml.CswAttrXml('name'));
                    };
                } // if ($propxml.CswAttrXml('hassubprops') === "true")

                $.CswFieldTypeFactory('make', fieldOpt);

                // recurse on sub-props
                var $subprops = $propxml.children('subprops');

                var $subtable = $propcell.CswLayoutTable('init', {
                    'ID': fieldOpt.propid + '_subproptable',
                    'OddCellRightAlign': true,
                    'ReadOnly': (o.EditMode === 'PrintReport'),
                    'cellset': {
                        rows: 1,
                        columns: 2
                    },
                    'onSwap': function (e, onSwapData)
                    {
                        onSwap(onSwapData);
                    },
                    'showConfigButton': false
                });

                if (($subprops.length > 0 && $subprops.children('[display != "false"]').length > 0) || ConfigMode)
                {
                    _handleProps($subtable, $subprops, $tabcontentdiv, tabid, ConfigMode);
                    if(ConfigMode) {
                        $subtable.CswLayoutTable('ConfigOn');
                    } else {
                        $subtable.CswLayoutTable('ConfigOff');
                    }
                }
            } // if ($propxml.CswAttrXml('display') != 'false' || ConfigMode )
        } // _makeProp()

        function _updateSubProps(fieldOpt, SinglePropUrl, EditMode, cswnbtnodekey, PropId, nodetypeid, $propxml, $propcell, $tabcontentdiv, tabid, ConfigMode)
        {
			// do a fake 'save' to update the xml with the current value
			$.CswFieldTypeFactory('save', fieldOpt);

			// update the propxml from the server
			var dataXml = {
				EditMode: EditMode,
				NodeId: o.nodeid,
				SafeNodeKey: cswnbtnodekey,
				PropId: PropId,
				NodeTypeId: nodetypeid,
				NewPropXml: xmlToString($propxml)
			};

			CswAjaxXml({
				url: SinglePropUrl,
				data: dataXml,
				stringify: true,
				success: function ($xml)
				{
					_makeProp($propcell, $xml.children().first(), $tabcontentdiv, tabid, ConfigMode);
				}
			});
        } // _updateSubProps()

        function Save($form, $layouttable, $propsxml, $savebtn)
        {
            if($form.valid())
            {
                _updatePropXmlFromForm($layouttable, $propsxml);
                var data = {
                    'EditMode': o.EditMode,
                    'NodeId': o.nodeid,
                    'SafeNodeKey': o.cswnbtnodekey,
                    'NodeTypeId': o.nodetypeid,
                    'ViewId': $.CswCookie('get', CswCookieName.CurrentViewId),
                    'NewPropsXml': xmlToString($propsxml)
                };

                CswAjaxJSON({
                    url: o.SavePropUrl,
                    //data: "{ EditMode: '" + o.EditMode + "', SafeNodeKey: '" + o.cswnbtnodekey + "', NodeTypeId: '" + o.nodetypeid + "', ViewId: '"+ $.CswCookie('get', CswCookieName.CurrentView.ViewId) +"', NewPropsXml: '" + safeJsonParam(xmlToString($propsxml)) + "' }",
                    data: data,
                    success: function (data)
                    {
                        var doSave = true;
                        if(o.ShowCheckboxes)
                        {
                            // apply the newly saved checked property values on this node to the checked nodes
                            var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
                            var $propchecks = $('.' + o.ID + '_check:checked');
                            if($nodechecks.length > 0 && $propchecks.length > 0)
                            {
                                var dataJson = {
                                    SourceNodeKey: o.cswnbtnodekey,
                                    CopyNodeIds: [],
                                    PropIds: []
                                };
                                
                                $nodechecks.each(function() { 
                                    var nodeid = $(this).CswAttrDom('nodeid');
                                    dataJson.CopyNodeIds.push(nodeid); 
                                });

                                $propchecks.each(function() { 
                                    var propid = $(this).CswAttrDom('propid');
                                    dataJson.PropIds.push(propid);
                                });

                                CswAjaxJSON({
                                    url: o.CopyPropValuesUrl,
                                    data: dataJson
                                }); // ajax
                            } // if($nodechecks.length > 0 && $propchecks.length > 0)
                            else
                            {
                                doSave = false;
                                alert('You have not selected any properties to save.');
                            }
                        } // if(o.ShowCheckboxes)
                        if( doSave ) o.onSave(data.nodeid, data.cswnbtnodekey);
                    }, // success
                    error: function()
                    {
                        $savebtn.CswButton('enable');
                    }
                }); // ajax
            } // if($form.valid())
            else 
            {
                $savebtn.CswButton('enable');
            }
        } // Save()

        function _updatePropXmlFromForm($layouttable, $propsxml)
        {
            $propsxml.children().each(function ()
            {
                var propOpt = {
                    '$propxml': $(this),
                    '$propdiv': '',
                    '$propCell': '',
                    'fieldtype': '',
                    'nodeid': o.nodeid,
                    'cswnbtnodekey': o.cswnbtnodekey
                };
                propOpt.fieldtype = propOpt.$propxml.CswAttrXml('fieldtype');
                var $cellset = $layouttable.CswLayoutTable('cellset', propOpt.$propxml.CswAttrXml('displayrow'), propOpt.$propxml.CswAttrXml('displaycol'));
                propOpt.$propcell = _getPropertyCell($cellset);
                propOpt.$propdiv = propOpt.$propcell.children('div').first();

                $.CswFieldTypeFactory('save', propOpt);

                // recurse on subprops
                if (propOpt.$propxml.CswAttrXml('hassubprops') === "true")
                {
                    var $subprops = propOpt.$propxml.children('subprops');
                    if ($subprops.length > 0 && $subprops.children('[display != "false"]').length > 0)
                    {
                        var $subtable = propOpt.$propcell.children('#' + propOpt.$propxml.CswAttrXml('id') + '_subproptable').first();
                        if($subtable.length > 0)
                        {
                            _updatePropXmlFromForm($subtable, $subprops);
                        }
                    }
                }
            }); // each()
        } // _updatePropXmlFromForm()

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);

